using MOD4.Web.DomainService;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Utility.Helper;

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
            var _userMenuInfo = CatchHelper.Get($"userMenuInfo");
            List<UserEntity> _currentUserInfo = new List<UserEntity>();

            if (_userMenuInfo != null)
                _currentUserInfo = JsonConvert.DeserializeObject<List<UserEntity>>(_userMenuInfo);

            if (_currentUserInfo.FirstOrDefault(f => f.sn == accountSn) == null)
            {
                var _userMenuList = _menuDomainService.GetAccountMenuInfo(accountSn);

                _currentUserInfo.Add(new UserEntity
                {
                    sn = accountSn,
                    UserMenuList = _userMenuList
                });

                CatchHelper.Delete(new string[] { "userMenuInfo" });
                CatchHelper.Set("userMenuInfo", JsonConvert.SerializeObject(_currentUserInfo.Select(s => new 
                {
                    sn = s.sn,
                    UserMenuList = s.UserMenuList
                })), 604800);
            }

            var noSub = _currentUserInfo.FirstOrDefault(f => f.sn == accountSn).UserMenuList
                .Where(w => w.ParentMenuSn == 0 && w.Href != "#").Select(se =>
                {
                    return new Menu
                    {
                        Href = se.Href,
                        PageName = se.PageName,
                        ClassName = se.ClassName,
                        HaveSub = false
                    };
                }).ToList();

            var subsMain = _currentUserInfo.FirstOrDefault(f => f.sn == accountSn).UserMenuList
                .Where(w => w.ParentMenuSn == 0 && w.Href == "#").Select(se =>
                {
                    return new Menu
                    {
                        Href = se.Href,
                        PageName = se.PageName,
                        ClassName = se.ClassName,
                        HaveSub = true,
                        SubMenuList = _currentUserInfo.FirstOrDefault(f => f.sn == accountSn).UserMenuList.Where(w => se.MenuSn == w.ParentMenuSn).Select(s =>
                        {
                            return new MenuDetail
                            {
                                Href = s.Href,
                                PageName = s.PageName,
                                ClassName = s.ClassName,
                            };
                        }).ToList()
                    };
                }).ToList();

            return noSub.Union(subsMain).ToList();
        }
    }
}
