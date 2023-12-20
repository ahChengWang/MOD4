using MOD4.Web.Enum;
using System.Collections.Generic;

namespace MOD4.Web.ViewModel
{
    public class MonitorProdEqTTViewModel
    {
        public int ProdSn { get; set; }
        public string ProdDesc { get; set; }
        public List<MonitorProdEqTTDetailViewModel> DetailTTInfo { get; set; }
    }

    public class MonitorProdEqTTDetailViewModel
    {
        public int Node { get; set; }
        public string EquipmentNo { get; set; }
        public decimal TargetTackTime { get; set; }
        public decimal TackTime { get; set; }
        public TTWarningLevelEnum TTWarningLevelId { get; set; }
    }
}
