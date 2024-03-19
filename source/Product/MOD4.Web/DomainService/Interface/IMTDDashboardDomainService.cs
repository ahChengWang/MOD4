using Microsoft.AspNetCore.Http;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MOD4.Web.DomainService
{
    public interface IMTDDashboardDomainService
    {
        (string Result, List<MTDDashboardMainEntity> Entitys) DashboardSearch(int floor = 2, string date = "", decimal time = 24, int owner = 1, string shift = "ALL");

        List<ManufactureScheduleEntity> Search(string dateRange = "", int floor = 2, int owner = 1);

        string GetLatestUpdate(int floor = 2, int owner = 1);

        string Upload(IFormFile formFile, int floor, int owner, UserEntity userEntity);

        List<MTDScheduleSettingEntity> GetMTDSetting(int prodSn = 1206);

        MTBFMTTRDashboardEntity GetMTBFMTTRList(string beginDate, string endDate, string equipment, int floor);

        string UpdateMTBFMTTRSetting(EqMappingEntity updateEntity, UserEntity userEntity);

        Task<List<MTDProcessDailyEntity>> GetMonitorDailyMTDAsync();

        string UpdateMTDSetting(List<MTDScheduleSettingEntity> updEntity, UserEntity userEntity);
    }
}
