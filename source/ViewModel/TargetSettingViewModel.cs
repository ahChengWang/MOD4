using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MOD4.Web.ViewModel
{
    public class TargetSettingViewModel
    {
        public List<TargetSettingDetailModel> SettingDetailList { get; set; }
    }


    public class TargetSettingDetailModel
    {
        public string Node { get; set; }

        [Display(Name = "07:30~08:30")]
        [Required(ErrorMessage = "*必填")]
        [RegularExpression(@"^[0-9]{1,3}?$", ErrorMessage = "請輸入正確數值")]
        public int Time0730 { get; set; }

        [Display(Name = "08:30~09:30")]
        [Required(ErrorMessage = "*必填")]
        [RegularExpression(@"^[0-9]{1}?$", ErrorMessage = "請輸入正確數值")]
        public int Time0830 { get; set; }

        [Display(Name = "09:30~10:30")]
        [Required(ErrorMessage = "*必填")]
        [RegularExpression(@"^[0-9]{1,3}?$", ErrorMessage = "請輸入正確數值")]
        public int Time0930 { get; set; }

        [Display(Name = "10:30~11:30")]
        [Required(ErrorMessage = "*必填")]
        [RegularExpression(@"^[0-9]{1,3}?$", ErrorMessage = "請輸入正確數值")]
        public int Time1030 { get; set; }

        [Display(Name = "11:30~12:30")]
        [Required(ErrorMessage = "*必填")]
        [RegularExpression(@"^[0-9]{1,3}?$", ErrorMessage = "請輸入正確數值")]
        public int Time1130 { get; set; }

        [Display(Name = "12:30~13:30")]
        [Required(ErrorMessage = "*必填")]
        [RegularExpression(@"^[0-9]{1,3}?$", ErrorMessage = "請輸入正確數值")]
        public int Time1230 { get; set; }

        [Display(Name = "13:30~14:30")]
        [Required(ErrorMessage = "*必填")]
        [RegularExpression(@"^[0-9]{1,3}?$", ErrorMessage = "請輸入正確數值")]
        public int Time1330 { get; set; }

        [Display(Name = "14:30~15:30")]
        [Required(ErrorMessage = "*必填")]
        [RegularExpression(@"^[0-9]{1,3}?$", ErrorMessage = "請輸入正確數值")]
        public int Time1430 { get; set; }

        [Display(Name = "15:30~16:30")]
        [Required(ErrorMessage = "*必填")]
        [RegularExpression(@"^[0-9]{1,3}?$", ErrorMessage = "請輸入正確數值")]
        public int Time1530 { get; set; }

        [Display(Name = "16:30~17:30")]
        [Required(ErrorMessage = "*必填")]
        [RegularExpression(@"^[0-9]{1,3}?$", ErrorMessage = "請輸入正確數值")]
        public int Time1630 { get; set; }

        [Display(Name = "17:30~18:30")]
        [Required(ErrorMessage = "*必填")]
        [RegularExpression(@"^[0-9]{1,3}?$", ErrorMessage = "請輸入正確數值")]
        public int Time1730 { get; set; }

        [Display(Name = "18:30~19:30")]
        [Required(ErrorMessage = "*必填")]
        [RegularExpression(@"^[0-9]{1,3}?$", ErrorMessage = "請輸入正確數值")]
        public int Time1830 { get; set; }

        [Display(Name = "19:30~20:30")]
        [Required(ErrorMessage = "*必填")]
        [RegularExpression(@"^[0-9]{1,3}?$", ErrorMessage = "請輸入正確數值")]
        public int Time1930 { get; set; }

        [Display(Name = "20:30~21:30")]
        [Required(ErrorMessage = "*必填")]
        [RegularExpression(@"^[0-9]{1,3}?$", ErrorMessage = "請輸入正確數值")]
        public int Time2030 { get; set; }

        [Display(Name = "21:30~22:30")]
        [Required(ErrorMessage = "*必填")]
        [RegularExpression(@"^[0-9]{1,3}?$", ErrorMessage = "請輸入正確數值")]
        public int Time2130 { get; set; }

        [Display(Name = "22:30~23:30")]
        [Required(ErrorMessage = "*必填")]
        [RegularExpression(@"^[0-9]{1,3}?$", ErrorMessage = "請輸入正確數值")]
        public int Time2230 { get; set; }

        [Display(Name = "23:30~00:30")]
        [Required(ErrorMessage = "*必填")]
        [RegularExpression(@"^[0-9]{1,3}?$", ErrorMessage = "請輸入正確數值")]
        public int Time2330 { get; set; }

        [Display(Name = "00:30~01:30")]
        [Required(ErrorMessage = "*必填")]
        [RegularExpression(@"^[0-9]{1,3}?$", ErrorMessage = "請輸入正確數值")]
        public int Time0030 { get; set; }

        [Display(Name = "01:30~02:30")]
        [Required(ErrorMessage = "*必填")]
        [RegularExpression(@"^[0-9]{1,3}?$", ErrorMessage = "請輸入正確數值")]
        public int Time0130 { get; set; }

        [Display(Name = "02:30~03:30")]
        [Required(ErrorMessage = "*必填")]
        [RegularExpression(@"^[0-9]{1,3}?$", ErrorMessage = "請輸入正確數值")]
        public int Time0230 { get; set; }

        [Display(Name = "03:30~04:30")]
        [Required(ErrorMessage = "*必填")]
        [RegularExpression(@"^[0-9]{1,3}?$", ErrorMessage = "請輸入正確數值")]
        public int Time0330 { get; set; }

        [Display(Name = "04:30~05:30")]
        [Required(ErrorMessage = "*必填")]
        [RegularExpression(@"^[0-9]{1,3}?$", ErrorMessage = "請輸入正確數值")]
        public int Time0430 { get; set; }

        [Display(Name = "05:30~06:30")]
        [Required(ErrorMessage = "*必填")]
        [RegularExpression(@"^[0-9]{1,3}?$", ErrorMessage = "請輸入正確數值")]
        public int Time0530 { get; set; }

        [Display(Name = "06:30~07:30")]
        [Required(ErrorMessage = "*必填")]
        [RegularExpression(@"^[0-9]{1,3}?$", ErrorMessage = "請輸入正確數值")]
        public int Time0630 { get; set; }
    }
}
