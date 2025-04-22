using System;
namespace FixProUs.Models
{
	public class ItemsServicesCategoryModel
	{
        public int Id { get; set; }
        public int? AccountId { get; set; }
        public int? BrancheId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public string Picture { get; set; }
        public bool? Active { get; set; }
        public int? CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}

