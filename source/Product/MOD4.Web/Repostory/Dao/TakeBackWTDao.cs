using MOD4.Web.Enum;
using System;

namespace MOD4.Web.Repostory.Dao
{
    public class TakeBackWTDao
    {
        public int Sn { get; set; }
        public DateTime ProcessDate { get; set; }
        public WTCategoryEnum WTCategoryId { get; set; }
        public decimal BondTakeBack { get; set; }
        public decimal FogTakeBack { get; set; }
        public decimal LamTakeBack { get; set; }
        public decimal AssyTakeBack { get; set; }
        public decimal CDPTakeBack { get; set; }
        public decimal TTLTakeBack { get; set; }
        public decimal TakeBackPercnet { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateTime { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
