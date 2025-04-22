using System;
using System.Collections.Generic;
using System.Text;

namespace FixProUs.Models
{
    public class EmployeesInPageModel
    {
        public List<EmployeeModel> EmployeesInPage { get; set; }
        public int Pages { get; set; }
        public int CurrentPage { get; set; }
    }
}
