using Microsoft.AspNetCore.Http;
using MOD4.Web.Enum;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MOD4.Web.ViewModel
{
    public class DemanCreateViewModel
    {
        public string OrderId { get; set; }

        [Display(Name = "系統類型")]
        [Required(ErrorMessage = "*必填")]
        public DemandCategoryEnum DemandCategoryId { get; set; }

        public DemandStatusEnum DemandStatusId { get; set; }

        [Display(Name = "主旨")]
        [Required(ErrorMessage = "*必填")]
        public string Subject { get; set; }

        [Display(Name = "需求內容")]
        [Required(ErrorMessage = "*必填")]
        public string Content { get; set; }

        [Display(Name = "申請人姓名")]
        [RegularExpression(@"^[\u4e00-\u9fa5a-zA-Z]+$", ErrorMessage = "*輸入中英文")]
        [Required(ErrorMessage = "*必填")]
        public string Applicant { get; set; }

        [Display(Name = "申請人工號")]
        [RegularExpression(@"^[0-9]{8}?$", ErrorMessage = "*輸入8位數字")]
        [Required(ErrorMessage = "*必填")]
        public string JobNo { get; set; }

        [Display(Name = "上傳檔案")]
        [Required(ErrorMessage = "*必填")]
        public List<IFormFile> UploadFile { get; set; }

        //public IFormFile UploadFile2 { get; set; }

        //public IFormFile UploadFile3 { get; set; }
    }
}