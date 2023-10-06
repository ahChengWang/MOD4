using Microsoft.AspNetCore.Http;
using MOD4.Web.Enum;
using System;
using System.Collections.Generic;

namespace MOD4.Web.DomainService.Entity
{
    public class BulletinCreateEntity
    {
        public string Subject { get; set; }

        public string Content { get; set; }

        public string Target { get; set; }

        public IFormFile UploadFile { get; set; }
    }
}
