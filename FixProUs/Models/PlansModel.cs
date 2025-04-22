using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixProUs.Models
{
    public class PlansModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string MonthlyPrice { get; set; }

        public string AnnualPrice { get; set; }

        public string CountUsers { get; set; }

        public bool? Support { get; set; }

        public bool? CustomFields { get; set; }

        public bool? UsersNotificationSettings { get; set; }

        public bool? ReminderRules { get; set; }

        public bool? Branches { get; set; }

        public bool? Customers { get; set; }

        public bool? UsersPermission { get; set; }

        public bool? Equipmets { get; set; }

        public bool? TimeSheet { get; set; }

        public bool? Scheduling { get; set; }

        public bool? Expenses { get; set; }

        public bool? InvoiceQuotes { get; set; }

        public bool? Contracts { get; set; }

        public bool? Payment { get; set; }

        public bool? Notes { get; set; }

        public bool? MessagesChat { get; set; }

        public bool? Tracking { get; set; }

        public bool? Map { get; set; }

        public bool? Reporting { get; set; }

        public bool? CustomersSection { get; set; }
    }
}
