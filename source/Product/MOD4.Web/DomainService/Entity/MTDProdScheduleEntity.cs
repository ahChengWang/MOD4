using MOD4.Web.Enum;
using System;

namespace MOD4.Web.DomainService.Entity
{
    public class MTDProdScheduleEntity
    {
        public int Sn { get; set; }
        public string Process { get; set; }
        public string Model { get; set; }
        public int PassNode { get; set; }
        public int WipNode { get; set; }
        public int WipNode2 { get; set; }
        public string EqNo { get; set; }
        public int LcmProdSn { get; set; }
        public string ProdId { get; set; }
        public DateTime Date { get; set; }
        public int Value { get; set; }
        public int Floor { get; set; }
        public int OwnerId { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateTime { get; set; }
        public int MonthPlan { get; set; }
    }
}
