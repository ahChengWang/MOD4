using System;

namespace MOD4.Web.Repostory.Dao
{
    public class SPCChartSettingDao
    {
        public int sn { get; set; }
        public string ONCHID { get; set; }
        public string PECD { get; set; }
        public string ONCHTYPE { get; set; }
        public string DataGroup { get; set; }
        public float Target { get; set; }
        public float USPEC { get; set; }
        public float LSPEC { get; set; }
        public float UCL1 { get; set; }
        public float LCL1 { get; set; }
        public float CL1 { get; set; }
        public float UCL2 { get; set; }
        public float LCL2 { get; set; }
        public float CL2 { get; set; }
        public string PEQPT_ID { get; set; }
        public string PROC_ID { get; set; }
        public string CHARTGRADE { get; set; }
        public int FLOOR { get; set; }
        public string Memo { get; set; }
    }
}