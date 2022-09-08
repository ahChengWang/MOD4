using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MOD4.Web.DomainService;
using MOD4.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace MOD4.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountDomainService _accountDomainService;
        private readonly IMenuDomainService _menuDomainService;

        public AccountController(IAccountDomainService accountDomainService, IMenuDomainService menuDomainService)
        {
            _accountDomainService = accountDomainService;
            _menuDomainService = menuDomainService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login([FromForm] LoginViewModel loginViewMode)
        {
            var _result = _accountDomainService.GetAccountInfo(loginViewMode.Account, loginViewMode.Password);

            if (_result != null)
            {
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, Convert.ToString(_result.Account)),
                    new Claim("Name", _result.Name),
                    new Claim("Account", _result.Account),
                    new Claim("sn", Convert.ToString(_result.sn, 16)),
                };
                //Initialize a new instance of the ClaimsIdentity with the claims and authentication scheme    
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                //Initialize a new instance of the ClaimsPrincipal with ClaimsIdentity    
                var principal = new ClaimsPrincipal(identity);
                //SignInAsync is a Extension method for Sign in a principal for the specified scheme.    
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties()
                {
                    IsPersistent = loginViewMode.RememberMe //IsPersistent = false：瀏覽器關閉立馬登出；IsPersistent = true 就變成常見的Remember Me功能
                }).Wait();

                //紀錄Session
                //HttpContext.Session.Set("CurrentAccount", ByteConvertHelper.Object2Bytes(_result.sn));

                return Json("");
            }

            return Json("帳號密碼錯誤");
        }


        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        //登出 Action 記得別加上[Authorize]，不管用戶是否登入，都可以執行Logout
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();

            return RedirectToAction("Index", "Account");//導至登入頁
        }

        public IActionResult MenuList()
        {
            HttpContext.SignOutAsync();

            return RedirectToAction("Index", "Account");//導至登入頁
        }

    }
}
