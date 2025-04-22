
using System;
using System.Collections.Generic;
using System.Text;
using FixProUs.Controls;
using Syncfusion.Maui.Calendar;


namespace FixProUs.Model
{
    public class CalendarModel
    {
        public CalendarDateRange SelectedRange { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

    }


}
