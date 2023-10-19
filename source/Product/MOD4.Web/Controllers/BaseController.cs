using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MOD4.Web.DomainService;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Utility.Helper;

namespace MOD4.Web.Controllers
{
    public class BaseController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccountDomainService _accountDomainService;

        public BaseController(IHttpContextAccessor httpContextAccessor,
            IAccountDomainService accountDomainService)
        {
            _httpContextAccessor = httpContextAccessor;
            _accountDomainService = accountDomainService;
        }

        public UserEntity GetUserInfo()
        {
            try
            {
                var _userClaims = _httpContextAccessor.HttpContext.User.Claims;

                var _catchUserInfo = CatchHelper.Get($"userInfo_{_userClaims.FirstOrDefault(m => m.Type == "JobId").Value}");

                if (_catchUserInfo == null)
                {
                    List<AccountMenuInfoEntity> _userPermission =
                        _accountDomainService.GetUserAllMenuPermission(Convert.ToInt32(_userClaims.FirstOrDefault(m => m.Type == "sn").Value));

                    var _userInfoEntity = _accountDomainService.GetAccountInfo(new List<int> { Convert.ToInt32(_userClaims.FirstOrDefault(m => m.Type == "sn").Value) }).FirstOrDefault();

                    if (_userInfoEntity == null)
                        throw new Exception("查無人員資料");

                    var _userEntity = new UserEntity
                    {
                        sn = _userInfoEntity.sn,
                        Account = _userInfoEntity.Account,
                        Name = _userInfoEntity.Name,
                        RoleId = _userInfoEntity.Role,
                        Level_id = _userInfoEntity.Level_id,
                        DeptSn = _userInfoEntity.DeptSn,
                        Mail = _userInfoEntity.Mail,
                        JobId = _userInfoEntity.JobId,
                        Password = _userInfoEntity.Password,
                        UserMenuPermissionList = _userPermission
                    };

                    //_userInfo.sn = Convert.ToInt32(_userClaims.FirstOrDefault(m => m.Type == "sn").Value);
                    //_userInfo.Account = _userClaims.FirstOrDefault(m => m.Type == ClaimTypes.Sid)?.Value;
                    //_userInfo.Name = _userClaims.FirstOrDefault(m => m.Type == ClaimTypes.Name)?.Value;
                    //_userInfo.RoleId = Convert.ToInt32(_userClaims.FirstOrDefault(m => m.Type == ClaimTypes.Role)?.Value);
                    //_userInfo.Level_id = (JobLevelEnum)Convert.ToInt32(_userClaims.FirstOrDefault(m => m.Type == "LevelId").Value);
                    //_userInfo.DeptSn = Convert.ToInt32(_userClaims.FirstOrDefault(m => m.Type == "DeptSn").Value);
                    //_userInfo.Mail = _userClaims.FirstOrDefault(m => m.Type == ClaimTypes.Email).Value;
                    //_userInfo.JobId = _userClaims.FirstOrDefault(m => m.Type == "JobId").Value;
                    //_userInfo.Password = _userClaims.FirstOrDefault(m => m.Type == "Psw").Value;
                    //_userInfo.UserMenuPermissionList = _userPermission;

                    CatchHelper.Set($"userInfo_{_userEntity.JobId}", JsonConvert.SerializeObject(new UserEntity
                    {
                        sn = _userInfoEntity.sn,
                        Account = _userInfoEntity.Account,
                        Name = _userInfoEntity.Name,
                        RoleId = _userInfoEntity.Role,
                        Level_id = _userInfoEntity.Level_id,
                        DeptSn = _userInfoEntity.DeptSn,
                        Mail = _userInfoEntity.Mail,
                        JobId = _userInfoEntity.JobId,
                        Password = _userInfoEntity.Password,
                        UserMenuPermissionList = _userPermission
                    }), 432000);

                    return _userEntity;
                }
                else
                {
                    return JsonConvert.DeserializeObject<UserEntity>(_catchUserInfo);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
