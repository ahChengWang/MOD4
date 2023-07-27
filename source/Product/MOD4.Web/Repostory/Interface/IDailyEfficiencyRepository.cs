using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface IDailyEfficiencyRepository
    {
        List<DailyPerformanceDao> SelectByConditions(DateTime mfgDay, int floor = 3);
    }
}