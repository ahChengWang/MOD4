using System;

namespace MOD4.Web.DomainService.Entity
{
    public class SAPWorkOrderEntity
    {
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
        public int Unit { get; set; }
        public int ExptQty { get; set; }
        public int DisburseQty { get; set; }
        public int ReturnQty { get; set; }
        public int ActStorageQty { get; set; }
        public int ScrapQty { get; set; }
        public int DiffQty { get; set; }
        public decimal DiffRate { get; set; }
    }
}
