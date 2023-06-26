using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MOD4.Web.DomainService;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MOD4.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountDomainService _accountDomainService;
        private readonly string _shaKey = string.Empty;
        private readonly bool _innxVerify = false;

        public AccountController(IAccountDomainService accountDomainService,
            IConfiguration connectionString)
        {
            _accountDomainService = accountDomainService;
            _shaKey = connectionString.GetSection("SHAKey").Value;
            _innxVerify = bool.Parse(connectionString.GetSection("VerifyInxSSO").Value);
        }

        public IActionResult Index()
        {
            ViewBag.Version = $"v{GetType().Assembly.GetName().Version.ToString()}";

            return View();
        }

        [HttpPost]
        public IActionResult Login([FromForm] LoginViewModel loginViewMode)
        {
            try
            {
                var _verifyResult = _accountDomainService.DoUserVerify(new LoginEntity
                {
                    Account = loginViewMode.Account,
                    Password = loginViewMode.Password,
                    Token = loginViewMode.Token
                }, _shaKey, _innxVerify);

                if (_verifyResult.Item1)
                {
                    var claims = new List<Claim>()
                    {
                        new Claim("sn", Convert.ToString(_verifyResult.Item2.sn)),
                        new Claim(ClaimTypes.Sid, _verifyResult.Item2.Account.ToLower()),
                        new Claim(ClaimTypes.Name, _verifyResult.Item2.Name),
                        new Claim("Psw", _verifyResult.Item2.Password),
                        new Claim(ClaimTypes.Role, Convert.ToString((int)_verifyResult.Item2.RoleId)),
                        new Claim("LevelId", Convert.ToString((int)_verifyResult.Item2.Level_id)),
                        new Claim("DeptSn", Convert.ToString((int)_verifyResult.Item2.DeptSn)),
                        new Claim("JobId", _verifyResult.Item2.JobId),
                        new Claim(ClaimTypes.Email, _verifyResult.Item2.Mail)
                    };

                    //Initialize a new instance of the ClaimsIdentity with the claims and authentication scheme    
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    //Initialize a new instance of the ClaimsPrincipal with ClaimsIdentity    
                    var principal = new ClaimsPrincipal(identity);
                    //SignInAsync is a Extension method for Sign in a principal for the specified scheme.    
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties()
                    {
                        ExpiresUtc = DateTime.UtcNow.AddDays(2),
                        IsPersistent = loginViewMode.RememberMe //IsPersistent = false：瀏覽器關閉立馬登出；IsPersistent = true 就變成常見的Remember Me功能
                    }).Wait();

                    //紀錄Session
                    //HttpContext.Session.Set("CurrentAccount", ByteConvertHelper.Object2Bytes(_result.sn));

                    return Json(new { IsSuccess = true, InnxSSO = _innxVerify, Data = _verifyResult.Item2 });
                }
                else
                    return Json(new { IsSuccess = false, InnxSSO = _innxVerify, Data = "帳密有誤" });

            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Data = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult GetToken([FromForm] LoginViewModel loginViewMode)
        {
            try
            {
                return Json(new
                {
                    ssoVerify = _innxVerify,
                    token = _accountDomainService.GetToken(loginViewMode.Account, loginViewMode.Password)
                });
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult VerifyToken(InxSSOViewModel inxSSOVM)
        {
            try
            {
                return Json(_accountDomainService.VerifyToken(new InxSSOEntity
                {
                    Token = inxSSOVM.Token,
                    IsCheckIP = false,
                    SysID = null,
                    RemoteIP = "",
                    SSOTicket4 = inxSSOVM.SSOTicket4
                }));
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
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

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="data">加密資料</param>
        /// <param name="key">8位字元的金鑰字串</param>
        /// <param name="iv">8位字元的初始化向量字串</param>
        /// <returns></returns>
        public static string DES_Encrypt(string data, string key, string iv)
        {
            byte[] byKey = ASCIIEncoding.ASCII.GetBytes(key);
            byte[] byIV = ASCIIEncoding.ASCII.GetBytes(iv);

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            int i = cryptoProvider.KeySize;
            MemoryStream ms = new MemoryStream();
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write);

            StreamWriter sw = new StreamWriter(cst);
            sw.Write(data);
            sw.Flush();
            cst.FlushFinalBlock();
            sw.Flush();
            return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="data">解密資料</param>
        /// <param name="key">8位字元的金鑰字串(需要和加密時相同)</param>
        /// <param name="iv">8位字元的初始化向量字串(需要和加密時相同)</param>
        /// <returns></returns>
        public static string DES_Decrypt(string data, string key, string iv)
        {
            byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(key);
            byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(iv);

            byte[] byEnc;
            try
            {
                byEnc = Convert.FromBase64String(data);
            }
            catch
            {
                return null;
            }

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream(byEnc);
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cst);
            return sr.ReadToEnd();
        }
    }
}
