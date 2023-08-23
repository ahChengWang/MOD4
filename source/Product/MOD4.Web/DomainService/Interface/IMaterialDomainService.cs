using Microsoft.AspNetCore.Http;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;

namespace MOD4.Web.DomainService
{
    public interface IMaterialDomainService
    {
        (byte[], string) Download(string jobId, ApplyAreaEnum applyAreaId, int itemId, UserEntity userEntity);
        (bool, string, string) Upload(IFormFile formFile, UserEntity userEntity);
    }
}