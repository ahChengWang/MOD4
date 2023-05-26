using Microsoft.AspNetCore.Http;
using MOD4.Web.Enum;
using System;
using System.Collections.Generic;

namespace MOD4.Web.DomainService.Entity
{
    public class MESPermissionEntity
    {
        public int OrderSn { get; set; }
        public string OrderNo { get; set; }
        public string Status { get; set; }
        public DemandStatusEnum StatusId { get; set; }
        public string Department { get; set; }
        public string SubUnit { get; set; }
        public MESOrderTypeEnum MESOrderTypeId { get; set; }
        public string MESOrderType { get; set; }
        public string Applicant { get; set; }
        public string JobId { get; set; }
        public string Phone { get; set; }
        public string ApplicantName { get; set; }
        public string ApplicantJobId { get; set; }
        public int AuditAccountSn { get; set; }
        public string SamePermName { get; set; }
        public string SamePermJobId { get; set; }
        public string PermissionList { get; set; }
        public string OtherPermission { get; set; }
        public string ApplicantReason { get; set; }
        public string AuditName { get; set; }
        public int ApplicantAccountSn { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateTimeStr { get; set; }
        public bool IsCancel { get; set; }
        public string Url { get; set; }
        public IFormFile UploadFile { get; set; }
        public string UploadFileName { get; set; }
        public List<MESApplicantEntity> Applicants { get; set; }
        public List<MESPermissionDetailEntity> PermissionInfo { get; set; }
        public List<MESPermissionAuditHistoryEntity> MESOrderAuditHistory { get; set; }
    }
}
