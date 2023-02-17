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
        public double DTX { get; set; }
        public double DTRM { get; set; }
        public double USL { get; set; }
        public double Target { get; set; }
        public double LSL { get; set; }
        public double UCL1 { get; set; }
        public double CL1 { get; set; }
        public double LCL1 { get; set; }
        public double UCL2 { get; set; }
        public double CL2 { get; set; }
        public double LCL2 { get; set; }
        public bool OOS { get; set; }
        public bool OOC1 { get; set; }
        public bool OOC2 { get; set; }
        public double OOR1 { get; set; }
        public double OOR2 { get; set; }
        public double OOR3 { get; set; }
        public DateTime CreateTime { get; set; }
    }
}