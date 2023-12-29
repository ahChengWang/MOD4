using System;
using System.Collections.Generic;

namespace MOD4.Web.DomainService.Entity
{
    public class MonitorEqTTHistoryEntity
    {
        public string ProdNo { get; set; }
        public string Operator { get; set; }
        public decimal MaxTT { get; set; }
        public decimal minTT { get; set; }
        public decimal MedianTT { get; set; }
        public List<ProdPerfDetailEntity> EqTTHistoryList { get; set; }

    }
}
