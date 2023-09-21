using System;

namespace MOD4.Web.ViewModel
{
    public class SAPWorkOrderViewModel
    {
        public string Order { get; set; }
        public string Prod { get; set; }
        public string MaterialNo { get; set; }
        public string SAPNode { get; set; }
        public int ProdQty { get; set; }
        public int StorageQty { get; set; }
        public string StartDate { get; set; }
        public string FinishDate { get; set; }
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
        public string MatlShortName { get; set; }
        public string OPIwoStatus { get; set; }
        public string WOType { get; set; }
        public string UseNode { get; set; }
        public string WOComment { get; set; }
        public int MESScrap { get; set; }
        public int ICScrap { get; set; }
    }
}
