using System;
using System.Collections.Generic;
using System.Text;

namespace FixProUs.Models
{
    public class PaymentsModel
    {
        public int Id { get; set; }
        public int? AccountId { get; set; }
        public int? BrancheId { get; set; }
        public int? CustomerId { get; set; }
        public int? ContractId { get; set; }
        public int? InvoiceId { get; set; }
        public int? ExpensesId { get; set; }
        public DateTime? PaymentDate { get; set; }
        public decimal? Amount { get; set; }
        public decimal? OverAmount { get; set; }
        public int? Type { get; set; }
        public int? Method { get; set; }
        public int? IncreaseDecrease { get; set; }
        public string TransactionID { get; set; }
        public string CheckNumber { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string SignaturePrintName { get; set; }
        public string SignatureDraw { get; set; }
        public string Notes { get; set; }
        public bool? Active { get; set; }
        public int? CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
