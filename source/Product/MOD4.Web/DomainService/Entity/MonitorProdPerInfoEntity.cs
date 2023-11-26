namespace MOD4.Web.DomainService.Entity
{
    public class MonitorProdPerInfoEntity
    {
        // equipment area

        public int Node { get; set; }
        public string EqNumber { get; set; }
        public decimal DefTopRate { get; set; }
        public decimal DefLeftRate { get; set; }
        public decimal DefWidth { get; set; }
        public decimal DefHeight { get; set; }
        public string Border { get; set; }
        public string Background { get; set; }

        // prod info

        public string Area { get; set; }

        public string ProdNo { get; set; }

        public string PassQty { get; set; }

        // alarm info

        public string StatusCode { get; set; }

        public string Comment { get; set; }

        public bool IsFrontEnd { get; set; }

        public string StartTime { get; set; }
    }
}
