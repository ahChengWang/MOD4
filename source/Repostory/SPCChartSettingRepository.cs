using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.Repostory
{
    public class SPCChartSettingRepository : BaseRepository, ISPCChartSettingRepository
    {

        public List<SPCChartSettingDao> SelectByConditions(string chartgrade, int floor, List<string> dataGroupList = null, List<string> prodList = null, List<string> eqList = null, int sn = 0)
        {
            string sql = "select * from [carUX_report].[dbo].[SPC_Chart_Setting] where 1=1 ";

            if (sn != 0)
                sql += " and sn = @Sn ";

            if (floor != 0)
                sql += " and FLOOR = @FLOOR ";

            if (!string.IsNullOrEmpty(chartgrade))
                sql += " and CHARTGRADE = @CHARTGRADE ";

            if (prodList != null && prodList.Any())
                sql += " and PECD in @PECD ";

            if (eqList != null && eqList.Any())
                sql += " and PEQPT_ID in @PEQPT_ID ";

            if (dataGroupList != null && dataGroupList.Any())
                sql += " and DataGroup in @DataGroup ";


            var dao = _dbHelper.ExecuteQuery<SPCChartSettingDao>(sql, new
            {
                Sn = sn,
                CHARTGRADE = chartgrade,
                FLOOR = floor,
                PECD = prodList,
                PEQPT_ID = eqList,
                DataGroup = dataGroupList
            });

            return dao;
        }

        public List<SPCChartSettingDao> SelectSPCFloorAndChartGrade()
        {
            string sql = "select CHARTGRADE,FLOOR from [carUX_report].[dbo].[SPC_Chart_Setting] group by CHARTGRADE,FLOOR;";

            var dao = _dbHelper.ExecuteQuery<SPCChartSettingDao>(sql);

            return dao;
        }
    }
}
