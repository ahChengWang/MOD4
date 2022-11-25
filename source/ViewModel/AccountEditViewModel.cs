using MOD4.Web.Enum;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MOD4.Web.ViewModel
{
    public class AccountEditViewModel
    {
        public int Sn { get; set; }

        [Display(Name = "帳號")]
        [Required(ErrorMessage = "必填")]
        public string Account { get; set; }

        [Display(Name = "姓名")]
        [Required(ErrorMessage = "必填")]
        public string Name { get; set; }

        [Display(Name = "職稱")]
        [Required(ErrorMessage = "必填")]
        public JobLevelEnum LevelId { get; set; }

        public string Level { get; set; }

        [Display(Name = "工號")]
        [Required(ErrorMessage = "必填")]
        public string JobId { get; set; }

        [Display(Name = "API key")]
        public string ApiKey { get; set; }

        [Display(Name = "廠別")]
        [Required(ErrorMessage = "必填")]
        public int MODId { get; set; }

        [Display(Name = "部級")]
        public int SectionId { get; set; }

        [Display(Name = "課級")]
        public int DepartmentId { get; set; }

        [Display(Name = "mail")]
        [Required(ErrorMessage = "必填")]
        public string Mail { get; set; }

        public List<PermissionViewModel> MenuPermissionList { get; set; }
    }
}