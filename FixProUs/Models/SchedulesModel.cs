using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;


namespace FixProUs.Models
{
    public class SchedulesModel : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public int ScheduleDateId { get; set; }
        public int? CountPhotos { get; set; }
        public int? AccountId { get; set; }
        public int? BrancheId { get; set; }
        public int? ContractId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool? Recurring { get; set; }
        public int? FrequencyType { get; set; }
        public string StartDate { get; set; }
        public string ScheduleDate { get; set; }
        public string Time { get; set; }
        public int? EndType { get; set; }
        public string EndDate { get; set; }
        public string CalendarColor { get; set; }
        public bool? ShowMoreOptions { get; set; }
        public bool? InvoiceableTask { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string Location { get; set; }
        public int? EmployeeCategoryId { get; set; }
        public string Employees { get; set; }
        public int? CallId { get; set; }

        string _Notes;
        public string Notes
        {
            get
            {
                return _Notes;
            }
            set
            {
                _Notes = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Notes"));
                }
            }
        }
        public bool? Active { get; set; }
        public int? CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? PriorityId { get; set; }
        public string TimeEnd { get; set; }
        public string StartTimeAc { get; set; }
        public string EndTimeAc { get; set; }
        public int? Status { get; set; }
        public int ShowCheckBtn { get { return (!string.IsNullOrEmpty(StartTimeAc) && string.IsNullOrEmpty(EndTimeAc)) ? 1 : (!string.IsNullOrEmpty(StartTimeAc) && !string.IsNullOrEmpty(EndTimeAc)) ? 2 : Status == 0 ? 3 : 0; } set { } } // 0 = show start job btn only // 1 = show end job btn only // 2 = No show eny btns(complete job)// 3 = Not service

        public ScheduleItemsServicesModel OneScheduleService { get; set; }  
        public List<ScheduleItemsServicesModel> LstScheduleItemsServices { get; set; }
        public CustomersModel CustomerDTO { get; set; }
        //public List<EmployeeModel> LstEmployeeDTO { get; set; } = new List<EmployeeModel>();
        public List<ScheduleEmployeesModel> LstScheduleEmployeeDTO { get; set; }
        public SchaduleDateModel OneScheduleDate { get; set; }

        public List<ScheduleMaterialReceiptModel> LstMaterialReceipt { get; set; }

        public List<SchedulePicturesModel> LstSchedulePictures { get; set; }

        public List<ScheduleItemsServicesModel> LstFreeServices { get; set; }

        public List<ScheduleItemsServicesModel> LstFirstCreateServices { get; set; }

        public string[] ArrStringFrom { get { return Time != null ? Time.Split(':') : "12:00".Split(':'); } }

        public string[] ArrStringTo { get { return TimeEnd != null ? TimeEnd.Split(':') : "12:00".Split(':'); } }
        public int TimeHourFrom { get { return int.Parse(ArrStringFrom[0]); } }
        public int TimeMinFrom { get { return int.Parse(ArrStringFrom[1]); } }

        public int TimeHourTo { get { return int.Parse(ArrStringTo[0]); } }
        public int TimeMinTo { get { return int.Parse(ArrStringTo[1]); } }


        public Color Color { get { return Color.FromHex("#FFFFFF"); } }

        public Brush Background { get { return Color.FromHex(CalendarColor); } }

        //public DateTime ConvertStartDate { get { return Convert.ToDateTime(StartDate); } }

        //public DateTime From { get { return new DateTime(ConvertStartDate.Year, ConvertStartDate.Month, ConvertStartDate.Day, TimeHour, TimeMin, 0); } }
        //public DateTime To { get { return new DateTime(ConvertStartDate.Year, ConvertStartDate.Month, ConvertStartDate.Day, TimeHour, TimeMin, 0).AddHours(1); } }


        public DateTime From { get { return new DateTime(Convert.ToDateTime(StartDate).Year, Convert.ToDateTime(StartDate).Month, Convert.ToDateTime(StartDate).Day, TimeHourFrom, TimeMinFrom, 0); } }
        public DateTime To { get { return new DateTime(Convert.ToDateTime(StartDate).Year, Convert.ToDateTime(StartDate).Month, Convert.ToDateTime(StartDate).Day, TimeHourTo, TimeMinTo, 0); } }

        public bool GetPictures { get; set; } = true;
        public int InvoiceOrEstimate { get; set; }

        public EstimateModel EstimateDTO { get; set; }
        public InvoiceModel InvoiceDTO { get; set; }

        public bool IsStopUpdateSchedule { get { return Id != 0 ? false : true; } set { } }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
