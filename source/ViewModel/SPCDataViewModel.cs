using System;
using System.ComponentModel;

namespace MOD4.Web.ViewModel
{
    public class SPCDataViewModel
    {
        [DisplayName("Measure Date")]
        public string MeasureDate { get; set; }

        [DisplayName("Measure Time")]
        public string MeasureTime { get; set; }

        [DisplayName("SHT ID")]
        public string SHTId { get; set; }

        [DisplayName("PROD ID")]
        public string ProductId { get; set; }

        [DisplayName("Data Group")]
        public string DataGroup { get; set; }

        [DisplayName("DTX")]
        public string DTX { get; set; }

        [DisplayName("DTRM")]
        public string DTRM { get; set; }

        [DisplayName("USL")]
        public string USL { get; set; }

        [DisplayName("Target")]
        public string Target { get; set; }

        [DisplayName("LSL")]
        public string LSL { get; set; }

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

        [DisplayName("OOS")]
        public bool OOS { get; set; }

        [DisplayName("OOC1")]
        public bool OOC1 { get; set; }

        [DisplayName("OOC2")]
        public bool OOC2 { get; set; }

        [DisplayName("OOR1")]
        public string OOR1 { get; set; }

        [DisplayName("OOR2")]
        public string OOR2 { get; set; }

        [DisplayName("OOR3")]
        public string OOR3 { get; set; }
    }
}
