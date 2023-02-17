using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public class SPCChartSettingRepository : BaseRepository, ISPCChartSettingRepository
    {

        public List<SPCChartSettingDao> SelectByConditions(string chartgrade, int floor)
        {
            string sql = "select * from [CarUXReport].[dbo].[SPC_Chart_Setting] where CHARTGRADE=@CHARTGRADE and FLOOR=@FLOOR ";

            var dao = _dbHelper.ExecuteQuery<SPCChartSettingDao>(sql, new
            {
                CHARTGRADE = chartgrade,
                FLOOR = floor,
            });

            return dao;
        }
    }
}
