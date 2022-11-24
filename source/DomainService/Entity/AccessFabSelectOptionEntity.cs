using MOD4.Web.Enum;
using System;

namespace MOD4.Web.DomainService.Entity
{
    public class AccessFabSelectOptionEntity
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int StatusId { get; set; }
        public int FabInTypeId { get; set; }
        public string OrderNo { get; set; }
        public string Applicant { get; set; }
        public int ApplicantAccountSn { get; set; }
        public int AuditAccountSn { get; set; }

    }
}
