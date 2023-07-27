using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.Repostory
{
    public class DailyEfficiencyRepository : BaseRepository, IDailyEfficiencyRepository
    {

        public List<DailyPerformanceDao> SelectByConditions(DateTime mfgDay, int floor = 3)
        {
            string sql = @" select ISNULL(def.prodNo,'') 'prodNo', daily.* from daily_performance daily 
left join definition_lcm_prod def 
on daily.lcmProdSn = def.sn 
where daily.MFG_Day = @MFG_Day and daily.floor = @Floor; ";

            var dao = _dbHelper.ExecuteQuery<DailyPerformanceDao>(sql, new
            {
                MFG_Day = mfgDay,
                Floor = floor
            });

            return dao;
        }

    }
}
