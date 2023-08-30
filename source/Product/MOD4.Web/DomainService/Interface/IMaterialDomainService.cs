using Microsoft.AspNetCore.Http;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using System.Collections.Generic;

namespace MOD4.Web.DomainService
{
    public interface IMaterialDomainService
    {
        List<MaterialSettingEntity> GetMaterialSetting(MatlCodeTypeEnum codeTypeId);
        string UpdateMaterialSetting(List<MaterialSettingEntity> updEntity, MatlCodeTypeEnum codeTypeId, UserEntity userEntity);
        (byte[], string) Download(string jobId, ApplyAreaEnum applyAreaId, int itemId, UserEntity userEntity);
        (bool, string, string) Upload(IFormFile formFile, UserEntity userEntity);

        string UploadCodeRate(IFormFile uploadFile, MatlCodeTypeEnum codeTypeId, UserEntity userEntity);

        (bool, string, string) MatlSettingDownload(MatlCodeTypeEnum codeTypeId);
    }
}