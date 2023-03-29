using MOD4.Web.Enum;
using System.Collections.Generic;

namespace MOD4.Web.ViewModel
{
    public class MenuActionViewModel
    {
        public bool IsActionActive { get; set; }
        public PermissionEnum ActionId { get; set; }
        public string Action { get; set; }
    }
}
