using System;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.DomainService.Entity
{

    public class MTDDashboardMainEntity
    {
        public string Process { get; set; }

        public List<MTDDashboardSubEntity> MTDSubList { get; set; }

    }

    public class MTDDashboardSubEntity
    {
        public string Process { get; set; }
        public int Plan { get; set; }
        public int Actual { get; set; }
        public int Diff { get; set; }

        public string DownTime { get; set; }

        public string DownPercent { get; set; }

        public string UPPercent { get; set; }

        public string RUNPercent { get; set; }

        public string UPHPercent { get; set; }

        public string OEEPercent { get; set; }

        public int Sn { get; set; }

        public int EqNo { get; set; }

        public List<MTDDashboardDetailEntity> MTDDetail { get; set; }
    }
}
