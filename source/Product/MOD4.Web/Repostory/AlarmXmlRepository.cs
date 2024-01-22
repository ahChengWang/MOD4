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

        public List<AlarmXmlDao> SelectByConditions(string date, List<string> toolIdList, int sn, bool isRepaired, List<string> statusList)
        {
            string sql = "select * from alarm_xml where 1=1";

            if (!string.IsNullOrEmpty(date))
                sql += " and MFG_Day = @MFG_Day ";
            if (toolIdList != null && toolIdList.Any())
                sql += " and tool_id in @tool_id ";
            if (sn != 0)
                sql += " and sn = @sn ";
            if (isRepaired)
                sql += " and end_time is not null ";
            else
                sql += " and end_time is null ";
            if (statusList != null && statusList.Any())
                sql += " and statusId in @statusId ";

            var dao = _dbHelper.ExecuteQuery<AlarmXmlDao>(sql, new
            {
                MFG_Day = date,
                tool_id = toolIdList,
                sn = sn,
                statusId = statusList
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

        public List<AlarmXmlDao> SelectRepaired(string mfgDay)
        {
            string sql = @" select * from alarm_xml where end_time is not null and user_id != 'AUTO' and MFG_Day = @MFG_Day ; ";

            var dao = _dbHelper.ExecuteQuery<AlarmXmlDao>(sql, new
            {
                MFG_Day = mfgDay
            });

            return dao;
        }

        public List<AlarmXmlDao> SelectDayTopRepaired(string mfgDay)
        {
            string sql = @" select TOP 10 DATEDIFF(MINUTE,lm_time,end_time) 'repairedTime',* from alarm_xml 
where MFG_Day = @MFG_Day and DATEDIFF(MINUTE,lm_time,end_time) > 30 and user_id != 'AUTO' 
order by DATEDIFF(MINUTE,lm_time,end_time) desc; ";

            var dao = _dbHelper.ExecuteQuery<AlarmXmlDao>(sql, new
            {
                MFG_Day = mfgDay
            });

            return dao;
        }

        public List<ProdXmlTmpDao> SelectProdInfo(string mfgDay)
        {
            string sql = @" select tool_id,prod_id,map.AREA,MAX(CONVERT(int, move_cnt))'move_cnt',MAX(XML_time)'XML_time'
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

        public int UpdateAlarmInfo(AlarmXmlDao updDao)
        {
            string sql = @"UPDATE [dbo].[alarm_xml]
   SET [comment] = @comment
      ,[shift] = @shift
      ,[eq_unitId] = @eq_unitId
      ,[defect_qty] = @defect_qty
      ,[defect_rate] = @defect_rate
      ,[engineer] = @engineer
      ,[priority] = @priority
      ,[memo] = @memo
      ,[mnt_user] = @mnt_user
      ,[mnt_minutes] = @mnt_minutes
      ,[processId] = @processId
      ,[eq_unit_partId] = @eq_unit_partId
      ,[typeId] = @typeId
      ,[yId] = @yId
      ,[subYId] = @subYId
      ,[xId] = @xId
      ,[subXId] = @subXId
      ,[rId] = @rId
      ,[statusId] = @statusId
 WHERE sn = @sn; ";

            var response = _dbHelper.ExecuteNonQuery(sql, updDao);

            return response;
        }

        public int UpdateAlarmInfoByENG(AlarmXmlDao updDao)
        {
            string sql = @"update alarm_xml set 
       [engTypeId] = @engTypeId
      ,[engYId] = @engYId
      ,[engSubYId] = @engSubYId
      ,[engXId] = @engXId
      ,[engSubXId] = @engSubXId
      ,[engRId] = @engRId
      ,[priority] = @priority
      ,[engineer] = @engineer
      ,[memo] = @memo
      ,[statusId] = @statusId
       where sn = @sn ;";

            var response = _dbHelper.ExecuteNonQuery(sql, updDao);

            return response;
        }

        public int InsertAlarmXml(AlarmXmlDao updDao)
        {
            string sql = @"INSERT INTO [dbo].[alarm_xml]
           ([tool_id]
           ,[tool_status]
           ,[status_cdsc]
           ,[user_id]
           ,[comment]
           ,[lm_time]
           ,[XML_time]
           ,[MFG_Day]
           ,[MFG_HR]
           ,[end_time]
           ,[prod_id]
           ,[prod_sn]
           ,[shift]
           ,[eq_unitId]
           ,[defect_qty]
           ,[defect_rate]
           ,[statusId]
           ,[memo]
           ,[mnt_user]
           ,[mnt_minutes]
           ,[processId]
           ,[eq_unit_partId]
           ,[typeId]
           ,[yId]
           ,[subYId]
           ,[xId]
           ,[subXId]
           ,[rId]
           ,[isManual])
     VALUES (@tool_id
           ,@tool_status
           ,@status_cdsc
           ,@user_id
           ,@comment
           ,@lm_time
           ,@XML_time
           ,@MFG_Day
           ,@MFG_HR
           ,@end_time
           ,@prod_id
           ,@prod_sn
           ,@shift
           ,@eq_unitId
           ,@defect_qty
           ,@defect_rate
           ,@statusId
           ,@memo
           ,@mnt_user
           ,@mnt_minutes
           ,@processId
           ,@eq_unit_partId
           ,@typeId
           ,@yId
           ,@subYId
           ,@xId
           ,@subXId
           ,@rId
           ,@isManual);";

            var response = _dbHelper.ExecuteNonQuery(sql, updDao);

            return response;
        }
    }
}
