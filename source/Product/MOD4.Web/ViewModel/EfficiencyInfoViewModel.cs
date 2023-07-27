using System.Collections.Generic;

namespace MOD4.Web.ViewModel
{
    public class EfficiencyInfoViewModel
    {
        public string ProdNo { get; set; }
        public string Shift { get; set; }
        public int PassQty { get; set; }
        public decimal EfficiencyInline { get; set; }
        public decimal EfficiencyInlineOffline { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string MedianTT { get; set; }
    }
}
