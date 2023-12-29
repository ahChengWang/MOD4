using System.Collections.Generic;

namespace MOD4.Web.DomainService.Entity
{
    public class MonitorEqTTEntity
    {
        public List<MonitorProdPerInfoEntity> ProdPerformanceList { get; set; }

        public List<MonitorAlarmTopEntity> AlarmDayTop { get; set; }

        public List<MTDProcessDailyEntity> DailyMTD { get; set; }
    }
}
