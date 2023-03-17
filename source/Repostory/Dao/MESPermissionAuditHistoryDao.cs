using MOD4.Web.Enum;
using System;

namespace MOD4.Web.Repostory.Dao
{
    public class MESPermissionAuditHistoryDao
    {
        public int auditSn { get; set; }
        public int mesPermissionSn { get; set; }
        public int auditAccountSn { get; set; }
        public string auditAccountName { get; set; }
        public DemandStatusEnum statusId { get; set; }
        public DateTime? receivedTime { get; set; }
        public DateTime? auditTime { get; set; }
        public string auditRemark { get; set; }
        public bool isDel { get; set; }
        public string Mail { get; set; }
    }
}
