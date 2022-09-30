using Microsoft.AspNetCore.Http;
using MOD4.Web.Enum;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MOD4.Web.ViewModel
{
    public class DemanMainViewModel
    {
        public int OrderSn { get; set; }

        public string OrderId { get; set; }

        [Display(Name = "系統類型")]
        public string DemandCategory { get; set; }

        public DemandCategoryEnum DemandCategoryId { get; set; }

        [Display(Name = "狀態")]
        public string DemandStatus { get; set; }

        public DemandStatusEnum DemandStatusId { get; set; }

        [Display(Name = "主旨")]
        public string Subject { get; set; }

        [Display(Name = "需求內容")]
        public string Content { get; set; }

        [Display(Name = "申請人")]
        public string Applicant { get; set; }

        [Display(Name = "申請人工號")]
        public string JobNo { get; set; }

        [Display(Name = "申請日")]
        public string CreateDate { get; set; }

        [Display(Name = "上傳檔案")]
        public List<IFormFile> UploadFile { get; set; }

        public bool UserEditable { get; set; }

        public RoleEnum RoleId { get; set; }
    }
}
