using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MID_Test.Models
{
    public class Employee
    {
        public int Id { get; set; }
        [Display(Name ="Табельный номер")]
        public int? Number { get; set; }
        [Display(Name = "ФИО")]
        public string Name { get; set; }
        [Display(Name = "Пол")]
        public Gender? Gender { get; set; }
        [Display(Name = "Дата рождения")]
        public DateTime? BirthDate { get; set; }
        [Display(Name = "Штатный сотрудник")]
        public bool IsInternal { get; set; }
    }
    public enum Gender
    {
        [Display(Name ="Мужской")]
        Male,
        [Display(Name = "Женский")]
        Female
    }
}