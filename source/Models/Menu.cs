using System.Collections.Generic;

namespace MOD4.Web.Models
{
    public class Menu
    {
        public string ClassName { get; set; }
        public string PageName { get; set; }
        public string Href { get; set; }
        public bool HaveSub { get; set; }
        public List<MenuDetail> SubMenuList { get; set; }
    }

    public class MenuDetail
    {
        public string ClassName { get; set; }
        public string PageName { get; set; }
        public string Href { get; set; }
    }
}
