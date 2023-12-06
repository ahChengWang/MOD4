using System.Collections.Generic;

namespace MOD4.Web.ViewModel
{
    public class MonitorViewModel
    {
        public List<MonitorEqInfoViewModel> EqInfoList { get; set; }

        public List<MonitorAlarmDayTopViewModel> AlarmDayTop { get; set; }

        public List<MonitorDailyMTDViewModel> DailyMTDList { get; set; }
    }
}
