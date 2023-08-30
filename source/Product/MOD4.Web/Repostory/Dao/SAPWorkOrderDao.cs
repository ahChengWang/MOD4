using System;

namespace MOD4.Web.Repostory.Dao
{
    public class SAPWorkOrderDao
    {
        public int Sn { get; set; }
        public string Order { get; set; }
        public string Prod { get; set; }
        public string MaterialSpec { get; set; }
        public string MaterialNo { get; set; }
        public string MaterialName { get; set; }
        public string SAPNode { get; set; }
        public string Dept { get; set; }
        public int ProdQty { get; set; }
        public int StorageQty { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public decimal Unit { get; set; }
        public decimal ExptQty { get; set; }
        public decimal DisburseQty { get; set; }
        public decimal ReturnQty { get; set; }
        public decimal ActStorageQty { get; set; }
        public decimal ScrapQty { get; set; }
        public decimal DiffQty { get; set; }
        public decimal DiffRate { get; set; }
        public decimal OverDisburse { get; set; }
        public decimal DiffDisburse { get; set; }
        public decimal WOPremiumOut { get; set; }
        public decimal CantNegative { get; set; }
        public string OPIwoStatus { get; set; }
        public string WOType { get; set; }
    }
}
