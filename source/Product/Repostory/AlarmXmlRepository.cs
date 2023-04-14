using MOD4.Web.Repostory.Dao;
using System;
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

        public List<AlarmXmlDao> SelectForMTD(DateTime date, List<string> toolIdList, List<string> prodList)
        {
            string sql = $@"select * from (
select ROW_NUMBER() OVER (Partition by tool_id order by DATEDIFF(MINUTE,XML_time,CASE WHEN end_time is null THEN GETDATE() ELSE end_time END) desc)'no'
, DATEDIFF(MINUTE,XML_time,CASE WHEN end_time is null THEN GETDATE() ELSE end_time END)'spend_time',* 
from alarm_xml 
where ((end_time is not null and DATEDIFF(MINUTE,XML_time,end_time) >= 60) or (end_time is null and DATEDIFF(MINUTE,XML_time,GETDATE()) >= 60 ))  and MFG_Day = @MFG_Day and prod_id in @Prod_id 
) z
where z.no = 1; ";

            var dao = _dbHelper.ExecuteQuery<AlarmXmlDao>(sql, new
            {
                MFG_Day = date,
                tool_id = toolIdList,
                Prod_id = prodList
            });

            return dao;
        }
    }
}
