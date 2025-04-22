using System;
using System.Collections.Generic;
using System.Text;

namespace FixProUs.Models
{
    public class MemberModel
    {
        public int Id { get; set; }
        public int? AccountId { get; set; }
        public int? BrancheId { get; set; }
        public string Name { get; set; }
        public bool? MemberType { get; set; }
        public decimal? MemberValue { get; set; }
        public string Notes { get; set; }
        public bool? Active { get; set; }
        public int? CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
