using System;
using System.Collections.Generic;
using System.Text;

namespace FixProUs.Models
{
    public class TaxModel
    {
        public int Id { get; set; }
        public int? AccountId { get; set; }
        public int? BrancheId { get; set; }
        public string Taxname { get; set; }
        public string Description { get; set; }
        public decimal? Rate { get; set; }
        public bool? Globaldefaultrate { get; set; }
        public string Notes { get; set; }
        public bool? Active { get; set; }
        public int? CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
