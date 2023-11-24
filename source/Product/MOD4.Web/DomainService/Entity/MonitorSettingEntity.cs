using System;

namespace MOD4.Web.DomainService.Entity
{
    public class MonitorSettingEntity
    {
        public int Node { get; set; }
        public string EqNumber { get; set; }
        public decimal DefTopRate { get; set; }
        public decimal DefLeftRate { get; set; }
        public decimal DefWidth { get; set; }
        public decimal DefHeight { get; set; }
        public string Border { get; set; }
        public string Background { get; set; }
        public int Floor { get; set; }
        public DateTime UpdateTime { get; set; }
        public string UpdateUser { get; set; }
    }
}
