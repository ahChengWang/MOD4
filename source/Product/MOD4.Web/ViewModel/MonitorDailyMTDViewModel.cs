using System;
using System.Collections.Generic;

namespace MOD4.Web.ViewModel
{
    public class MonitorDailyMTDViewModel
    {
        public string Process { get; set; }
        public int DayPlanQty { get; set; }
        public int DayPeriodPlanQty { get; set; }
        public int DayActQty { get; set; }
        public int DayDiff { get; set; }
        public decimal MeetRate { get; set; }
    }
}
