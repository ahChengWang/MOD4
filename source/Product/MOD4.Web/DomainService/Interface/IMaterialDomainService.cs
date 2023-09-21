using Microsoft.AspNetCore.Http;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using System.Collections.Generic;

namespace MOD4.Web.DomainService
{
    public interface IMaterialDomainService
    {
        List<SAPWorkOrderEntity> GetSAPDropDownList();

        List<SAPWorkOrderEntity> GetSAPWorkOredr(string workOrder, string prodNo, string sapNode, string matrlNo);

        (bool, string, string) GetSAPwoCloseDownload(string workOrder, string prodNo, string sapNode, string matrlNo);

        (bool, string, string) UploadAndCalculate(IFormFile formFile, UserEntity userEntity);

        List<MaterialSettingEntity> GetMaterialSetting(MatlCodeTypeEnum codeTypeId);

        string UpdateMaterialSetting(List<MaterialSettingEntity> updEntity, MatlCodeTypeEnum codeTypeId, UserEntity userEntity);

        (bool, string) UploadCodeRate(IFormFile uploadFile, MatlCodeTypeEnum codeTypeId, UserEntity userEntity);

        (bool, string, string) MatlSettingDownload(MatlCodeTypeEnum codeTypeId);
    }
}