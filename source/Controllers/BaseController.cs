using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MOD4.Web.DomainService;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using MOD4.Web.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

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
            var _userClaims = _httpContextAccessor.HttpContext.User.Claims;

            var _catchUserInfo = CatchHelper.Get($"userInfo_{Convert.ToInt32(_userClaims.FirstOrDefault(m => m.Type == "sn").Value)}");
            UserEntity _userInfo = new UserEntity();

            if (_catchUserInfo == null)
            {
                List<AccountMenuInfoEntity> _userPermission =
                    _accountDomainService.GetUserAllMenuPermission(Convert.ToInt32(_userClaims.FirstOrDefault(m => m.Type == "sn").Value));

                _userInfo.sn = Convert.ToInt32(_userClaims.FirstOrDefault(m => m.Type == "sn").Value);
                _userInfo.Account = _userClaims.FirstOrDefault(m => m.Type == "Account")?.Value;
                _userInfo.Name = _userClaims.FirstOrDefault(m => m.Type == "Name")?.Value;
                _userInfo.RoleId = (RoleEnum)(Convert.ToInt32(_userClaims.FirstOrDefault(m => m.Type == "Role")?.Value));
                _userInfo.Level_id = (JobLevelEnum)Convert.ToInt32(_userClaims.FirstOrDefault(m => m.Type == "LevelId").Value);
                _userInfo.DeptSn = Convert.ToInt32(_userClaims.FirstOrDefault(m => m.Type == "DeptSn").Value);
                _userInfo.Mail = _userClaims.FirstOrDefault(m => m.Type == "Mail").Value;
                _userInfo.JobId = _userClaims.FirstOrDefault(m => m.Type == "JobId").Value;
                _userInfo.UserMenuPermissionList = _userPermission;


                CatchHelper.Set($"userInfo_{_userInfo.sn}", JsonConvert.SerializeObject(_userInfo), 604800);

                return _userInfo;
            }
            else
            {
                return JsonConvert.DeserializeObject<UserEntity>(_catchUserInfo);
            }
        }
    }
}
