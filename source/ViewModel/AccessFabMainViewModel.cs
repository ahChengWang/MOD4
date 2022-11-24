using MOD4.Web.Enum;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MOD4.Web.ViewModel
{
    public class AccessFabMainViewModel
    {
        public int OrderSn { get; set; }

        [Display(Name = "單號")]
        public string OrderNo { get; set; }

        [Display(Name = "申請人")]
        public string Applicant { get; set; }

        [Display(Name = "開單日")]
        public string Date { get; set; }

        public FabInTypeEnum FabInTypeId { get; set; }

        [Display(Name = "入廠性質")]
        public string FabInType { get; set; }

        public FabInCategoryEnum FabInCategoryId { get; set; }
        [Display(Name = "入廠對象")]
        public string FabInCategory { get; set; }

        [Display(Name = "入廠目的")]
        public string Content { get; set; }

        [Display(Name = "待簽核人員")]
        public string AuditAccount { get; set; }

        public FabInOutStatusEnum StatusId { get; set; }

        [Display(Name = "簽核狀態")]
        public string Status { get; set; }


        public string Url { get; set; }
    }
}