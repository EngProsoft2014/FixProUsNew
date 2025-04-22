using System;
using System.Collections.Generic;
using System.Text;

namespace FixProUs.Models
{
    public class ScheduleEmployeesModel
    {
        public int Id { get; set; }
        public int? ScheduleDateId { get; set; }
        public int? EmpId { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string SpentTimeHour { get; set; }
        public string SpentTimeMin { get; set; }
        public decimal? Duration { get; set; }
        public decimal? Labor { get; set; }
        public int? Status { get; set; }
        public string Reasonnotserve { get; set; }
        public string Notes { get; set; }
        public int? CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }

        public string EmpUserName { get; set; }
        public string EmpFirstName { get; set; }
        public string EmpLastName { get; set; }
        public string EmpFullName { get; set; }
        public string ScheduleDate { get; set; }
    }
}
