using MOD4.Web.Enum;
using System;
using System.Collections.Generic;

namespace MOD4.Web.DomainService.Entity
{
    public class MonitorTackTimeEntity
    {
        public int ProdSn { get; set; }
        public string ProdDesc { get; set; }
        public List<MonitorTackTimeDetailEntity> DetailTTInfo { get; set; }
    }

    public class MonitorTackTimeDetailEntity
    {
        public int Node { get; set; }
        public string EquipmentNo { get; set; }
        public decimal TargetTackTime { get; set; }
        public decimal TackTime { get; set; }
        public string TTWarningLevel { get; set; }
        public TTWarningLevelEnum TTWarningLevelId { get; set; }
    }
}
