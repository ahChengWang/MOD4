using System;

namespace MOD4.Web.Repostory.Dao
{
    public class MESPermissionApplicantsDao
    {
        public int mesPermissionSn { get; set; }
        public string applicantName { get; set; }
        public string applicantJobId { get; set; }
        public string createUser { get; set; }
        public DateTime createTime { get; set; }
    }
}
