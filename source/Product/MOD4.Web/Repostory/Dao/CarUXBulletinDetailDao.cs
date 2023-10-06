using System;

namespace MOD4.Web.Repostory.Dao
{
    public class CarUXBulletinDetailDao
    {
        public int BulletinSn { get; set; }
        public int AccountId { get; set; }
        public int Status { get; set; }
        public DateTime? ReadDate { get; set; }
        public string Ip { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
