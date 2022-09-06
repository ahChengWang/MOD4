using MOD4.Web.DomainService.Entity;
using System.Collections.Generic;

namespace MOD4.Web.DomainService
{
    public interface IMenuDomainService
    {
        List<MenuEntity> GetAccountMenuInfo(int sn);
    }
}
