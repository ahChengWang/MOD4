using MOD4.Web.Enum;
using System;

namespace MOD4.Web.Repostory.Dao
{
    public class TakeBackWTDetailDao
    {
        public int TakeBackWtSn { get; set; }
        public ProcessEnum ProcessId { get; set; }
        public string EqId { get; set; }
        public int Prod { get; set; }
        public decimal IEStandard { get; set; }
        public decimal IETT { get; set; }
        public decimal IEWT { get; set; }
        public int PassQty { get; set; }
        public decimal TakeBackWT { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateTime { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
