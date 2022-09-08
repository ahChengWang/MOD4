using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.Repostory
{
    public class AlarmXmlRepository : BaseRepository, IAlarmXmlRepository
    {


        public List<string> SelectToolList()
        {
            string sql = "select tool_id from alarm_xml_unrepaired group by tool_id;";

            var dao = _dbHelper.ExecuteQuery<string>(sql);

            return dao;
        }


        public List<AlarmXmlDao> SelectByConditions(string date, List<string> toolIdList)
        {
            string sql = "select * from alarm_xml_unrepaired where 1=1";

            if (!string.IsNullOrEmpty(date))
            {
                sql += " and MFG_Day = @MFG_Day ";
            }
            if (toolIdList != null && toolIdList.Any())
            {
                sql += " and tool_id in @tool_id ";
            }

            var dao = _dbHelper.ExecuteQuery<AlarmXmlDao>(sql, new
            {
                MFG_Day = date,
                tool_id = toolIdList
            });

            return dao;
        }
    }
}
