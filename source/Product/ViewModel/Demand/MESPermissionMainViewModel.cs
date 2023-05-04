using System.ComponentModel.DataAnnotations;

namespace MOD4.Web.ViewModel
{
    public class MESPermissionMainViewModel
    {
        public int OrderSn { get; set; }

        [Display(Name = "單號")]
        public string OrderNo { get; set; }

        [Display(Name = "申請人")]
        public string Applicant { get; set; }

        [Display(Name = "工號")]
        public string JobId { get; set; }

        [Display(Name = "部門")]
        public string Department { get; set; }

        [Display(Name = "類別")]
        public string MESOrderType { get; set; }

        [Display(Name = "狀態")]
        public string Status { get; set; }

        public int StatusId { get; set; }

        [Display(Name = "申請名單")]
        public string ApplicantList { get; set; }

        [Display(Name = "申請日")]
        public string CreateDate { get; set; }

        [Display(Name = "待簽人員")]
        public string AuditPerson { get; set; }

        public string Url { get; set; }
    }
}
