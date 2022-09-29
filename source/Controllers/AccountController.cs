using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MOD4.Web.DomainService;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Helper;
using MOD4.Web.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MOD4.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountDomainService _accountDomainService;
        private readonly IMenuDomainService _menuDomainService;
        private readonly string _shaKey = string.Empty;

        public AccountController(IAccountDomainService accountDomainService,
            IMenuDomainService menuDomainService,
            IConfiguration connectionString)
        {
            _accountDomainService = accountDomainService;
            _menuDomainService = menuDomainService;
            _shaKey = connectionString.GetSection("SHAKey").Value;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login([FromForm] LoginViewModel loginViewMode)
        {
            try
            {
                // fab 無法 call InxSSO
                var _domainName = Environment.UserDomainName;

                if (_domainName == "CMINL")
                {
                    // call InxSSO 確認帳密
                    bool _verifyResult = _accountDomainService.VerifyInxSSO(loginViewMode.Account, loginViewMode.Password);

                    if (!_verifyResult)
                        return Json("帳號密碼錯誤");
                }

                var _encryptPw = Encrypt(loginViewMode.Password, _shaKey);

                _accountDomainService.InsertUpdateAccountInfo(loginViewMode.Account, _encryptPw);

                var _catchAccInfo = CatchHelper.Get($"accInfo");
                AccountInfoEntity _currentUser = new AccountInfoEntity();

                if (_catchAccInfo == null)
                {
                    var _allAccInfo = _accountDomainService.GetAllAccountInfo();
                    CatchHelper.Set("accInfo", JsonConvert.SerializeObject(_allAccInfo), 604800);
                    _currentUser = _allAccInfo.FirstOrDefault(f => f.Account == loginViewMode.Account && f.Password == _encryptPw);
                }
                else
                {
                    _currentUser = JsonConvert.DeserializeObject<List<AccountInfoEntity>>(_catchAccInfo)
                        .FirstOrDefault(f => f.Account == loginViewMode.Account && f.Password == _encryptPw);
                }

                //var _result = _accountDomainService.GetAccountInfo(loginViewMode.Account, _encryptPw);

                if (_currentUser != null)
                {
                    var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, Convert.ToString(_currentUser.Account)),
                    new Claim("sn", Convert.ToString(_currentUser.sn)),
                    new Claim("Account", _currentUser.Account),
                    new Claim("Name", _currentUser.Name),
                    new Claim("Role", Convert.ToString((int)_currentUser.RoleId))
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


        private string Encrypt(string password, string key)
        {
            // ComputeHash - returns byte array  
            byte[] bytesPW = Encoding.UTF8.GetBytes(password);
            byte[] bytesKey = Encoding.UTF8.GetBytes(key);
            // Hash the password with SHA256
            byte[] keyBytes = SHA256.Create().ComputeHash(bytesKey);
            return Convert.ToBase64String(AES_Encrypt(bytesPW, keyBytes));
        }

        private string Decrypt(string password, string key)
        {
            byte[] bytesToBeDecrypted = Convert.FromBase64String(password);
            byte[] passwordBytesdecrypt = Encoding.UTF8.GetBytes(key);
            passwordBytesdecrypt = SHA256.Create().ComputeHash(passwordBytesdecrypt);
            return Encoding.UTF8.GetString(AES_Decrypt(bytesToBeDecrypted, passwordBytesdecrypt));
        }

        private byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 2, 1, 7, 3, 6, 4, 8, 5 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        private byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 2, 1, 7, 3, 6, 4, 8, 5 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
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
