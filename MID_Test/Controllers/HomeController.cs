using MID_Test.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
            return null;
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
            if (upload != null)
            {
                byte[] jsonArray = new byte[upload.ContentLength];
                upload.InputStream.Read(jsonArray, 0, upload.ContentLength);

                string jsonStr = Encoding.UTF8.GetString(jsonArray);
                var empls = JsonConvert.DeserializeObject<List<Employee>>(jsonStr);
                StringBuilder sb = new StringBuilder();               

                foreach(var employee in empls)
                {
                    if (employee.IsInternal && employee.Number == null)
                    {
                        sb.Append($"ошибка для {employee.Number} - {employee.IsInternal}");
                        sb.AppendLine();
                        continue;
                    }
                    if (employee.IsInternal == false && employee.Number != null)
                    {
                        sb.Append($"ошибка для {employee.Number} - {employee.IsInternal}");
                        sb.AppendLine();
                        continue;
                    }
                    if (db.Employees.Any(x => x.Id == employee.Id))
                    {
                        sb.Append($"Изменена запись {employee.Id} {employee.Name}");
                        sb.AppendLine();
                    }
                    else
                    {
                        sb.Append($"Добавлена запись {employee.Id} {employee.Name}");
                        sb.AppendLine();
                    }

                    db.Employees.Add(employee);
                }
                db.SaveChanges();

                //string path = Server.MapPath("~/Files/result.txt");
                //FileStream fs = new FileStream(path, FileMode.Create);
                //System.IO.File.WriteAllText(path, sb.ToString());
                //string file_type = "application/txt";
                //string file_name = "result.txt";
                //return File(fs, file_type, file_name);

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
            if (employee.IsInternal && employee.Number == null)
            {
                ModelState.AddModelError("", "Если сотрудник штатный, то обязательно нужно указывать табельный номер");
            }
            if (employee.IsInternal == false && employee.Number != null)
            {
                ModelState.AddModelError("", "Если сотрудник внешний, то поле табельным номером должно быть пустым");
            }
            if (ModelState.IsValid)
            {
                var employees = db.Employees;                
                employees.Add(employee);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(employee);
        }
    }
}