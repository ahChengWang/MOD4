﻿using MOD4.Web.Repostory.Dao;
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

        public List<AlarmXmlDao> SelectUnrepaired()
        {
            string sql = @" select ala.*, map.AREA from alarm_xml ala 
join equip_mapping map 
on ala.tool_id = map.EQUIP_NBR 
where ala.end_time is null and ala.user_id != 'AUTO' and map.isForMonitor = 1 ";

            var dao = _dbHelper.ExecuteQuery<AlarmXmlDao>(sql);

            return dao;
        }

        public List<AlarmXmlDao> SelectDayTopRepaired(string mfgDay)
        {
            string sql = @" select TOP 5 DATEDIFF(MINUTE,lm_time,end_time) 'repairedTime',* from alarm_xml 
where MFG_Day = @MFG_Day and DATEDIFF(MINUTE,lm_time,end_time) > 60 and user_id != 'AUTO' 
order by DATEDIFF(MINUTE,lm_time,end_time) desc; ";

            var dao = _dbHelper.ExecuteQuery<AlarmXmlDao>(sql,new {
                MFG_Day = mfgDay
            });

            return dao;
        }

        public List<ProdXmlTmpDao> SelectProdInfo(string mfgDay)
        {
            string sql = @" select tool_id,prod_id,map.AREA,MAX(CONVERT(int, move_cnt))'move_cnt' 
  from carUX_CFM_2f.dbo.prod_xml prod
  join equip_mapping map 
    on prod.tool_id = map.EQUIP_NBR 
 where prod.MFG_Day = @MFG_Day
   and prod.move_cnt != '' 
 group by tool_id,prod_id,map.AREA; ";
 //           string sql = @" select tool_id,prod_id,MAX(CONVERT(int, move_cnt))'move_cnt' 
 // from carUX_CFM_2f.dbo.prod_xml prod
 // join equip_mapping map 
 //   on prod.tool_id = map.EQUIP_NBR 
 //where prod.MFG_Day = @MFG_Day
 //  and map.isForMonitor = 1
 //  and prod.move_cnt != '' 
 //group by tool_id,prod_id; ";

            var dao = _dbHelper.ExecuteQuery<ProdXmlTmpDao>(sql, new
            {
                MFG_Day = mfgDay
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
