using System.Collections.Generic;

namespace MOD4.Web.DomainService.Entity
{
    public class MonitorEntity
    {
        public List<MonitorSettingEntity> MapAreaList { get; set; }

        public List<MonitorAlarmEntity> AlarmList { get; set; }

        public List<MonitorAlarmTopEntity> AlarmDayTop { get; set; }

        public List<MonitorProdPerInfoEntity> ProdPerInfo { get; set; }
    }
}
