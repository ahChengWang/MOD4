namespace MOD4.Web.DomainService.Entity
{
    public class OptionEntity
    {
        public int Id { get; set; }

        public int SubId { get; set; }

        public string Value { get; set; }

        public string SubValue { get; set; }

        // for checkbox
        public bool Checked { get; set; } = false;
    }
}
