namespace MOD4.Web.DomainService.Entity
{
    public class WorkOrderEntity
    {
        public string WorkOrder { get; set; }
        public string WOStatus { get; set; }
        public string LcmProduct { get; set; }
        public string WOType { get; set; }
        public int ActualQty { get; set; }
        public string WOComment { get; set; }
        public int Scrap { get; set; }
    }
}
