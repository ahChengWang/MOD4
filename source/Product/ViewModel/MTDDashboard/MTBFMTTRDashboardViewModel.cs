using System.Collections.Generic;
using System.ComponentModel;

namespace MOD4.Web.ViewModel
{
    public class MTBFMTTRDashboardViewModel
    {
        [DisplayName("MTBF Target")]
        public string MTBFTarget { get; set; }

        [DisplayName("MTBF Actual")]
        public string MTBFActual { get; set; }

        [DisplayName("MTTR Target")]
        public string MTTRTarget { get; set; }

        [DisplayName("MTTR Actual")]
        public string MTTRActual { get; set; }

        public List<MTTRDetailViewModel> MTTRDetail { get; set; }

        public List<MTBFMTTREqInfoViewModel> EqpInfoDetail { get; set; }
    }
}
