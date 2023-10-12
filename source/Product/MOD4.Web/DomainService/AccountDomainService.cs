using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using Utility.Helper;
using MOD4.Web.Repostory;
using MOD4.Web.Repostory.Dao;
using MOD4.Web.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;
using System.Web;
using System.Xml.Serialization;
using System.Xml;
using NLog;

namespace MOD4.Web.DomainService
{
    public class AccountDomainService : BaseDomainService, IAccountDomainService
    {
        private readonly IAccountInfoRepository _accountInfoRepository;
        private readonly IMenuRepository _menuRepository;

        public AccountDomainService(IAccountInfoRepository accountInfoRepository,
            IMenuRepository menuRepository)
        {
            _accountInfoRepository = accountInfoRepository;
            _menuRepository = menuRepository;
        }

        public List<AccountInfoEntity> GetAllAccountInfo()
            => _accountInfoRepository.SelectByConditions().Select(s => new AccountInfoEntity
            {
                sn = s.sn,
                Account = s.account,
                Name = s.name,
                Password = s.password,
                RoleId = s.role,
                JobId = s.jobId,
                Level_id = s.level_id,
                ApiKey = s.apiKey,
                DeptSn = s.deptSn,
                Mail = s.mail
            }).ToList();

        public List<AccountInfoEntity> GetAccountInfo(List<int> accountSnList)
        {
            return _accountInfoRepository.SelectByConditions(accountSnList: accountSnList).Select(s => new AccountInfoEntity
            {
                sn = s.sn,
                Account = s.account,
                Name = s.name,
                Password = s.password,
                RoleId = s.role,
                JobId = s.jobId,
                Level_id = s.level_id,
                ApiKey = s.apiKey,
                DeptSn = s.deptSn,
                Mail = s.mail
            }).ToList();
        }

        public List<AccountInfoEntity> GetAccountInfoByConditions(List<RoleEnum> roleIdList, string name, string jobId, string account)
        {
            return _accountInfoRepository.SelectByConditions(account: account, roleIdList: roleIdList, name: name, jobId: jobId).Select(s => new AccountInfoEntity
            {
                sn = s.sn,
                Account = s.account,
                Name = s.name,
                Password = s.password,
                RoleId = s.role,
                JobId = s.jobId,
                Level_id = s.level_id,
                ApiKey = s.apiKey,
                DeptSn = s.deptSn,
                Mail = s.mail
            }).ToList();
        }

        public AccessFabOrderFlowEntity GetAuditFlowInfo(UserEntity userEntity)
        {
            var _defDepartment = _accountInfoRepository.SelectDefinitionDepartment(deptSn: userEntity.DeptSn).FirstOrDefault();
            var _auditFlow = _accountInfoRepository.SelectAccessAuditFlow(userEntity.sn, userEntity.Level_id, _defDepartment.LevelId);
            return _auditFlow;
        }

        public AccountInfoEntity GetAccInfoByDepartment(UserEntity userEntity)
            => GetAccInfoListByDepartment(new List<int> { userEntity.DeptSn }).FirstOrDefault();

        public List<AccountInfoEntity> GetAccInfoListByDepartment(List<int> deptList)
        {
            var _daoList = _accountInfoRepository.SelectByConditions(deptList: deptList);

            return _daoList.Select(acc => new AccountInfoEntity
            {
                sn = acc.sn,
                Account = acc.account,
                Name = acc.name,
                Password = acc.password,
                RoleId = acc.role,
                JobId = acc.jobId,
                Level_id = acc.level_id,
                ApiKey = acc.apiKey,
                DeptSn = acc.deptSn,
                Mail = acc.mail
            }).ToList();
        }

