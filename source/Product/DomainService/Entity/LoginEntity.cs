using System;

namespace MOD4.Web.DomainService.Entity
{
    public class LoginEntity
    {
        public string Account { get; set; }

        public string Password { get; set; }

        public string EncryptPw { get; set; }

        public bool RememberMe { get; set; }

        public string Token { get; set; }
    }
}
