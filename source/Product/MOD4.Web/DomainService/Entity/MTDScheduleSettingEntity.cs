﻿using System;
using System.Collections.Generic;

namespace MOD4.Web.DomainService.Entity
{
    public class MTDScheduleSettingEntity
    {
        public int Sn { get; set; }
        public string Process { get; set; }
        public int LcmProdSn { get; set; }
        public int PassNode { get; set; }
        public int WipNode { get; set; }
        public string EqNo { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateTime { get; set; }

    }
}