        public List<AccountDeptEntity> GetAccountDepartmentList()
        {
            var _allAccountInfo = GetAccountInfo(null);
            _allAccountInfo = _allAccountInfo.OrderBy(o => o.Name, StringComparer.Create(new System.Globalization.CultureInfo(0x00030404), false)).ToList();
            var _allDepartmentInfo = _accountInfoRepository.SelectDefinitionDepartment();

            return _allAccountInfo.Select(acc => new AccountDeptEntity
            {
                sn = acc.sn,
                Account = acc.Account,
                Name = acc.Name,
                Password = acc.Password,
                RoleId = acc.RoleId,
                JobId = acc.JobId,
                Level_id = acc.Level_id,
                ApiKey = acc.ApiKey,
                DeptSn = acc.DeptSn,
                Mail = acc.Mail,
                DepartmentName = _allDepartmentInfo.FirstOrDefault(f => f.DeptSn == acc.DeptSn)?.DepartmentName ?? ""
            }).ToList();
        }

        public AccountEditEntity GetAccountAndMenuInfo(int accountSn)
        {
            var _accountInfo = GetAccountInfo(new List<int> { accountSn }).FirstOrDefault();
            var _accountMenuPermission = GetUserAllMenuPermission(accountSn);
            var _allMenu = _menuRepository.SelectAllMenu();

            // get department list
            var _catchDeptInfo = CatchHelper.Get($"deptList");

            List<OptionEntity> _deptOptionList = new List<OptionEntity>();
            List<DefinitionDepartmentDao> _allDepartmentList = new List<DefinitionDepartmentDao>();

            if (_catchDeptInfo == null)
            {
                _allDepartmentList = _accountInfoRepository.SelectDefinitionDepartment();
                CatchHelper.Set("deptList", JsonConvert.SerializeObject(_allDepartmentList), 604800);
            }
            else
                _allDepartmentList = JsonConvert.DeserializeObject<List<DefinitionDepartmentDao>>(_catchDeptInfo);

            int _sectionId = 0;
            int _deptId = 0;
            int _modId = 0;

            DefinitionDepartmentDao _currentDept = _allDepartmentList.FirstOrDefault(f => f.DeptSn == _accountInfo.DeptSn);

            if (_currentDept != null)
            {

                switch (_currentDept.LevelId)
                {
                    case 1:
                        _modId = _currentDept.DeptSn;
                        break;
                    case 2:
                        _deptId = _currentDept.DeptSn;
                        _modId = _currentDept.ParentDeptId;
                        break;
                    case 3:
                        _sectionId = _currentDept.DeptSn;
                        var _upper = _allDepartmentList.FirstOrDefault(f => f.DeptSn == _currentDept.ParentDeptId);
                        _deptId = _upper.DeptSn;
                        _modId = _upper.ParentDeptId;
                        break;
                }
            }

            AccountEditEntity _response = new AccountEditEntity
            {
                sn = _accountInfo.sn,
                Account = _accountInfo.Account,
                Password = Decrypt(_accountInfo.Password, "MOD4_Saikou"),
                Name = _accountInfo.Name,
                RoleId = _accountInfo.RoleId,
                ApiKey = _accountInfo.ApiKey,
                JobId = _accountInfo.JobId,
                Level_id = _accountInfo.Level_id,
                Mail = _accountInfo.Mail,
                MODId = _modId,
                DepartmentId = _deptId,
                SectionId = _sectionId,
                MenuPermissionList = new List<MenuPermissionEntity>()
            };

            _allMenu.ForEach(menu =>
            {
                var _currentMenuPermission = _accountMenuPermission.FirstOrDefault(accMenu => (int)accMenu.MenuSn == menu.sn);

                if (_currentMenuPermission != null)
                {
                    _response.MenuPermissionList.Add(new MenuPermissionEntity
                    {
                        IsMenuActive = true,
                        MenuId = (MenuEnum)menu.sn,
                        Menu = ((MenuEnum)menu.sn).GetDescription(),
                        ActionList = ConvertMenuActionPermission(_currentMenuPermission.AccountPermission)
                    });
                }
                else
                {
                    _response.MenuPermissionList.Add(new MenuPermissionEntity
                    {
                        IsMenuActive = false,
                        MenuId = (MenuEnum)menu.sn,
                        Menu = ((MenuEnum)menu.sn).GetDescription(),
                        ActionList = EnumHelper.GetEnumValue<PermissionEnum>().Select(action =>
                        new MenuActionEntity
                        {
                            IsActionActive = false,
                            ActionId = action,
                            Action = action.GetDescription()
                        }).ToList()
                    });
                }
            });

            return _response;
        }

