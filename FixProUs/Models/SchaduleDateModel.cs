using System;
using System.Collections.Generic;
using System.Text;

namespace FixProUs.Models
{
    public class SchaduleDateModel
    {
        public int Id { get; set; }
        public int? AccountId { get; set; }
        public int? BrancheId { get; set; }
        public int? ScheduleId { get; set; }
        public string Date { get; set; }
        public string ScheduleStartTime { get; set; }
        public string ScheduleEndTime { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string SpentTimeHour { get; set; }
        public string SpentTimeMin { get; set; }
        public int? Status { get; set; }
        public string CalendarColor { get; set; }
        public string Reasonnotserve { get; set; }
        public int? InvoiceId { get; set; }
        public int? EstimateId { get; set; }
        public int? CreateOrginal_Custom { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string GoogleReviewLink { get; set; }
        public string Notes { get; set; }
        public bool? Active { get; set; }
        public int? CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public EmployeeModel OneEmployee { get; set; } = new EmployeeModel();

        public string[] ArrStringFrom { get { return ScheduleStartTime != null ? ScheduleStartTime.Split(':') : "12:00".Split(':'); } }

        public string[] ArrStringTo { get { return ScheduleEndTime != null ? ScheduleEndTime.Split(':') : "12:00".Split(':'); } }
        public int TimeHourFrom { get { return int.Parse(ArrStringFrom[0]); } }
        public int TimeMinFrom { get { return int.Parse(ArrStringFrom[1]); } }

        public int TimeHourTo { get { return int.Parse(ArrStringTo[0]); } }
        public int TimeMinTo { get { return int.Parse(ArrStringTo[1]); } }

        public bool IsChecked { get; set; }
    }
}
