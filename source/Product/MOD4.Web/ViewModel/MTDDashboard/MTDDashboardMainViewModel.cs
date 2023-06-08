using System.Collections.Generic;
using System.ComponentModel;

namespace MOD4.Web.ViewModel
{
    public class MTDDashboardMainViewModel
    {
        public string Process { get; set; }

        [DisplayName("PLAN")]
        public string Plan { get; set; }

        [DisplayName("ACTUAL")]
        public string Actual { get; set; }

        [DisplayName("DIFF")]
        public string Diff { get; set; }

        public List<MTDDashboardViewModel> ProcessList { get; set; }
    }
}
