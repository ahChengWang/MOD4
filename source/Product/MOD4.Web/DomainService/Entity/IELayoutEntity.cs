using MOD4.Web.Enum;
using System;

namespace MOD4.Web.DomainService.Entity
{
    public class IELayoutEntity
    {
        public int OrderSn { get; set; }
        public string OrderNo { get; set; }
        public string ApplicantName { get; set; }
        public int ApplicantAccountSn { get; set; }
        public string Department { get; set; }
        public string Phone { get; set; }
        public DateTime ApplyDate { get; set; }
        public string ApplyDateStr { get; set; }
        public AuditStatusEnum StatusId { get; set; }
        public string Status { get; set; }
        //FactoryFloorEnum
        public int FactoryFloor { get; set; }
        //ProcessAreaEnum
        public int ProcessArea { get; set; }
        public string PartRemark { get; set; }
        //FormatTypeEnum
        public int FormatType { get; set; }
        public ReasonTypeEnum ReasonTypeId { get; set; }
        public string Reason { get; set; }
        public LayerTypeEnum LayerTypeId { get; set; }
        public string IssueRemark { get; set; }
        public int AuditAccountSn { get; set; }
        public string AuditName { get; set; }
        public SecretLevelEnum SecretLevelId { get; set; }
        public DateTime? ExptOutputDate { get; set; }
        public string ExptOutputDateStr { get; set; }        
        public string Version { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateTimeStr { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateTime { get; set; }
        public bool IsIEFlow { get; set; }
    }
}
