using System.Collections.Generic;
using System.ComponentModel;

namespace MOD4.Web.ViewModel
{
    public class MTDDashboardViewModel
    {
        public string Process { get; set; }

        [DisplayName("PLAN")]
        public string Plan { get; set; }

        [DisplayName("ACTUAL")]
        public string Actual { get; set; }

        [DisplayName("DIFF")]
        public string Diff { get; set; }

        [DisplayName("DOWN TIME")]
        public string DownTime { get; set; }

        [DisplayName("DOWN%")]
        public string DownPercent { get; set; }

        [DisplayName("UP%")]
        public string UPPercent { get; set; }

        [DisplayName("RUN%")]
        public string RUNPercent { get; set; }

        [DisplayName("UPH%")]
        public string UPHPercent { get; set; }

        [DisplayName("OEE%")]
        public string OEEPercent { get; set; }

        public List<MTDDashboardDetailViewModel> MTDDetail { get; set; }
    }
}
