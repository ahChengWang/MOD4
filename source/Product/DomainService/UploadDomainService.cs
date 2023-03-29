using Microsoft.Extensions.Configuration;

namespace MOD4.Web.DomainService
{
    public class UploadDomainService : IUploadDomainService
    {
        private readonly string _url = string.Empty;

        public UploadDomainService(IConfiguration config)
        {
            _url = config.GetSection("FileServer").GetValue<string>("Url");
        }


        public string GetFileServerUrl()
        => _url;
    }
}
