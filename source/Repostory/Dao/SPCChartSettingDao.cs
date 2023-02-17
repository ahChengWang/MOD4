using System;

namespace MOD4.Web.Repostory.Dao
{
    public class SPCChartSettingDao
    {
        public string ONCHID { get; set; }
        public string PECD { get; set; }
        public string ONCHTYPE { get; set; }
        public string DataGroup { get; set; }
        public double Target { get; set; }
        public double USPEC { get; set; }
        public double LSPEC { get; set; }
        public double UCL1 { get; set; }
        public double LCL1 { get; set; }
        public double CL1 { get; set; }
        public double UCL2 { get; set; }
        public double LCL2 { get; set; }
        public double CL2 { get; set; }
        public double PEQPT_ID { get; set; }
        public double PROC_ID { get; set; }
        public string CHARTGRADE { get; set; }
        public int FLOOR { get; set; }
    }
}