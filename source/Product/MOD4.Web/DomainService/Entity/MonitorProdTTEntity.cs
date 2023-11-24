using System;
using System.Collections.Generic;

namespace MOD4.Web.DomainService.Entity
{
    public class MonitorProdTTEntity
    {
        public int Node { get; set; }
        public int LcmProdSn { get; set; }
        public string ProdDesc { get; set; }
        public string DownEquipment { get; set; }
        public int TimeTarget { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
