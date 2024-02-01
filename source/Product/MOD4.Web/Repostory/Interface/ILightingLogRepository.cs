using MOD4.Web.Enum;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface ILightingLogRepository
    {
        List<LightingLogDao> SelectByConditions(List<int> snList = null, DateTime? startDate = null, DateTime? endDate = null, string panelId = "", LightingCategoryEnum? categoryId = null);
        int InsertLightingLog(List<LightingLogDao> daoList);
        int UpdateRWLog(List<LightingLogDao> updList);
    }
}