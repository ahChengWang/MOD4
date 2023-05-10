using Microsoft.AspNetCore.Http;
using MOD4.Web.DomainService.Entity;
using System.Collections.Generic;

namespace MOD4.Web.DomainService
{
    public interface IMTDDashboardDomainService
    {
        List<MTDDashboardEntity> DashboardSearch(int floor = 2, string date = "", decimal time = 24);

        (List<ManufactureScheduleEntity> manufactureSchedules, string latestUpdInfo) Search(string dateRange = "", int floor = 2);

        string Upload(IFormFile formFile, int floor, UserEntity userEntity);

        MTBFMTTRDashboardEntity GetMTBFMTTRList(string beginDate, string endDate, string equipment);

        string UpdateMTBFMTTRSetting(EqMappingEntity updateEntity, UserEntity userEntity);
    }
}
