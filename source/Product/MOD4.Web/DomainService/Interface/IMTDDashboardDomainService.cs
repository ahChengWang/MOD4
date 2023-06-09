﻿using Microsoft.AspNetCore.Http;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using System.Collections.Generic;

namespace MOD4.Web.DomainService
{
    public interface IMTDDashboardDomainService
    {
        List<MTDDashboardMainEntity> DashboardSearch(int floor = 2, string date = "", decimal time = 24, bool isMass = true);

        List<MftrScheduleEntity> Search(string dateRange = "", int floor = 2, int owner = 1, MTDCategoryEnum mtdCategoryId = MTDCategoryEnum.BOND);

        string GetLatestUpdate(int floor = 2, int owner = 1);

        string Upload(IFormFile formFile, int floor, int owner, UserEntity userEntity);

        MTBFMTTRDashboardEntity GetMTBFMTTRList(string beginDate, string endDate, string equipment, int floor);

        string UpdateMTBFMTTRSetting(EqMappingEntity updateEntity, UserEntity userEntity);
    }
}
