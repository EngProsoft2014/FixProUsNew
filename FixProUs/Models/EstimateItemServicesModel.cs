using System;
using System.Collections.Generic;
using System.Text;

namespace FixProUs.Models
{
    public class EstimateItemServicesModel
    {
        public int Id { get; set; }
        public int? AccountId { get; set; }
        public int? BrancheId { get; set; }
        public int? EstimateId { get; set; }
        public int? ItemsServicesId { get; set; }
        public string ItemsServicesName { get; set; }
        public decimal? Price { get; set; }
        public int? TaxId { get; set; }
        public decimal? Tax { get; set; }
        public decimal? Total { get; set; }
        public int? Quantity { get; set; }
        public bool? Taxable { get; set; }
        public bool? Discountable { get; set; }
        public string Unit { get; set; }
        public bool? Active { get; set; }
        public int? CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
