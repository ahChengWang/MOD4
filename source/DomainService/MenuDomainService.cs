using MOD4.Web.DomainService.Entity;
using MOD4.Web.Repostory;
using SpareManagement.Helper;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.DomainService
{
    public class MenuDomainService : IMenuDomainService
    {
        private readonly IMenuRepository _menuRepository;

        public MenuDomainService(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }


        public List<MenuEntity> GetAccountMenuInfo(int sn)
        {
            var _menuList = _menuRepository.SelectByConditions(sn).Select(s => {
                return new MenuEntity {
                    sn = s.sn,
                    MenuSn = s.menu_sn,
                    ParentMenuSn = s.parent_menu_sn,
                    PageName = s.page_name,
                    ClassName = s.class_name,
                    Href = s.href,
                };
            }).ToList();

            return _menuList;
        }
    }
}
