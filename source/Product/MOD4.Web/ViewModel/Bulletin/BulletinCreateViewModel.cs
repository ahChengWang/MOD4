using Microsoft.AspNetCore.Http;

namespace MOD4.Web.ViewModel
{
    public class BulletinCreateViewModel
    {
        public string Subject { get; set; }

        public string Content { get; set; }

        public string Target { get; set; }

        public bool IsIncludeIDL { get; set; }

        public IFormFile UploadFile { get; set; }
    }
}