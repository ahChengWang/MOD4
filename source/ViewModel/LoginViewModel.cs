using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MOD4.Web.ViewModel
{
    public class LoginViewModel
    {
        [DisplayName("帳號")]
        [Required(ErrorMessage = "*請輸入帳號")]
        public string Account { get; set; }

        [DisplayName("密碼")]
        [Required(ErrorMessage = "*請輸入密碼")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayName("記住我")]
        public bool RememberMe { get; set; }
    }
}
