using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FixProUs.Models
{
    public class EmployeeBranchesModel
    {
        public int Id { get; set; }

        public int? EmployeeId { get; set; }

        public int BranchId { get; set; }

        public string BranchName { get; set; }

        public bool ActiveBranch { get; set; }
    }
}