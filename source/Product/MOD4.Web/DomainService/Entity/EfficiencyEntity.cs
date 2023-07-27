using System.Collections.Generic;

namespace MOD4.Web.DomainService.Entity
{
    public class EfficiencyEntity
    {
        public string Process { get; set; }
        public string Floor { get; set; }
        public List<EfficiencyDetailEntity> InfoList { get; set; }
        public int TTLPassQty { get; set; }
        public decimal EfficiencyInlineTTL { get; set; }
        public decimal EfficiencyInlineOfflineTTL { get; set; }
        public int TTLcount { get; set; }
    }
}
