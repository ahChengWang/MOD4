using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MOD4.Web.Models;
using MOD4.Web.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web
{
    [ViewComponent(Name = "Navigation")]
    public class NavigationViewComponent : ViewComponent
    {
        private readonly MenuService _menuAppService;
        public NavigationViewComponent(MenuService menuAppService)
        {
            _menuAppService = menuAppService;
        }

        public IViewComponentResult Invoke()
        {
            var _accouunt = Convert.ToInt16(HttpContext.User.Claims.FirstOrDefault(m => m.Type == "sn")?.Value);

            if (_accouunt == null)
            {
                return View(new List<Menu>());
            }

            //int accountSn = ByteConvertHelper.Bytes2Object<int>(HttpContext.Session.Get("CurrentAccount"));
            var menus = _menuAppService.GetMenusByUser(_accouunt);
            return View(menus);
        }
    }
}
