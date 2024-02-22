using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using System;
using System.Collections.Generic;

namespace MOD4.Web.ViewModel
{
    public class IELayoutAuditViewModel
    {
        public int LayoutOrderSn { get; set; }
        public int AuditAccountSn { get; set; }
        public AuditStatusEnum AuditStatusId { get; set; }
        public string Remark { get; set; }
        public List<OptionEntity> SecretLevelList { get; set; }
        public DateTime ExptOutputDate { get; set; }
        public string Version { get; set; }
    }
}