        public string GetToken(string account, string password)
        {
            try
            {
                string url = "http://pcuxsamv4athetn.cminl.oa/api/SSO/Login";
                string _responseStr = "";
                string _token = "";
                string _ssoTicket4 = "";

                var _payload = JsonConvert.SerializeObject(new
                {
                    Password = password,
                    SysID = (string)null,
                    URL = (string)null,
                    UserID = account,
                    UserType = "Auto"
                });

                using (var client = new HttpClient())
                {
                    HttpContent ttcontent = new StringContent(_payload);
                    ttcontent.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                    var response = client.PostAsync(url, ttcontent).Result;

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        _token = response.Content.ReadAsStringAsync().Result;
                        _ssoTicket4 = response.Headers.FirstOrDefault(f => f.Key == "Set-Cookie").Value?.FirstOrDefault(sf => sf.Contains("SsoTicket4")).Split(";")[0].Split("=")[1] ?? "";
                        _responseStr = $"{_token}|{_ssoTicket4}";
                    }
                }

                return _responseStr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string VerifyToken(InxSSOEntity inxSSOEntity)
        {
            try
            {
                string url = "http://pcuxsamv4athetn.cminl.oa/api/SSO/VerifyToken";
                string _token = "";

                var _payload = JsonConvert.SerializeObject(inxSSOEntity);

                using (var client = new HttpClient())
                {
                    HttpContent ttcontent = new StringContent(_payload);
                    ttcontent.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                    ttcontent.Headers.Add("Cookie", $"SSOToken={inxSSOEntity.Token}");
                    ttcontent.Headers.Add("Cookie", $"SsoTicket4={inxSSOEntity.SSOTicket4}");
                    ttcontent.Headers.Add("Cookie", $"Name=TOWNS.WANG");
                    var response = client.PostAsync(url, ttcontent).Result;

                    _token = response.Content.ReadAsStringAsync().Result;
                }

                return _token;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool VerifyInxSSO(string account, string password)
        {
            string _ssoCookies = "";
            string _enCrypAccount = "";
            string _enCrypPsw = "";

            var _cryptoUrl = "http://inlcnws/PublicWebService/SSO/Crypto.asmx";

            // 加密

            FormUrlEncodedContent _cryptoPayload = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("EnCryptoStr", account)
            });

            using (var client = new HttpClient())
            {
                //var response = client.PostAsync(url, data);
                var response = client.PostAsync($"{_cryptoUrl}/getEnCrypto", _cryptoPayload).Result;
                //var response = client.GetAsync(url);

                string[] result = response.Content.ReadAsStringAsync().Result.Split("\">");

                //var _test = HttpUtility.HtmlDecode(result).Trim();

                //var _settings = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment, IgnoreWhitespace = true, IgnoreComments = true };
                //var reader = XmlReader.Create(new StringReader(_test), _settings);

                //XmlRootAttribute xRoot = new XmlRootAttribute();
                //xRoot.ElementName = "testele";
                //xRoot.IsNullable = true;
                //var _htmlSerializer = new XmlSerializer(typeof(TestEntity), xRoot);

                //var instance = (TestEntity)_htmlSerializer.Deserialize(reader);
                _enCrypAccount = result[1].Split("</")[0];
            }

            _cryptoPayload = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("EnCryptoStr", password)
            });

            using (var client = new HttpClient())
            {
                var response = client.PostAsync($"{_cryptoUrl}/getEnCrypto", _cryptoPayload).Result;

                string[] result = response.Content.ReadAsStringAsync().Result.Split("\">");

                _enCrypPsw = result[1].Split("</")[0];
            }

            string soapAction = "Public.WebService/checkIdentities";
            List<string[]> _string = new List<string[]>();
            _string.Add(new string[] { "towns.wang" });
            _string.Add(new string[] { "T@wns05160405" });
            var _userAndPasswd = JsonConvert.SerializeObject(_string.ToArray());

            List<KeyValuePair<string, string>> _bodyProper = new List<KeyValuePair<string, string>>();

