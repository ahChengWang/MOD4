using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MOD4.Web.ViewModel
{
    public class SPCSettingEditViewModel
    {
        public int Sn { get; set; }

        [DisplayName("PROD ID")]
        public string ProductId { get; set; }

        [DisplayName("ONCHTYPE")]
        public string OnchType { get; set; }

        [DisplayName("測項")]
        public string DataGroup { get; set; }

        [DisplayName("站點")]
        public string Node { get; set; }

        [DisplayName("Chartgrade")]
        public string Chartgrade { get; set; }

        [DisplayName("USPEC")]
        public string USPEC { get; set; }

        [DisplayName("LSPEC")]
        public string LSPEC { get; set; }

        [DisplayName("UCL1")]
        public string UCL1 { get; set; }

        [DisplayName("CL1")]
        public string CL1 { get; set; }

        [DisplayName("LCL1")]
        public string LCL1 { get; set; }

        public string Last2MonUCL1 { get; set; }

        public string Last2MonCL1 { get; set; }

        public string Last2MonLCL1 { get; set; }

        public string LastMonUCL1 { get; set; }

        public string LastMonCL1 { get; set; }

        public string LastMonLCL1 { get; set; }

        public string CurrMonUCL1 { get; set; }

        public string CurrMonCL1 { get; set; }

        public string CurrMonLCL1 { get; set; }

        [Required(ErrorMessage = "*必填")]
        public string NewUCL1 { get; set; }

        [Required(ErrorMessage = "*必填")]
        public string NewCL1 { get; set; }

        [Required(ErrorMessage = "*必填")]
        public string NewLCL1 { get; set; }

        [DisplayName("UCL2")]
        public string UCL2 { get; set; }

        [DisplayName("CL2")]
        public string CL2 { get; set; }

        [DisplayName("LCL2")]
        public string LCL2 { get; set; }

        public string Last2MonUCL2 { get; set; }

        public string Last2MonCL2 { get; set; }

        public string Last2MonLCL2 { get; set; }

        public string LastMonUCL2 { get; set; }

        public string LastMonCL2 { get; set; }

        public string LastMonLCL2 { get; set; }

        public string CurrMonUCL2 { get; set; }

        public string CurrMonCL2 { get; set; }

        public string CurrMonLCL2 { get; set; }

        [Required(ErrorMessage = "*必填")]
        public string NewUCL2 { get; set; }

        [Required(ErrorMessage = "*必填")]
        public string NewCL2 { get; set; }

        [Required(ErrorMessage = "*必填")]
        public string NewLCL2 { get; set; }

        [DisplayName("備註")]
        [Required(ErrorMessage = "*必填")]
        public string Memo { get; set; }

    }
}
