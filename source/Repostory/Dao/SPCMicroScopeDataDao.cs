using System;

namespace MOD4.Web.Repostory.Dao
{
    public class SPCMicroScopeDataDao
    {
        public string EquipmentId { get; set; }
        public DateTime MeasureDate { get; set; }
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