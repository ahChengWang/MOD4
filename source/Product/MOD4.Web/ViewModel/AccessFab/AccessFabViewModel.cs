using MOD4.Web.Enum;
using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MOD4.Web.ViewModel
{
    public class AccessFabViewModel
    {
        public int OrderSn { get; set; }
        [Display(Name = "單號")]
        public string OrderNo { get; set; }

        public FabInTypeEnum FabInTypeId { get; set; }
        public string FabInType { get; set; }
        public AuditStatusEnum StatusId { get; set; }
        public string Status { get; set; }
        [Display(Name = "申請人")]
        public string Applicant { get; set; }
        [Display(Name = "工號")]
        public string JobId { get; set; }
        [Display(Name = "申請人連絡電話")]
        public string ApplicantMVPN { get; set; }
        public DateTime FabInDate { get; set; }
        public DateTime FabOutDate { get; set; }
        public string AccompanyJobId { get; set; }
        public string Content { get; set; }
        public FabInOutFlowEnum CurrentFlowId { get; set; }
        public string CurrentFlow { get; set; }
        public int AuditAccountSn { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateTime { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateTime { get; set; }
        public string Remark { get; set; }
    }
}