using System;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.DomainService.Entity
{
    public class MTDDashboardEntity
    {
        public string Process { get; set; }

        public int Plan
        {
            get
            {
                return 0;
            }
            set
            {
                value = this.Detail.Sum(sum => Convert.ToInt32(sum.RangPlan));
            }
        }

        public int Actual
        {
            get
            {
                return 0;
            }
            set
            {
                value = this.Detail.Sum(sum => Convert.ToInt32(sum.Output));
            }
        }

        public int Diff 
        {
            get
            {
                return 0;
            }
            set
            {
                value = this.Actual - this.Plan;
            }
        }

        public List<MTDDashboardDetailEntity> Detail { get; set; }
    }
}
