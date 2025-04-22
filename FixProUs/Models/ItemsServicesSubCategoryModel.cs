using System;
namespace FixProUs.Models
{
	public class ItemsServicesSubCategoryModel
	{
        public int Id { get; set; }
        public int? AccountId { get; set; }
        public int? BrancheId { get; set; }
        public int? ItemsServicesCategoryId { get; set; }
        public string ItemsServicesSubCategory { get; set; }
        public string Picture { get; set; }
        public bool? Active { get; set; }
        public int? CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}

