using System.Collections.Generic;
using System.ComponentModel;

namespace MOD4.Web.ViewModel
{
    public class MTDDashboardViewModel
    {
        public string Process { get; set; }

        [DisplayName("PLAN")]
        public int Plan { get; set; }

        [DisplayName("ACTUAL")]
        public int Actual { get; set; }

        [DisplayName("DIFF")]
        public int Diff {
            get
            {
                return 0;
            } 
            set 
            {
                value = this.Plan - this.Actual;
            } 
        }

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
