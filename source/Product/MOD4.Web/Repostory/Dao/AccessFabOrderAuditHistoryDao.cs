using MOD4.Web.Enum;
using System;

namespace MOD4.Web.Repostory.Dao
{
    public class AccessFabOrderAuditHistoryDao
    {
        public int AuditSn { get; set; }
        public int AccessFabOrderSn { get; set; }
        public int AuditAccountSn { get; set; }
        public string AuditAccountName { get; set; }
        public FabInOutStatusEnum StatusId { get; set; }
        public DateTime? ReceivedTime { get; set; }
        public DateTime? AuditTime { get; set; }
        public string AuditRemark { get; set; }
        public bool IsDel { get; set; }
    }
}
