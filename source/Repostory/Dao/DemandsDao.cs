using MOD4.Web.Enum;
using System;

namespace MOD4.Web.Repostory.Dao
{
    public class DemandsDao
    {
        public int orderSn { get; set; }
        public string orderNo { get; set; }
        public DemandCategoryEnum categoryId { get; set; }
        public DemandStatusEnum statusId { get; set; }
        public string subject { get; set; }
        public string content { get; set; }
        public string applicant { get; set; }
        public string jobNo { get; set; }
        public string uploadFiles { get; set; }
        public string createUser { get; set; }
        public DateTime createTime { get; set; }
        public string updateUser { get; set; }
        public DateTime updateTime { get; set; }
        public string rejectReason { get; set; }
        public string completeFiles { get; set; }
        public string remark { get; set; }
    }
}
