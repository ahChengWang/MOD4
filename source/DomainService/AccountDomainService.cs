using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using MOD4.Web.Helper;
using MOD4.Web.Repostory;
using MOD4.Web.Repostory.Dao;
using MOD4.Web.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;

namespace MOD4.Web.DomainService
{
    public class AccountDomainService : IAccountDomainService
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
        {
            var _dao = _accountInfoRepository.SelectByConditions(deptSn: userEntity.DeptSn).FirstOrDefault();

            return new AccountInfoEntity
            {
                sn = _dao.sn,
                Account = _dao.account,
                Name = _dao.name,
                Password = _dao.password,
                RoleId = _dao.role,
                JobId = _dao.jobId,
                Level_id = _dao.level_id,
                ApiKey = _dao.apiKey,
                DeptSn = _dao.deptSn,
                Mail = _dao.mail
            };
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

        public bool VerifyInxSSO(string account, string password)
        {
            string _ssoCookies = "";

            var url = "http://inlcnws.cminl.oa/InxSSO/Logon.aspx";

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

        public void InsertUpdateAccountInfo(string account, string password)
        {
            try
            {
                bool _alreadyAcc = _accountInfoRepository.SelectByConditions(account, password).Count == 1;
                bool _updPwAcc = _accountInfoRepository.SelectByConditions(account).Count == 1;

                if (!_alreadyAcc && !_updPwAcc)
                {
                    InsertUserAndPermission(account, password);
                    CatchHelper.Delete(new string[] { $"accInfo" });
                    CatchHelper.Set("accInfo", JsonConvert.SerializeObject(GetAllAccountInfo()), 604800);
                }
                else if (!_alreadyAcc && _updPwAcc)
                {
                    UpdateUserPw(account, password);
                    CatchHelper.Delete(new string[] { $"accInfo" });
                    CatchHelper.Set("accInfo", JsonConvert.SerializeObject(GetAllAccountInfo()), 604800);
                }
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
                role = RoleEnum.User,
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

            CatchHelper.Delete(new string[] { $"accInfo" });

            return _updateRes;
        }
        #endregion


        #region Private

        private string InsertUserAndPermission(string acc, string pw)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    _accountInfoRepository.InsertUserAccount(new AccountInfoDao
                    {
                        account = acc,
                        password = pw,
                        name = acc,
                        role = RoleEnum.User,
                        level_id = JobLevelEnum.Employee,
                        mail = "test@INNOLUX.COM",
                        jobId = "",
                    });

                    var _data = _accountInfoRepository.SelectByConditions(acc).FirstOrDefault();

                    if (_data == null)
                        return "使用者新增失敗";

                    _accountInfoRepository.InsertUserPermission(new List<AccountMenuInfoDao>
                {
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
                        menu_sn = 12,
                        menu_group_sn = 0,
                        account_permission = 0
                    },
                    new AccountMenuInfoDao
                    {
                        account_sn = _data.sn,
                        menu_sn = (int)MenuEnum.AccessFab,
                        menu_group_sn = 0,
                        account_permission = 0
                    }
                });
                    scope.Complete();
                }

                return "";
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
