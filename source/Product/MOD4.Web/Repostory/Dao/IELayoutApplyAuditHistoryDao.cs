using MOD4.Web.Enum;
using System;

namespace MOD4.Web.Repostory.Dao
{
    public class IELayoutApplyAuditHistoryDao
    {
        public int IELayoutSn { get; set; }
        public int AuditSn { get; set; }
        public int AuditAccountSn { get; set; }
        public AuditStatusEnum AuditStatusId { get; set; }
        public string AuditName { get; set; }
        public DateTime? ReceivedTime { get; set; }
        public DateTime? AuditTime { get; set; }
        public string Remark { get; set; }
    }
}