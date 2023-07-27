using System.Collections.Generic;

namespace MOD4.Web.ViewModel
{
    public class EfficiencyViewModel
    {
        public string Process { get; set; }
        public string Floor { get; set; }
        public List<EfficiencyInfoViewModel> InfoList { get; set; }
        public int TTLPassQty { get; set; }
        public decimal EfficiencyInlineTTL { get; set; }
        public decimal EfficiencyInlineOfflineTTL { get; set; }
        public int TTLcount { get; set; }
    }
}
