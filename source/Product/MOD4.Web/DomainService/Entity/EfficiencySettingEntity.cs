using MOD4.Web.Enum;
using System;

namespace MOD4.Web.DomainService.Entity
{
    public class EfficiencySettingEntity
    {
        public int LcmProdSn { get; set; }
        public string ProdNo { get; set; }
        public string Process { get; set; }
        public ProcessEnum ProcessId { get; set; }
        public string Shift { get; set; }
        public int Node { get; set; }
        public decimal WT { get; set; }
        public int InlineEmps { get; set; }
        public int OfflineEmps { get; set; }
        public int Floor { get; set; }
        public DateTime UpdateTime { get; set; }
        public string UpdateUser { get; set; }
    }
}
