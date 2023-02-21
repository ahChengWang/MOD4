using System;

namespace MOD4.Web.DomainService.Entity
{
    public class SPCMicroScopeDataEntity
    {
        private string _measureDate = "";
        private string _measureTime = "";

        public string EquipmentId { get; set; }
        public DateTime MeasureDate { get; set; }
        public string MeasureDateStr
        {
            get
            {
                _measureDate = this.MeasureDate.ToString("yyyy/MM/dd HH:mm:ss").Substring(0, 10);
                return _measureDate;
            }
            set
            {
                value = _measureDate;
            }
        }
        public string MeasureTimeStr
        {
            get
            {
                _measureTime = this.MeasureDate.ToString("yyyy/MM/dd HH:mm:ss").Substring(11, 8);
                return _measureTime;
            }
            set
            {
                value = _measureTime;
            }
        }
        public string Result { get; set; }
        public string SHTId { get; set; }
        public string ProductId { get; set; }
        public string DataGroup { get; set; }
        public float DTX { get; set; }
        public float DTRM { get; set; }
        public float USL { get; set; }
        public float Target { get; set; }
        public float LSL { get; set; }
        public float UCL1 { get; set; }
        public float CL1 { get; set; }
        public float LCL1 { get; set; }
        public float UCL2 { get; set; }
        public float CL2 { get; set; }
        public float LCL2 { get; set; }
        public bool OOS { get; set; }
        public bool OOC1 { get; set; }
        public bool OOC2 { get; set; }
        public bool OOR1 { get; set; }
        public bool OOR2 { get; set; }
        public bool OOR3 { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
