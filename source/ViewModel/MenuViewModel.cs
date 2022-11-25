using MOD4.Web.Enum;
using System.Collections.Generic;

namespace MOD4.Web.ViewModel
{
    public class PermissionViewModel
    {
        public MenuEnum MenuSn { get; set; }
        public string Menu { get; set; }
        public List<PermissionEnum> PermissionList { get; set; }
    }
}
