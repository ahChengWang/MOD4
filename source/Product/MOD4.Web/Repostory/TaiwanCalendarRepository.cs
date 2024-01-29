using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public class TaiwanCalendarRepository : BaseRepository, ITaiwanCalendarRepository
    {

        public List<TaiwanCalendarDao> SelectByConditions(int year)
        {
            string sql = "select * from taiwan_calendar where DATEPART(YEAR,date) = @Date and isHoliday = 1; ";

            var dao = _dbHelper.ExecuteQuery<TaiwanCalendarDao>(sql, new
            {
                Date = year
            });

            return dao;
        }
    }
}
