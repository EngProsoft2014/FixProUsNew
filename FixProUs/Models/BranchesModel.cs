using System;
using System.Collections.Generic;
using System.Text;

namespace FixProUs.Models
{
    public class BranchesModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? AccountId { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public bool? DefaultBranches { get; set; }
        public string GoogleReviewLink { get; set; }
        public string Notes { get; set; }
        public string Logo { get; set; }
        public bool? Active { get; set; }
        public int? CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
