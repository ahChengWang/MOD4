using System.Collections.Generic;

namespace MOD4.Web.ViewModel
{
    public class MonitorViewModel
    {
        public List<MonitorAlarmViewModel> AlarmList { get; set; }

        public List<MonitorAlarmDayTopViewModel> AlarmDayTop { get; set; }

        public List<MonitorProdPerInfoViewModel> ProdPerInfo { get; set; }
    }
}
