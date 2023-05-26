using MOD4.Web.Enum;

namespace MOD4.Web.DomainService.Entity
{
    public class MenuEntity
    {
        public int sn { get; set; }
        public string Account { get; set; }
        public string Name { get; set; }
        public int MenuSn { get; set; }
        public int ParentMenuSn { get; set; }
        public string ClassName { get; set; }
        public string PageName { get; set; }
        public string Href { get; set; }
    }
}
