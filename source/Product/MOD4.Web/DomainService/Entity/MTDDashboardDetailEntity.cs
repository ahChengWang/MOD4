using System.Collections.Generic;

namespace MOD4.Web.DomainService.Entity
{
    public class MTDDashboardDetailEntity
    {
        public string Date { get; set; }

        public string Equipment { get; set; }

        public string BigProduct { get; set; }

        public string Node { get; set; }

        public string PlanProduct { get; set; }

        public int Output { get; set; }

        public int Wip { get; set; }

        public int DayPlan { get; set; }

        public int RangPlan { get; set; }

        public int RangDiff { get; set; }

        public int MonthPlan { get; set; }

        public int MTDPlan { get; set; }

        public int MTDActual { get; set; }

        public int MTDDiff { get; set; }

        public string EqAbnormal { get; set; }

        public string RepaireTime { get; set; }

        public string Status { get; set; }

    }
}
