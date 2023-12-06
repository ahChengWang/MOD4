using System;

namespace MOD4.Web.DomainService.Entity
{
    public class MTDProcessDailyEntity
    {
        public int Sn { get; set; }
        public string Process { get; set; }
        public int DayPlanQty { get; set; }

        private int _nowTime = DateTime.Now.AddMinutes(-450).Hour + 1;
        public int DayPeriodPlanQty
        {
            get
            {
                return Convert.ToInt32(Math.Round(Convert.ToDouble(DayPlanQty) / Convert.ToDouble(24), 0)) * _nowTime;
            }
        }
        public int DayActQty { get; set; }
        public int DayDiff
        {
            get
            {
                return this.DayActQty - this.DayPeriodPlanQty;
            }
        }
        public decimal MeetRate
        {
            get
            {
                return this.DayPlanQty == 0 ? 0 : Convert.ToDecimal(Convert.ToDouble(this.DayActQty) / Convert.ToDouble(this.DayPeriodPlanQty));
            }
        }
    }
}
