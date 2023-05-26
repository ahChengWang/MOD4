using System.Collections.Generic;
using System.ComponentModel;

namespace MOD4.Web.ViewModel
{
    public class MTDDashboardDetailViewModel
    {
        [DisplayName("日期")]
        public string Date { get; set; }

        [DisplayName("主機台")]
        public string Equipment { get; set; }

        [DisplayName("大機種")]
        public string BigProduct { get; set; }

        [DisplayName("當月排程機種")]
        public string PlanProduct { get; set; }

        [DisplayName("產出")]
        public string Output { get; set; }

        [DisplayName("計畫(當天)")]
        public string DayPlan { get; set; }

        [DisplayName("計畫(時段)")]
        public string RangPlan { get; set; }

        [DisplayName("差異(時段)")]
        public string RangDiff { get; set; }

        [DisplayName("當月計畫")]
        public string MonthPlan { get; set; }

        [DisplayName("MTD計畫")]
        public string MTDPlan { get; set; }

        [DisplayName("MTD實際")]
        public string MTDActual { get; set; }

        [DisplayName("MTD差異")]
        public string MTDDiff { get; set; }

        [DisplayName("異常機況")]
        public string EqAbnormal { get; set; }

        [DisplayName("時間")]
        public string RepaireTime { get; set; }

        [DisplayName("狀態")]
        public string Status { get; set; }
    }
}
