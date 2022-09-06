using System;

namespace MOD4.Web.DomainService.Entity
{
    public class PerformanceDetailEntity
    {
        public string Prod { get; set; }
        public DateTime Time { get; set; }
        public string Equipment { get; set; }
        public int NG { get; set; }
        public int Qty { get; set; }
        public string Node { get; set; }
    }
}
