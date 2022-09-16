using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public class LineTTRepository : BaseRepository, ILineTTRepository
    {

        public List<LineTTDao> SelectByConditions(string mfgDay, List<string> lineList, string shift)
        {
            string sql = "select * from MOD4_ENG1.dbo.Line_TT where Line_TT > 0 and Line_TT < 1000 and MFG_Day=@MFG_Day and line IN @line ";

            if (shift == "A")
            {
                sql += " and MFG_HR < 12 ";
            }
            else
            {
                sql += " and MFG_HR >= 12 ";
            }

            var dao = _dbHelper.ExecuteQuery<LineTTDao>(sql, new
            {
                MFG_Day = mfgDay,
                line = lineList
            });

            return dao;
        }
    }
}
