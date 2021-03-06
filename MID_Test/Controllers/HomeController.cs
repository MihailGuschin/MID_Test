using MID_Test.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MID_Test.Controllers
{
    public class HomeController : Controller
    {
        private readonly EmployeeContext db = new EmployeeContext();
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Edit(Employee model)
        {
            Validate(model);
            if(ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("Index", model);
        }

        [HttpGet]
        public JsonResult GetAll()
        {
            return this.Json( db.Employees.ToList(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SendFile(HttpPostedFileBase upload)
        {
            var e = db.Employees.ToList();
            var ej = JsonConvert.SerializeObject(e);
            upload = Request.Files["file1"];
            
            if (upload != null && upload.ContentType.Contains("json"))
            {
                byte[] jsonArray = new byte[upload.ContentLength];
                upload.InputStream.Read(jsonArray, 0, upload.ContentLength);

                string jsonStr = Encoding.UTF8.GetString(jsonArray);
                List<Employee> empls = null;
                try
                {
                    empls = JsonConvert.DeserializeObject<List<Employee>>(jsonStr);
                }
                catch
                {
                    ModelState.AddModelError("", "Неверный формат файла");
                    return RedirectToAction("Index");
                }

                StringBuilder sb = new StringBuilder();               

                foreach(var employee in empls)
                {
                    if (employee.IsInternal && employee.Number == null)
                    {
                        sb.Append($"ошибка для {employee.Name} - {employee.Number}");
                        sb.AppendLine();
                        continue;
                    }
                    if (employee.IsInternal == false && employee.Number != null)
                    {
                        sb.Append($"ошибка для {employee.Name} - {employee.Number}");
                        sb.AppendLine();
                        continue;
                    }
                    if (db.Employees.Any(x => x.Id == employee.Id))
                    {
                        sb.Append($"Изменена запись {employee.Id} {employee.Name} - {employee.Number}");
                        sb.AppendLine();
                        db.Entry(employee).State = EntityState.Modified;
                    }
                    else
                    {
                        sb.Append($"Добавлена запись {employee.Id} {employee.Name} - {employee.Number}");
                        sb.AppendLine();
                        db.Employees.Add(employee);
                    }                    
                }
                db.SaveChanges();

                return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/plain", "result.txt");
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Index(Employee employee)
        {
            if (db.Employees.Any(x => x.IsInternal == employee.IsInternal && x.Number == employee.Number))
            {
                ModelState.AddModelError("", "Сотрудник уже существует");
            }
            Validate(employee);
            if (ModelState.IsValid)
            {
                var employees = db.Employees;
                employees.Add(employee);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(employee);
        }

        private void Validate(Employee employee)
        {            
            if (employee.IsInternal && employee.Number == null)
            {
                ModelState.AddModelError("", "Если сотрудник штатный, то обязательно нужно указывать табельный номер");
            }
            if (employee.IsInternal == false && employee.Number != null)
            {
                ModelState.AddModelError("", "Если сотрудник внешний, то поле табельным номером должно быть пустым");
            }
        }
    }
}