using MOD4.Web.Enum;
using System.Collections.Generic;

namespace MOD4.Web.ViewModel
{
    public class MenuPermissionViewModel
    {
        public bool IsMenuActive { get; set; }
        public MenuEnum MenuId { get; set; }
        public string Menu { get; set; }
        public List<MenuActionViewModel> MenuActionList { get; set; }
    }
}
