namespace MOD4.Web.DomainService.Entity
{
    public class MonitorProdPerInfoEntity : BaseMonitorEqAreaSettingEntity
    {
        // prod info

        public string Area { get; set; }

        public string ProdNoConcate { get; set; }

        public string PassQty { get; set; }

        // alarm info

        public string StatusCode { get; set; }

        public string ProdNo { get; set; }

        public string Comment { get; set; }

        public bool IsFrontEnd { get; set; }

        public string StartTime { get; set; }
    }
}
