using System;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.DomainService.Entity
{
    public class MTDDashboardEntity
    {
        public string Process { get; set; }

        private int _plan = 0;
        public string Plan
        {
            get
            {
                _plan = this.MTDDetail.Sum(sum => sum.RangPlan);
                return _plan.ToString("#,0");
            }
            set
            {
                value = _plan.ToString("#,0");
            }
        }

        private int _actual = 0;
        public string Actual
        {
            get
            {
                _actual = this.MTDDetail.Sum(sum => sum.Output);
                return _actual.ToString("#,0");
            }
            set
            {
                value = _actual.ToString("#,0");
            }
        }

        private int _diff = 0;
        public string Diff
        {
            get
            {
                _diff = _actual - _plan;
                return _diff.ToString("#,0");
            }
            set
            {
                value = _diff.ToString("#,0");
            }
        }

        public string DownTime { get; set; }

        public string DownPercent { get; set; }

        public string UPPercent { get; set; }

        public string RUNPercent { get; set; }

        public string UPHPercent { get; set; }

        public string OEEPercent { get; set; }

        public int Sn { get; set; }

        public List<MTDDashboardDetailEntity> MTDDetail { get; set; }
    }
}
