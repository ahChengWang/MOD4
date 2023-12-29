using System.Collections.Generic;

namespace MOD4.Web.DomainService.Entity
{
    public class MonitorAlarmEntity
    {
        public string EqNumber { get; set; }

        public string StatusCode { get; set; }

        public string Comment { get; set; }

        public bool IsFrontEnd { get; set; }

        public string ProdNo { get; set; }

        public string StartTime { get; set; }
    }
}
