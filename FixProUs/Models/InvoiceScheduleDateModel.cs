using System;
using System.Collections.Generic;
using System.Text;

namespace FixProUs.Models
{
    public class InvoiceScheduleDateModel
    {
        public int Id { get; set; }
        public int? AccountId { get; set; }
        public int? InvoiceId { get; set; }
        public int? ScheduleDateId { get; set; }
        public bool? Active { get; set; }
        public int? CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
