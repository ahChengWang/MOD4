using Microsoft.AspNetCore.Http;
using MOD4.Web.DomainService.Entity;
using System;
using System.Linq;
using System.Security.Claims;

namespace MOD4.Web
{
    public class UserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public UserEntity GetUserInfo()
        {
            var _userClaims = _httpContextAccessor.HttpContext.User.Claims;

            return new UserEntity
            {
                Account = _userClaims.FirstOrDefault(m => m.Type == ClaimTypes.Sid)?.Value,
                Name = _userClaims.FirstOrDefault(m => m.Type == ClaimTypes.Name)?.Value,
                sn = Convert.ToInt32(_userClaims.FirstOrDefault(m => m.Type == "sn").Value)
            };
        }
    }
}
