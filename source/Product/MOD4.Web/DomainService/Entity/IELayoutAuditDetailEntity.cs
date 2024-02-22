using MOD4.Web.Enum;
using System;

namespace MOD4.Web.DomainService.Entity
{
    public class IELayoutAuditDetailEntity
    {
        public int LayoutOrderSn { get; set; }
        public int AuditSn { get; set; }
        public int AuditAccountSn { get; set; }
        public string AuditName { get; set; }
        public AuditStatusEnum AuditStatusId { get; set; }
        public string AuditStatus { get; set; }
        public string ReceivedTime { get; set; }
        public string AuditTime { get; set; }
        public string Remark { get; set; }
        public string DiffTime { get; set; }
    }
}
