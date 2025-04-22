using System;
using System.Collections.Generic;
using System.Text;

namespace FixProUs.Models
{
    public class EstimateScheduleDateModel
    {
        public int Id { get; set; }
        public int? AccountId { get; set; }
        public int? EstimateId { get; set; }
        public int? ScheduleDateId { get; set; }
        public bool? Active { get; set; }
        public int? CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
