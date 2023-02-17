using System;
using System.Collections.Generic;

namespace MOD4.Web.DomainService.Entity
{
    public class SPCMainEntity
    {
        public string EquipmentId { get; set; }
        public string Result { get; set; }
        public string ProductId { get; set; }
        public string DataGroup { get; set; }
        public int Count { get; set; }
        public int OOSCount { get; set; }
        public int OOCCount { get; set; }
        public int OORCount { get; set; }
    }
}
