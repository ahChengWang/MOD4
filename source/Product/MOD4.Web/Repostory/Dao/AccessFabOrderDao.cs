using MOD4.Web.Enum;
using System;

namespace MOD4.Web.Repostory.Dao
{
    public class AccessFabOrderDao
    {
        public int OrderSn { get; set; }
        public string OrderNo { get; set; }
        public FabInTypeEnum FabInTypeId { get; set; }
        public string FabInOtherType { get; set; }
        public FabInCategoryEnum CategoryId { get; set; }
        public AuditStatusEnum StatusId { get; set; }
        public string Applicant { get; set; }
        public string JobId { get; set; }
        public string ApplicantMVPN { get; set; }
        public DateTime FabInDate { get; set; }
        public DateTime FabOutDate { get; set; }
        public string AccompanyJobId { get; set; }
        public string Content { get; set; }
        public string Route { get; set; }
        public string AccompanyingPerson { get; set; }
        public int AuditAccountSn { get; set; }
        public int ApplicantAccountSn { get; set; }
        public string UploadFileName { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateTime { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateTime { get; set; }
        public string Remark { get; set; }
        public int CreateAccountSn { get; set; }
        public string AccompanyingPersonMVPN { get; set; }
    }
}
