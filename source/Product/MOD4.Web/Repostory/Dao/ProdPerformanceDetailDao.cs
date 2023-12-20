using System;

namespace MOD4.Web.Repostory.Dao
{
    public class ProdPerformanceDetailDao
    {
        public string ProdNo { get; set; }
        public string EntityNo { get; set; }
        public DateTime TransDate { get; set; }
        public string TransType { get; set; }
        public string EquipNo { get; set; }
        public string Operator { get; set; }
        public int WorkCtr { get; set; }
        public string OutputFG { get; set; }
        public int ToWC { get; set; }
        public string DefectCode { get; set; }
        public string Comment { get; set; }
        public int RwTimes { get; set; }
        public int DefcTimes { get; set; }
        public int TTLCount { get; set; }
        public decimal TackTime { get; set; }

        // 暫時 for monitor TT
        public int Sn { get; set;}
    }
}
