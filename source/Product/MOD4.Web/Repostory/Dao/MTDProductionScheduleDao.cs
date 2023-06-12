using MOD4.Web.Enum;
using System;

namespace MOD4.Web.Repostory.Dao
{
    public class MTDProductionScheduleDao
    {
        public int Sn { get; set; }
        public string Process { get; set; }
        public MTDCategoryEnum MTDCategoryId { get; set; }
        public string Node { get; set; }
        public string Model { get; set; }
        public int LcmProdId { get; set; }
        public DateTime Date { get; set; }
        public int Qty { get; set; }
        public int Floor { get; set; }
        public bool IsMass { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateTime { get; set; }
        public int MonthPlan { get; set; }
    }
}
