using System.ComponentModel;

namespace MOD4.Web.ViewModel
{
    public class SPCSettingViewModel
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

        [DisplayName("UCL2")]
        public string UCL2 { get; set; }

        [DisplayName("CL2")]
        public string CL2 { get; set; }

        [DisplayName("LCL2")]
        public string LCL2 { get; set; }

    }
}
