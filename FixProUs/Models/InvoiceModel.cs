using System;
using System.Collections.Generic;
using System.Text;

namespace FixProUs.Models
{
    public class InvoiceModel
    {
        public int Id { get; set; }
        public int? AccountId { get; set; }
        public int? BrancheId { get; set; }
        public int? ContractInvoiceId { get; set; }
        public int? ContractId { get; set; }
        public int? EstimateId { get; set; }
        public int? ScheduleId { get; set; }
        public string ScheduleName { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerEmail { get; set; }
        public decimal? Total { get; set; }
        public int? TaxId { get; set; }
        public decimal? Tax { get; set; }
        public decimal? Taxval { get; set; }
        public int? MemberId { get; set; }
        public decimal? Discount { get; set; }
        public string DiscountAmountOrPercent { get; set; }
        public decimal? Paid { get; set; }
        public decimal? Net { get; set; }
        public int? Status { get; set; }
        public int? Type { get; set; }
        public string SignaturePrintName { get; set; }
        public string SignatureDraw { get; set; }
        public string Terms { get; set; }
        public string NotesForCustomer { get; set; }
        public string Notes { get; set; }
        public bool? Active { get; set; }
        public int? CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public List<InvoiceItemServicesModel> LstInvoiceItemServices { get; set; }
        public bool? WhithoutSch { get { return (ScheduleId == null || ScheduleId == 0) ? false : true; } }
        public List<SchaduleDateModel> LstScdDate { get; set; }
    }
}
