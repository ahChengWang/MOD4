using Microsoft.AspNetCore.Http;
using System.ComponentModel;

namespace MOD4.Web.ViewModel
{
    public class MaterialUploadViewModel
    {
        [DisplayName("檔案")]
        public IFormFile File { get; set; }
    }
}
