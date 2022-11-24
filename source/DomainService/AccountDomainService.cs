using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using MOD4.Web.Helper;
using MOD4.Web.Repostory;
using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Transactions;

namespace MOD4.Web.DomainService
{
    public class AccountDomainService : IAccountDomainService
    {
        private readonly IAccountInfoRepository _accountInfoRepository;

        public AccountDomainService(IAccountInfoRepository accountInfoRepository)
        {
            _accountInfoRepository = accountInfoRepository;
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

        public AccessFabOrderFlowEntity GetAuditFlowInfo(UserEntity userEntity)
        {
            var _defDepartment = _accountInfoRepository.SelectDefinitionDepartment(userEntity.DeptSn);
            var _auditFlow = _accountInfoRepository.SelectAccessAuditFlow(userEntity.sn, userEntity.Level_id, _defDepartment.LevelId);
            return _auditFlow;
        }

        public AccountInfoEntity GetAccInfoByDepartment(UserEntity userEntity)
        {
            var _dao = _accountInfoRepository.SelectByConditions(deptSn: userEntity.DeptSn).FirstOrDefault();

            return new AccountInfoEntity { 
                sn = _dao.sn,
                Account = _dao.account,
                Name = _dao.name,
                Password = _dao.password,
                RoleId = _dao.role,
                JobId = _dao.jobId,
                Level_id = _dao.level_id,
                ApiKey = _dao.apiKey,
                DeptSn = _dao.deptSn
            };
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
            bool _alreadyAcc = _accountInfoRepository.SelectByConditions(account, password).Count == 1;
            bool _updPwAcc = _accountInfoRepository.SelectByConditions(account).Count == 1;

            if (!_alreadyAcc && !_updPwAcc)
            {
                InsertUserAndPermission(account, password);
                CatchHelper.Delete(new string[] { $"accInfo" });
            }
            else if (!_alreadyAcc && _updPwAcc)
            {
                UpdateUserPw(account, password);
                CatchHelper.Delete(new string[] { $"accInfo" });
            }
        }

        public List<AccountMenuInfoEntity> GetUserAllMenuPermission(int userAccountSn)
        {
            return _accountInfoRepository.SelectUserMenuPermission(userAccountSn).Select(s => new AccountMenuInfoEntity
            {
                AccountSn = s.account_sn,
                MenuSn = s.menu_sn,
                MenuGroupSn = s.menu_group_sn,
                AccountPermission = s.account_permission
            }).ToList();
        }

        private string InsertUserAndPermission(string acc, string pw)
        {
            using (var scope = new TransactionScope())
            {
                _accountInfoRepository.InsertUserAccount(new AccountInfoDao
                {
                    account = acc,
                    password = pw,
                    name = acc,
                    role = RoleEnum.User,
                    level_id = JobLevelEnum.Employee
                });

                var _data = _accountInfoRepository.SelectByConditions(acc).FirstOrDefault();

                if (_data == null)
                    return "使用者新增失敗";

                _accountInfoRepository.InsertUserPermission(new List<AccountMenuInfoDao>
                {
                    new AccountMenuInfoDao
                    {
                        account_sn = _data.sn,
                        menu_sn = MenuEnum.Demand,
                        menu_group_sn = 0
                    }
                });
                scope.Complete();
            }

            return "";
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
    }
}
