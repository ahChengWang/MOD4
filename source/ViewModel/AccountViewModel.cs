using System.ComponentModel.DataAnnotations;

namespace MOD4.Web.ViewModel
{
    public class AccountViewModel
    {
        public int Sn { get; set; }

        [Display(Name = "帳號")]
        public string Account { get; set; }

        [Display(Name = "姓名")]
        public string Name { get; set; }

        [Display(Name = "職稱")]
        public string LevelId { get; set; }

        [Display(Name = "工號")]
        public string JobId { get; set; }

        [Display(Name = "部門")]
        public string Department { get; set; }
    }
}