            _bodyProper.Add(new KeyValuePair<string, string>("userAndPasswd[][]", _userAndPasswd));
            //_bodyProper.Add(new KeyValuePair<string, string>("", "T@wns05160405"));

            _cryptoPayload = new FormUrlEncodedContent(_bodyProper.ToArray());
            _cryptoPayload.Headers.Add("SOAPAction", soapAction);
            _cryptoPayload.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("text/xml");

            using (var client = new HttpClient())
            {
                var response = client.PostAsync("http://inlcnws/PublicWebService/SSO/Authentication.asmx", _cryptoPayload).Result;
                //var response = client.GetAsync(url);

                string result = response.Content.ReadAsStringAsync().Result;

            }




            string url = "http://inlcnws.cminl.oa/InxSSO/Logon.aspx";

            FormUrlEncodedContent formUrlEncodedContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("__VIEWSTATE","/wEPDwUKMTg0MjA5NDUxOWQYAQUeX19Db250cm9sc1JlcXVpcmVQb3N0QmFja0tleV9fFgUFDFJhZGlvQnV0dG9uMQUMUmFkaW9CdXR0b24yBQxSYWRpb0J1dHRvbjIFDFJhZGlvQnV0dG9uMwUMUmFkaW9CdXR0b24zYoJB7ujQoCzleJrWSiuWNWXGRZHQ7/SP30ANjeQ/SwM="),
                new KeyValuePair<string, string>("__VIEWSTATEGENERATOR","2610E229"),
                new KeyValuePair<string, string>("__EVENTVALIDATION","/wEdAAcNlBpVOiqrJRaXt8eqwelgSnv2O3fUgbn2xhKdsOTix6hXufbhIqPmwKf992GTkd212DBmZnljFuhZz59UzitQ0nXl4B2ZHr7gNcsGYW3A+4v2JRBi/RYri335vE45H6ainihG6d/Xh3PZm3b5AoMQjPWbB9wz7pHEpQ8H8GzwnKQqA7PMaES9P8xnfJlzdqc="),
                new KeyValuePair<string, string>("tbUserId",account),
                new KeyValuePair<string, string>("tbPassword",password),
                new KeyValuePair<string, string>("LoginType","RadioButton1"),
                new KeyValuePair<string, string>("btnLogin","Sign In"),
            });

            //// 新增加入 cookie 測試段落
            //var baseUrl = "http://mfgform/CFEQPLIC/ApplyEQPLIC.aspx?LoginFAB=O4";
            //var baseAddress = new Uri(baseUrl);
            //var cookie = new CookieContainer();
            //var handler = new HttpClientHandler() { CookieContainer = cookie };
            //using (var client = new HttpClient(handler))
            //{
            //    cookie.Add(baseAddress, new Cookie("CFEQPLICCookieUID", "UID=22008163"));
            //    cookie.Add(baseAddress, new Cookie("CFEQPLICCookiePWD", "PWD=22008163"));

            using (var client = new HttpClient())
            {
                //var response = client.PostAsync(url, data);
                var response = client.PostAsync(url, formUrlEncodedContent).Result;
                //var response = client.GetAsync(url);

                string result = response.Content.ReadAsStringAsync().Result;

                var _resHeaders = response.Headers;
                _ssoCookies = _resHeaders.FirstOrDefault(f => f.Key == "Set-Cookie").Value?.FirstOrDefault(sf => sf.Contains("SsoTicket")) ?? "";
            }

            if (_ssoCookies == "")
                return false;

            return true;
        }

        public (bool, AccountInfoEntity) DoUserVerify(LoginEntity loginEntity, string shaKey, bool requiredVerify)
        {
            try
            {
                AccountInfoEntity _accInfoEntity = new AccountInfoEntity();
                List<AccountInfoEntity> _accInfoList = new List<AccountInfoEntity>();

                // 密碼加密
                string _encryptPw = Encrypt(loginEntity.Password, shaKey);

                loginEntity.EncryptPw = _encryptPw;

                if (requiredVerify && loginEntity.Account.ToUpper() != "MFG")
                {
                    _accInfoEntity = DoInxSSOVerify(loginEntity);
                }
                else
                {
                    // 查詢 DB資料
                    _accInfoList = GetAllAccountInfo();
                    if (!_accInfoList.Any(a => a.Account.ToLower() == loginEntity.Account.ToLower() && a.Password == _encryptPw))
                        return (false, null);

                    _accInfoEntity = _accInfoList.FirstOrDefault(f => f.Account.ToLower() == loginEntity.Account.ToLower() && f.Password == _encryptPw);
                }

                CatchHelper.Delete(new string[] { $"accInfo" });
                CatchHelper.Set("accInfo", JsonConvert.SerializeObject(_accInfoList.Any() ? _accInfoList : GetAllAccountInfo()), 604800);

                _logHelper.WriteLog(LogLevel.Info, this.GetType().Name, $"使用者登錄:{_accInfoEntity.Name}({_accInfoEntity.JobId})");

                return (true, _accInfoEntity);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private AccountInfoEntity DoInxSSOVerify(LoginEntity loginEntity)
        {
            loginEntity.Account = loginEntity.Account.ToLower();

            Dictionary<string, string> _loginInfoDic = new Dictionary<string, string>()
            {
                { "account",loginEntity.Account},
                { "pasword",loginEntity.Password }
            };
            Dictionary<string, string> _enCryDic = new Dictionary<string, string>();

            string _cryptoUrl = "http://inlcnws/PublicWebService/SSO/Crypto.asmx";
            string _certificateUrl = "http://inlcnws/PublicWebService/SSO/Authentication.asmx/getCertificate";

            string[] _certificateRes;

            string _tokenTicket = GetToken(loginEntity.Account, loginEntity.Password);
            if (string.IsNullOrEmpty(_tokenTicket))
                throw new Exception("帳密錯誤 (無法取得Token)");

            // 加密
            foreach (KeyValuePair<string, string> item in _loginInfoDic)
            {
                FormUrlEncodedContent _cryptoPayload = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("EnCryptoStr", item.Value)
                });

                using (var client = new HttpClient())
                {
                    var response = client.PostAsync($"{_cryptoUrl}/getEnCrypto", _cryptoPayload).Result;

                    string[] result = response.Content.ReadAsStringAsync().Result.Split("\">");

                    _enCryDic.Add(item.Key, result[1].Split("</")[0]);
                }
            }

            FormUrlEncodedContent formUrlEncodedContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("userID", _enCryDic["account"]),
                new KeyValuePair<string, string>("password", _enCryDic["pasword"]),
                new KeyValuePair<string, string>("ipAddress", ""),
                new KeyValuePair<string, string>("certificate", "")
            });

            using (var client = new HttpClient())
            {
                var response = client.PostAsync(_certificateUrl, formUrlEncodedContent).Result;

                _certificateRes = response.Content.ReadAsStringAsync().Result.Split("<string>");
            }

            AccountInfoEntity _responseAccInfoEntity = InsertUpdateAccountInfo(loginEntity, _certificateRes);
            _responseAccInfoEntity.TokenTicket = _tokenTicket;

            return _responseAccInfoEntity;
        }

        private AccountInfoEntity InsertUpdateAccountInfo(LoginEntity loginEntity, string[] certificateResult)
        {
            try
            {
                var _catchDeptInfo = CatchHelper.Get($"deptList");

                List<OptionEntity> _deptOptionList = new List<OptionEntity>();
                List<DefinitionDepartmentDao> _allDepartmentList = new List<DefinitionDepartmentDao>();
                AccountInfoEntity _accountInfoEntity = new AccountInfoEntity()
                {
                    Account = loginEntity.Account,
                    Password = loginEntity.EncryptPw
                };

                if (_catchDeptInfo == null)
                {
                    _allDepartmentList = _accountInfoRepository.SelectDefinitionDepartment();
                    CatchHelper.Set("deptList", JsonConvert.SerializeObject(_allDepartmentList), 604800);
                }
                else
                    _allDepartmentList = JsonConvert.DeserializeObject<List<DefinitionDepartmentDao>>(_catchDeptInfo);


                AccountInfoDao _alreadyAcc = _accountInfoRepository.SelectByConditions(loginEntity.Account).FirstOrDefault();
                AccountInfoDao _updPwAcc = _accountInfoRepository.SelectByConditions(loginEntity.Account, loginEntity.EncryptPw).FirstOrDefault();

                // 新用戶, DB無資料
                if (_alreadyAcc == null && _updPwAcc == null)
                {
                    _accountInfoEntity.JobId = certificateResult[1].Split("</string")[0];
                    _accountInfoEntity.Name = certificateResult[4].Split("</string")[0];
                    _accountInfoEntity.Mail = certificateResult[6].Split("</string")[0];
                    _accountInfoEntity.RoleId = RoleEnum.User;
                    _accountInfoEntity.Level_id = JobLevelEnum.Employee;
                    _accountInfoEntity.DeptSn = _allDepartmentList.FirstOrDefault(f => f.DeptId == certificateResult[8].Split("</string")[0] && f.LevelId == 3)?.DeptSn ?? 0;
                    _accountInfoEntity.sn = InsertUserAndPermission(_accountInfoEntity);
                }
                // 既有用戶, 密碼變更
                else if (_alreadyAcc != null && _updPwAcc == null)
                {
                    UpdateUserPw(_accountInfoEntity.Account, _accountInfoEntity.Password);

                    _accountInfoEntity.sn = _alreadyAcc.sn;
                    _accountInfoEntity.JobId = _alreadyAcc.jobId;
                    _accountInfoEntity.Name = _alreadyAcc.name;
                    _accountInfoEntity.Mail = _alreadyAcc.mail;
                    _accountInfoEntity.RoleId = _alreadyAcc.role;
                    _accountInfoEntity.Level_id = _alreadyAcc.level_id;
                    _accountInfoEntity.DeptSn = _alreadyAcc.deptSn;
                }
                else
                {
                    _accountInfoEntity.sn = _alreadyAcc.sn;
                    _accountInfoEntity.JobId = _alreadyAcc.jobId;
                    _accountInfoEntity.Name = _alreadyAcc.name;
                    _accountInfoEntity.Mail = _alreadyAcc.mail;
                    _accountInfoEntity.RoleId = _alreadyAcc.role;
                    _accountInfoEntity.Level_id = _alreadyAcc.level_id;
                    _accountInfoEntity.DeptSn = _alreadyAcc.deptSn;
                }

                return _accountInfoEntity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<AccountMenuInfoEntity> GetUserAllMenuPermission(int userAccountSn)
        {
            return _accountInfoRepository.SelectUserMenuPermission(userAccountSn).Select(s => new AccountMenuInfoEntity
            {
                AccountSn = s.account_sn,
                MenuSn = (MenuEnum)s.menu_sn,
                MenuGroupSn = s.menu_group_sn,
                AccountPermission = s.account_permission
            }).ToList();
        }


        #region =========== AccountManagement ===========
        public string Create(AccountCreateEntity createEntity)
        {
            string _createRes = "";

            AccountInfoDao _insAccountInfoDao = new AccountInfoDao
            {
                account = createEntity.Account,
                password = Encrypt(createEntity.Password, "MOD4_Saikou"),
                name = createEntity.Name,
                jobId = createEntity.JobId,
                mail = createEntity.Mail,
                role = RoleEnum.User,
                apiKey = createEntity.ApiKey,
                deptSn = createEntity.SectionId != 0
                    ? createEntity.SectionId
                    : createEntity.DepartmentId != 0
                        ? createEntity.DepartmentId
                        : createEntity.MODId,
                level_id = createEntity.Level_id,
            };

            List<AccountMenuInfoDao> _insAccMenuInfo = createEntity.MenuPermissionList
                .Select(s =>
                new AccountMenuInfoDao
                {
                    menu_sn = (int)s.MenuSn,
                    account_permission = s.AccountPermission,
                    menu_group_sn = 0
                }).ToList();

            // 新增子頁權限, 查詢主頁 menu_sn 並新增
            var _parentPage = _menuRepository.SelectParentMenu(_insAccMenuInfo.Select(s => (int)s.menu_sn).ToList());
            _insAccMenuInfo.AddRange(_parentPage.Select(parentMenu => new AccountMenuInfoDao
            {
                menu_sn = parentMenu,
                account_permission = 1,
                menu_group_sn = 0
            }));

            using (var scope = new TransactionScope())
            {
                bool _insAccInfoRes = false;
                bool _insAccMenuInfoRes = false;

                _insAccInfoRes = _accountInfoRepository.InsertUserAccount(_insAccountInfoDao) == 1;
                var _newAccountSn =
                    _accountInfoRepository.SelectByConditions(account: _insAccountInfoDao.account, password: _insAccountInfoDao.password).FirstOrDefault().sn;

                _insAccMenuInfo.ForEach(fe => fe.account_sn = _newAccountSn);

                _insAccMenuInfoRes = _accountInfoRepository.InsertUserPermission(_insAccMenuInfo) == _insAccMenuInfo.Count;

                if (_insAccInfoRes && _insAccMenuInfoRes)
                    scope.Complete();
                else
                    _createRes = "";
            }

            CatchHelper.Delete(new string[] { $"accInfo" });

            return _createRes;
        }

        public string Update(AccountCreateEntity updateEntity)
        {
            string _updateRes = "";

            AccountInfoEntity _oldAccountInfo = GetAccountInfo(new List<int> { updateEntity.sn }).FirstOrDefault();

            AccountInfoDao _updAccountInfoDao = new AccountInfoDao
            {
                sn = updateEntity.sn,
                //account = updateEntity.Account,
                //password = Encrypt(updateEntity.Password, "MOD4_Saikou"),
                name = updateEntity.Name,
                jobId = updateEntity.JobId,
                mail = updateEntity.Mail,
                apiKey = updateEntity.ApiKey,
                deptSn = updateEntity.SectionId != 0
                       ? updateEntity.SectionId
                       : updateEntity.DepartmentId != 0
                           ? updateEntity.DepartmentId
                           : updateEntity.MODId,
                level_id = updateEntity.Level_id,
            };

            List<AccountMenuInfoDao> _insAccMenuInfo = updateEntity.MenuPermissionList
                .Select(s =>
                new AccountMenuInfoDao
                {
                    account_sn = updateEntity.sn,
                    menu_sn = (int)s.MenuSn,
                    account_permission = s.AccountPermission,
                    menu_group_sn = 0
                }).ToList();

            var _parentPage = _menuRepository.SelectParentMenu(_insAccMenuInfo.Select(s => s.menu_sn).ToList());

            _insAccMenuInfo.AddRange(_parentPage.Select(parentMenu => new AccountMenuInfoDao
            {
                account_sn = updateEntity.sn,
                menu_sn = parentMenu,
                account_permission = 0,
                menu_group_sn = 0
            }));

            using (var scope = new TransactionScope())
            {
                bool _insAccInfoRes = false;
                bool _insAccMenuInfoRes = false;

                _insAccInfoRes = _accountInfoRepository.UpdateUserAccount(_updAccountInfoDao) == 1;

                _accountInfoRepository.DeleteAccountPermission(_updAccountInfoDao.sn);
                _insAccMenuInfoRes = _accountInfoRepository.InsertUserPermission(_insAccMenuInfo) == _insAccMenuInfo.Count;

                if (_insAccInfoRes && _insAccMenuInfoRes)
                    scope.Complete();
                else
                    _updateRes = "";
            }

            CatchHelper.Delete(new string[] { "accInfo" });
            CatchHelper.Delete(new string[] { "userMenuInfo" });
            CatchHelper.Delete(new string[] { $"userInfo_{updateEntity.sn}" });
            CatchHelper.Delete(new string[] { "deptList" });

            return _updateRes;
        }
        #endregion


        #region Private

        private int InsertUserAndPermission(AccountInfoEntity accountInfoEntity)
        {
            try
            {
                int _accountSn = 0;

                using (var scope = new TransactionScope())
                {
                    _accountInfoRepository.InsertUserAccount(new AccountInfoDao
                    {
                        account = accountInfoEntity.Account,
                        password = accountInfoEntity.Password,
                        name = accountInfoEntity.Name,
                        role = accountInfoEntity.RoleId,
                        level_id = accountInfoEntity.Level_id,
                        mail = accountInfoEntity.Mail,
                        jobId = accountInfoEntity.JobId,
                        apiKey = "",
                        deptSn = accountInfoEntity.DeptSn
                    });

                    var _data = _accountInfoRepository.SelectByConditions(accountInfoEntity.Account, jobId: accountInfoEntity.JobId).FirstOrDefault();

                    if (_data == null)
                        return 0;

                    _accountSn = _data.sn;

                    _accountInfoRepository.InsertUserPermission(new List<AccountMenuInfoDao>
                    {
                        new AccountMenuInfoDao
                        {
                            account_sn = _data.sn,
                            menu_sn = 11,
                            menu_group_sn = 0,
                            account_permission = 0
                        },
                        new AccountMenuInfoDao
                        {
                            account_sn = _data.sn,
                            menu_sn = (int)MenuEnum.Demand,
                            menu_group_sn = 0,
                            account_permission = 3
                        },
                        new AccountMenuInfoDao
                        {
                            account_sn = _data.sn,
                            menu_sn = (int)MenuEnum.MESPermission,
                            menu_group_sn = 0,
                            account_permission = 3
                        },
                        new AccountMenuInfoDao
                        {
                            account_sn = _data.sn,
                            menu_sn = 12,
                            menu_group_sn = 0,
                            account_permission = 0
                        },
                        new AccountMenuInfoDao
                        {
                            account_sn = _data.sn,
                            menu_sn = (int)MenuEnum.AccessFab,
                            menu_group_sn = 0,
                            account_permission = 3
                        }
                    });
                    scope.Complete();
                }

                return _accountSn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string UpdateUserPw(string acc, string pw)
        {
            string _updRes = "";

            using (var scope = new TransactionScope())
            {
                var _updCnt = _accountInfoRepository.UpdateUserAccount(acc, pw);

                if (_updCnt == 1)
                    scope.Complete();
                else
                    _updRes = "更新失敗";
            }

            return _updRes;
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

        public string Decrypt(string password, string key = "MOD4_Saikou")
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

        private List<MenuActionEntity> ConvertMenuActionPermission(int permissionFlags)
        {
            List<MenuActionEntity> menuActionEntities = new List<MenuActionEntity>();
            menuActionEntities.Add(new MenuActionEntity
            {
                IsActionActive = Convert.ToBoolean(permissionFlags & (int)PermissionEnum.Create),
                Action = PermissionEnum.Create.GetDescription(),
                ActionId = PermissionEnum.Create
            });
            menuActionEntities.Add(new MenuActionEntity
            {
                IsActionActive = Convert.ToBoolean(permissionFlags & (int)PermissionEnum.Update),
                Action = PermissionEnum.Update.GetDescription(),
                ActionId = PermissionEnum.Update
            });
            menuActionEntities.Add(new MenuActionEntity
            {
                IsActionActive = Convert.ToBoolean(permissionFlags & (int)PermissionEnum.Audit),
                Action = PermissionEnum.Audit.GetDescription(),
                ActionId = PermissionEnum.Audit
            });
            menuActionEntities.Add(new MenuActionEntity
            {
                IsActionActive = Convert.ToBoolean(permissionFlags & (int)PermissionEnum.Reject),
                Action = PermissionEnum.Reject.GetDescription(),
                ActionId = PermissionEnum.Reject
            });
            menuActionEntities.Add(new MenuActionEntity
            {
                IsActionActive = Convert.ToBoolean(permissionFlags & (int)PermissionEnum.Cancel),
                Action = PermissionEnum.Cancel.GetDescription(),
                ActionId = PermissionEnum.Cancel
            });
            menuActionEntities.Add(new MenuActionEntity
            {
                IsActionActive = Convert.ToBoolean(permissionFlags & (int)PermissionEnum.Management),
                Action = PermissionEnum.Management.GetDescription(),
                ActionId = PermissionEnum.Management
            });

            return menuActionEntities;
        }

        #endregion
    }
}
