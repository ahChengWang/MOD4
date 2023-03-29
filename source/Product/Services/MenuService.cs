using MOD4.Web.DomainService;
using MOD4.Web.Models;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web
{
    public class MenuService
    {
        private readonly IMenuDomainService _menuDomainService;

        public MenuService(IMenuDomainService menuDomainService)
        {
            _menuDomainService = menuDomainService;
        }

        public List<Menu> GetMenusByUser(int accountSn)
        {
            var accountMenu = _menuDomainService.GetAccountMenuInfo(accountSn);

            var noSub = accountMenu.Where(w => w.ParentMenuSn == 0 && w.Href !="#").Select(se => {
                return new Menu
                {
                    Href = se.Href,
                    PageName = se.PageName,
                    ClassName = se.ClassName,
                    HaveSub = false
                };
            }).ToList();

            var subsMain = accountMenu.Where(w => w.ParentMenuSn == 0 && w.Href == "#").Select(se => {
                return new Menu
                {
                    Href = se.Href,
                    PageName = se.PageName,
                    ClassName = se.ClassName,
                    HaveSub = true,
                    SubMenuList = accountMenu.Where(w => se.MenuSn == w.ParentMenuSn).Select(s => {
                        return new MenuDetail
                        {
                            Href = s.Href,
                            PageName= s.PageName,
                            ClassName = s.ClassName,
                        };
                    }).ToList()
                };
            }).ToList();

            return noSub.Union(subsMain).ToList();
        }
    }
}
