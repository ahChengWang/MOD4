using Microsoft.AspNetCore.Http;
using MOD4.Web.DomainService.Entity;
using System.Collections.Generic;

namespace MOD4.Web.DomainService
{
    public interface IMTDDashboardDomainService
    {
        List<ManufactureScheduleEntity> Search(string dateRange = "");

        string Upload(IFormFile formFile, UserEntity userEntity);
    }
}
