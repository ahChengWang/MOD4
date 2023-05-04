using MOD4.Web.Enum;
using System;

namespace MOD4.Web.Repostory.Dao
{
    public class MESPermissionDao
    {
        public int orderSn { get; set; }
        public string orderNo { get; set; }
        public DemandStatusEnum statusId { get; set; }
        public string department { get; set; }
        public string subUnit { get; set; }
        public string applicant { get; set; }
        public string jobId { get; set; }
        public string phone { get; set; }
        public string samePermName { get; set; }
        public string samePermJobId { get; set; }
        public string permissionList { get; set; }
        public string otherPermission { get; set; }
        public int auditAccountSn { get; set; }
        public int applicantAccountSn { get; set; }
        public string createUser { get; set; }
        public DateTime createTime { get; set; }
        public string updateUser { get; set; }
        public DateTime updateTime { get; set; }
        public bool isCancel { get; set; }
        public MESOrderTypeEnum mesOrderTypeId { get; set; }
        public string applicantReason { get; set; }
    }
}
