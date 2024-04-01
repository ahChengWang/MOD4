using Microsoft.AspNetCore.Http;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using System;
using System.Collections.Generic;
using System.IO;

namespace MOD4.Web.DomainService
{
    public interface IExtensionDomainService
    {
        List<LightingLogMainEntity> GetLightingHisList(string panelId = "", string yearMonth = "");

        List<LightingLogEntity> GetLightingDayLogList(string panelDate, LightingCategoryEnum categoryId);

        string CreateLightingLog(List<LightingLogEntity> lightingLogList, UserEntity userEntity);

        string UpdateLightingLog(List<LightingLogEntity> lightingLogList, UserEntity userEntity);

        List<LightingLogEntity> GetLightingLogById(string panelId);

        string Upload(string jobId, ApplyAreaEnum applyAreaId, int itemId, IFormFile uploadFile, UserEntity userEntity);
        (byte[], string) Download(string jobId, ApplyAreaEnum applyAreaId, int itemId, UserEntity userEntity);

        string MPSUpload(IFormFile uploadFile, UserEntity userEntity);

        (bool, string, string) Download();

        (bool, string, string) DownloadLog(DateTime logDate);

    }
}
