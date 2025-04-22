using System;
using System.Collections.Generic;
using System.Text;

namespace FixProUs.Models
{
    public class ObjectOfCustomerModel
    {
        public SchedulesModel ObjSchedule { get; set; }
        public EstimateModel ObjEstimate { get; set; }
        public InvoiceModel ObjInvoice { get; set; }
    }
}
