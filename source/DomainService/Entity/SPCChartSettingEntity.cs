namespace MOD4.Web.DomainService.Entity
{
    public class SPCChartSettingEntity
    {
        public int sn { get; set; }
        public string ONCHID { get; set; }
        public string PECD { get; set; }
        public string ONCHTYPE { get; set; }
        public string DataGroup { get; set; }
        public double Target { get; set; }
        public double USPEC { get; set; }
        public double LSPEC { get; set; }
        public double LastMonCPK { get; set; }
        public double Last2MonCPK { get; set; }
        public double Last3MonCPK { get; set; }
        public double LastMonUCL1 { get; set; }
        public double LastMonCL1 { get; set; }
        public double LastMonLCL1 { get; set; }
        public double LastMonUCL2 { get; set; }
        public double LastMonCL2 { get; set; }
        public double LastMonLCL2 { get; set; }
        public double UCL1 { get; set; }
        public double CL1 { get; set; }
        public double LCL1 { get; set; }
        public double UCL2 { get; set; }
        public double CL2 { get; set; }
        public double LCL2 { get; set; }
        public double NewUCL1 { get; set; }
        public double NewCL1 { get; set; }
        public double NewLCL1 { get; set; }
        public double NewUCL2 { get; set; }
        public double NewCL2 { get; set; }
        public double NewLCL2 { get; set; }
        public string PEQPT_ID { get; set; }
        public string PROC_ID { get; set; }
        public string CHARTGRADE { get; set; }
        public int FLOOR { get; set; }
        public string Memo { get; set; }
    }
}
