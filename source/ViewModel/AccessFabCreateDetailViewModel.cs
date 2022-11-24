using System.ComponentModel.DataAnnotations;

namespace MOD4.Web.ViewModel
{
    public class AccessFabCreateDetailViewModel
    {
        [Display(Name = "單位")]
        [Required(ErrorMessage = "必填")]
        public string CompanyName { get; set; }

        [Display(Name = "聯絡電話")]
        [Required(ErrorMessage = "必填")]
        public string GuestPhone { get; set; }

        [Display(Name = "姓名")]
        [RegularExpression(@"^[\u4e00-\u9fa5a-zA-Z]+$", ErrorMessage = "*輸入中英文")]
        [Required(ErrorMessage = "必填")]
        public string Name { get; set; }
    }
}