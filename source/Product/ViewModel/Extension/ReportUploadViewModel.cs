using Microsoft.AspNetCore.Http;
using MOD4.Web.Enum;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MOD4.Web.ViewModel
{
    public class ReportUploadViewModel
    {
        [DisplayName("工號")]
        [Required(ErrorMessage = "必填")]
        public string JobId { get; set; }

        [DisplayName("區域")]
        [Required(ErrorMessage = "必填")]
        public ApplyAreaEnum ApplyAreaId { get; set; }

        [DisplayName("學習項目")]
        [Required(ErrorMessage = "必填")]
        public int ItemId { get; set; }

        [DisplayName("檔案")]
        //[Required(ErrorMessage = "必填")]
        public IFormFile File { get; set; }
    }
}
