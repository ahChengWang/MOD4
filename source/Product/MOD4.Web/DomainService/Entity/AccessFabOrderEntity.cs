using MOD4.Web.Enum;
using System;
using System.Collections.Generic;

namespace MOD4.Web.DomainService.Entity
{
    public class AccessFabOrderEntity
    {
        public int OrderSn { get; set; }
        public string OrderNo { get; set; }
        public FabInTypeEnum FabInTypeId { get; set; }
        public string FabInType { get; set; }
        public string FabInOtherType { get; set; }
        public FabInCategoryEnum CategoryId { get; set; }
        public string Category { get; set; }
        public AuditStatusEnum StatusId { get; set; }
        public string Status { get; set; }
        public string Applicant { get; set; }
        public string FillOutPerson { get; set; }
        public string JobId { get; set; }
        public string ApplicantMVPN { get; set; }
        public DateTime FabInDate { get; set; }
        public string FabInDateStr { get; set; }
        public DateTime FabOutDate { get; set; }
        public string AccompanyingPerson { get; set; }
        public string AccompanyingPersonMVPN { get; set; }
        public string Content { get; set; }
        public string Route { get; set; }
        public int ApplicantAccountSn { get; set; }
        public string AuditAccountName { get; set; }
        public int AuditAccountSn { get; set; }

        public string GustNames { get; set; }
        public string CreateUser { get; set; }
        public int CreateAccountSn { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateTimeStr { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateTime { get; set; }
        public string Remark { get; set; }
        public string Url { get; set; }
        public List<AccessFabOrderDetailEntity> DetailList { get; set; }

        public List<AccessFabOrderAuditHistoryEntity> AuditFlow { get; set; }
    }
}
