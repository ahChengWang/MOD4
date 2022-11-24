using System.ComponentModel.DataAnnotations;

namespace MOD4.Web.ViewModel
{
    public class AccessFabPersonsViewModel
    {
        public int DetailSn { get; set; }

        [Display(Name = "單位")]
        [Required(ErrorMessage = "必填")]
        public string CompanyName { get; set; }

        [Display(Name = "聯絡電話")]
        [Required(ErrorMessage = "必填")]
        public string GuestPhone { get; set; }

        [Display(Name = "姓名")]
        [Required(ErrorMessage = "必填")]
        public string Name { get; set; }
    }
}