using MOD4.Web.Enum;
using System.Collections.Generic;

namespace MOD4.Web.DomainService.Entity
{
    public class MenuPermissionEntity
    {
        public bool IsMenuActive { get; set; }
        public MenuEnum MenuId { get; set; }
        public string Menu { get; set; }
        public List<MenuActionEntity> ActionList { get; set; }
    }
}
