using Microsoft.AspNetCore.Http;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using System.IO;

namespace MOD4.Web.DomainService
{
    public interface IExtensionDomainService
    {
        string Upload(string jobId, ApplyAreaEnum applyAreaId, int itemId, IFormFile uploadFile, UserEntity userEntity);
        (byte[], string) Download(string jobId, ApplyAreaEnum applyAreaId, int itemId, UserEntity userEntity);

        string MPSUpload(IFormFile uploadFile, UserEntity userEntity);

        (bool, string, string) Download();
    }
}
