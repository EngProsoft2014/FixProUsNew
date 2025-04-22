using System;
using System.Collections.Generic;
using System.Text;

namespace FixProUs.Models
{
    public class EstimateModel
    {
        public int Id { get; set; }
        public int? AccountId { get; set; }
        public int? BrancheId { get; set; }
        public int? ScheduleId { get; set; }
        public string ScheduleName { get; set; }
        public DateTime? EstimateDate { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerEmail { get; set; }
        public decimal? Total { get; set; }
        public int? TaxId { get; set; }
        public decimal? Tax { get; set; }
        public decimal? Taxval { get; set; }
        public int? MemberId { get; set; }
        public decimal? Discount { get; set; }
        public string DiscountAmountOrPercent { get; set; }
        public decimal? Net { get; set; }
        public int? Status { get; set; }
        public string SignaturePrintName { get; set; }
        public string SignatureDraw { get; set; }
        public string SignatureDrawView { get { return Helpers.Utility.PathServerEstimateSignture + SignatureDraw; } }
        public string Terms { get; set; }
        public string NotesForCustomer { get; set; }
        public string Notes { get; set; }
        public bool? Active { get; set; }
        public int? CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? InvoiceId { get; set; }
        public List<EstimateItemServicesModel> LstEstimateItemServices { get; set; }
        public bool? WhithoutSch { get { return (ScheduleId == null || ScheduleId == 0) ? false : true; } }
        public bool? NotShowConvert { get; set; } = false;
        public bool? GoToInvoice { get; set; } = false;
        public List<SchaduleDateModel> LstScdDate { get; set; }
        //public bool? HaveInvoice { get { return Status == 1 ? true : false; } set { } } 
    }
}
