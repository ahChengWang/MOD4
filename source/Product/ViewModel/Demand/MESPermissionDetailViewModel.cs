using MOD4.Web.Enum;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MOD4.Web.ViewModel
{
    public class MESPermissionDetailViewModel
    {
        public int OrderSn { get; set; }

        [Display(Name = "訂單編號")]
        public string OrderNo { get; set; }

        [Display(Name = "狀態")]
        public string Status { get; set; }
        public DemandStatusEnum StatusId { get; set; }

        [Display(Name = "部門")]
        public string Department { get; set; }

        [Display(Name = "單位")]
        public string SubUnit { get; set; }

        [Display(Name = "類別")]
        public string MESOrderType { get; set; }

        [Display(Name = "申請人")]
        public string Applicant { get; set; }

        [Display(Name = "工號")]
        public string JobId { get; set; }

        [Display(Name = "電話")]
        public string Phone { get; set; }

        [Display(Name = "申請原因")]
        public string ApplicantReason { get; set; }

        [Display(Name = "申請名單")]
        public List<MESApplicantModel> ApplicantList { get; set; }

        public List<MESPermissionModel> PermissionList { get; set; }

        [Display(Name = "其他權限")]
        public string OtherPermission { get; set; }

        [Display(Name = "權限同姓名")]
        public string SameEmpName { get; set; }

        [Display(Name = "權限同工號")]
        public string SameEmpJobId { get; set; }

        [Display(Name = "申請日")]
        public string CreateDate { get; set; }

        [Display(Name = "備註")]
        public string Remark { get; set; }

        public List<MESPermissionAuditHistoryModel> AuditHistory { get; set; }
    }
}