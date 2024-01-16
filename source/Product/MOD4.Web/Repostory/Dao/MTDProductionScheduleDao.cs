using MOD4.Web.Enum;
using System;

namespace MOD4.Web.Repostory.Dao
{
    public class MTDProductionScheduleDao : IEquatable<MTDProductionScheduleDao>
    {
        public int Sn { get; set; }
        public string Process { get; set; }
        public string Model { get; set; }
        public int Node { get; set; }
        public string EqNo { get; set; }
        public int LcmProdSn { get; set; }
        public string ProdId { get; set; }
        public DateTime Date { get; set; }
        public int Value { get; set; }
        public int Floor { get; set; }
        public int OwnerId { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateTime { get; set; }
        public int MonthPlan { get; set; }

        public bool Equals(MTDProductionScheduleDao mtdDap)
        {
            if (mtdDap == null)
                return false;

            return this.Sn == mtdDap.Sn && this.Node == mtdDap.Node && this.EqNo == mtdDap.EqNo && this.ProdId == mtdDap.ProdId && this.Date == mtdDap.Date;
        }

        public override bool Equals(object obj) => Equals(obj as MTDProductionScheduleDao);
        public override int GetHashCode() => ProdId.GetHashCode();
    }
}
