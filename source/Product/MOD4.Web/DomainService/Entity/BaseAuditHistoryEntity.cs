using MOD4.Web.Enum;
using System;

namespace MOD4.Web.DomainService.Entity
{
    public class BaseAuditHistoryEntity
    {
        public int AuditSn { get; set; }
        public int AuditAccountSn { get; set; }
        public string AuditAccountName { get; set; }
        public string Status { get; set; }
        public DateTime? ReceivedTime { get; set; }
        public string ReceivedTimeStr { get; set; }
        public DateTime? AuditTime { get; set; }
        public string AuditTimeStr { get; set; }
        public string AuditRemark { get; set; }
        public bool IsDel { get; set; }
        public string Duration { get; set; }
    }
}
