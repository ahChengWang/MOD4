using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using MOD4.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MOD4.Web.Controllers
{
    public class BaseController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BaseController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public UserEntity GetUserInfo()
        {
            var _userClaims = _httpContextAccessor.HttpContext.User.Claims;

            return new UserEntity
            {
                sn = Convert.ToInt32(_userClaims.FirstOrDefault(m => m.Type == "sn").Value),
                Account = _userClaims.FirstOrDefault(m => m.Type == "Account")?.Value,
                Name = _userClaims.FirstOrDefault(m => m.Type == "Name")?.Value,
                RoleId = (RoleEnum)(Convert.ToInt32(_userClaims.FirstOrDefault(m => m.Type == "Role")?.Value))
            };
        }
    }
}
