using MOD4.Web.Enum;

namespace MOD4.Web.DomainService.Entity
{
    public class MonitorEqTTInfoEntity : BaseMonitorEqAreaSettingEntity
    {

        public string TackTime { get; set; }

        public TTWarningLevelEnum TTWarningLevelId { get; set; }
    }
}
