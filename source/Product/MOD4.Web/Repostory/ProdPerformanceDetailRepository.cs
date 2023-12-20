using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public class ProdPerformanceDetailRepository : BaseRepository, IProdPerformanceDetailRepository
    {

        public List<ProdPerformanceDetailDao> SelectByConditions(DateTime srcStartDate, DateTime endStartDate)
        {
            string sql = @"select ROW_NUMBER() OVER (PARTITION BY prodNo,equipNo,workCtr ORDER BY transDate DESC) 'sn', * 
from prod_performance_detail where transDate between @StartDate and @EndDate ";

            var dao = _dbHelper.ExecuteQuery<ProdPerformanceDetailDao>(sql, new
            {
                StartDate = srcStartDate,
                EndDate = endStartDate,
            });

            return dao;
        }
    }
}
