﻿using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.Repostory
{
    public class EqpInfoRepository : BaseRepository, IEqpInfoRepository
    {

        public List<string> SelectToolList(string date)
        {
            string sql = "select Equipment from vw_eqpinfo_repaired where 1=1 ";

            if (!string.IsNullOrEmpty(date))
            {
                sql += " and MFG_Day >= @MFG_Day ";
            }

            sql += "  group by Equipment; ";

            var dao = _dbHelper.ExecuteQuery<string>(sql, new
            {
                MFG_Day = date
            });

            return dao;
        }

        public List<EqpInfoDao> SelectByConditions(string date, List<string> equipmentList, bool isDefault, bool showAuto, List<int> prodSnList = null)
        {
            string sql = "select * from vw_eqpinfo_repaired_all where 1=1";

            if (!string.IsNullOrEmpty(date) && isDefault)
            {
                sql += " and MFG_Day >= @MFG_Day ";
            }
            else if (!string.IsNullOrEmpty(date))
            {
                sql += " and MFG_Day = @MFG_Day ";
            }
            if (equipmentList != null && equipmentList.Any())
            {
                sql += " and Equipment in @Equipment ";
            }
            if (!showAuto)
            {
                sql += " and Operator != 'AUTO' ";
            }
            if (prodSnList != null && prodSnList.Any())
            {
                sql += " and prod_sn in @ProdSn ";
            }


            var dao = _dbHelper.ExecuteQuery<EqpInfoDao>(sql, new
            {
                MFG_Day = date,
                Equipment = equipmentList,
                ProdSn = prodSnList
            });

            return dao;
        }

        public List<EqpInfoDao> SelectEqpinfoByConditions(int sn, List<string> equipmentList = null, DateTime? startTime = null, DateTime? endTime = null, List<int> prodSnList = null)
        {
            string sql = "select * from eqpinfo where Code != '111A' ";

            if (sn != 0)
                sql += " and sn = @sn ";
            if (equipmentList != null && equipmentList.Any())
                sql += " and Equipment in @Equipment ";
            if (prodSnList != null && prodSnList.Any())
                sql += " and prod_sn in @ProdSn ";
            if (startTime != null)
                sql += " and Start_Time >= @startTime ";
            if (endTime != null)
                sql += " and Start_Time <= @endTime ";

            var dao = _dbHelper.ExecuteQuery<EqpInfoDao>(sql, new
            {
                sn = sn,
                Equipment = equipmentList,
                ProdSn = prodSnList,
                startTime = startTime,
                endTime = endTime
            });

            return dao;
        }

        public List<EquipMappingDao> SelectUnRepaireEqList(string beginDate, string endDate)
        {
            string sql = @"WITH CTE AS
(select tool_id from alarm_xml where MFG_Day between @BeginDate and @EndDate and end_time is not null 
 group by tool_id)
select b.EQUIP_NBR, b.AREA 
  from CTE cte
  join equip_mapping b
    on cte.tool_id = b.EQUIP_NBR ";

            var dao = _dbHelper.ExecuteQuery<EquipMappingDao>(sql, new
            {
                BeginDate = beginDate,
                EndDate = endDate
            });

            return dao;
        }

        public List<EquipMappingDao> SelectRepairedEqList(string beginDate, string endDate)
        {
            string sql = @"WITH CTE AS
(
select Equipment from eqpinfo 
 where Start_Time between @BeginDate and @EndDate
 group by Equipment
)
select b.EQUIP_NBR, b.AREA 
  from CTE cte
  join equip_mapping b
    on cte.Equipment = b.EQUIP_NBR ";

            var dao = _dbHelper.ExecuteQuery<EquipMappingDao>(sql, new
            {
                BeginDate = beginDate,
                EndDate = endDate
            });

            return dao;
        }

        public List<EqpInfoDao> SelectForMTBFMTTR(DateTime beginDate, DateTime endDate, string equipment,int floor)
        {
            try
            {
                string sql = "";
                if (floor == 2)
                {
                    sql = @"select Equipment,Operator,Code,Code_Desc,Comments,MIN(Start_Time)'Start_Time',SUM(Convert(decimal,Repair_Time))'Repair_Time' 
                                , DATEADD(MINUTE,SUM(Convert(decimal,Repair_Time)),MIN(Start_Time))'End_Time' 
  from eqpinfo where Code not in ('111A','121A') and Code not like '2%' and Start_Time > @BeginDate and Start_Time < @EndDate and Equipment = @Equipment
 group by Equipment,Operator,Code,Code_Desc,Comments,P_key order by Start_Time asc; ";
                }
                else
                {
                    sql = @"select Equipment,Operator,Code,Code_Desc,Comments,MIN(Start_Time)'Start_Time',SUM(Convert(decimal,Repair_Time))'Repair_Time' 
                                , DATEADD(MINUTE,SUM(Convert(decimal,Repair_Time)),MIN(Start_Time))'End_Time' 
  from MOD4_ENG.dbo.eqpinfo where Code not in ('111A','121A') and Code not like '2%' and Start_Time > @BeginDate and Start_Time < @EndDate and Equipment = @Equipment
 group by Equipment,Operator,Code,Code_Desc,Comments,P_key order by Start_Time asc; ";
                }

                var dao = _dbHelper.ExecuteQuery<EqpInfoDao>(sql, new
                {
                    Equipment = equipment,
                    BeginDate = beginDate,
                    EndDate = endDate
                });

                return dao;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int Insert(List<EqpInfoDao> eqpInfoList)
        {
            string sql = @"INSERT INTO [dbo].[eqpinfo]
([Equipment]
,[Operator]
,[Code]
,[Code_Desc]
,[Comments]
,[Start_Time]
,[Repair_Time]
,[Update_Time]
,[shift]
,[eq_unitId]
,[defect_qty]
,[defect_rate]
,[engineer]
,[memo]
,[mnt_minutes]
,[prod_id]
,[typeId]
,[yId]
,[subYId]
,[xId]
,[subXId]
,[rId]
,[processId]
,[eq_unit_partId]
,[statusId]
,[prod_sn]
,[isManual]
,[status_desc_ie]
,[P_key])
VALUES
(@Equipment
,@Operator
,@Code
,@Code_Desc
,@Comments
,@Start_Time
,@Repair_Time
,@Update_Time
,@shift
,@eq_unitId
,@defect_qty
,@defect_rate
,@engineer
,@memo
,@mnt_minutes
,@prod_id
,@typeId
,@yId
,@subYId
,@xId
,@subXId
,@rId
,@processId
,@eq_unit_partId
,@statusId
,@prod_sn
,@isManual
,@status_desc_ie
,@P_key); ";

            var dao = _dbHelper.ExecuteNonQuery(sql, eqpInfoList);

            return dao;
        }

        public int UpdateEqpinfoByPM(EqpInfoDao updDao)
        {
            string sql = @"update eqpinfo set 
                Comments = @Comments ,
                shift = @shift ,
                processId = @processId ,
                eq_unitId = @eq_unitId ,
                eq_unit_partId = @eq_unit_partId ,
                defect_qty = @defect_qty ,
                defect_rate = @defect_rate ,
                mnt_user = @mnt_user ,
                mnt_minutes = @mnt_minutes ,
                typeId = @typeId ,
                yId = @yId ,
                subYId = @subYId ,
                xId = @xId ,
                subXId = @subXId ,
                rId = @rId  ,
                statusId = @statusId  
                where sn = @sn ";

            var response = _dbHelper.ExecuteNonQuery(sql, new
            {
                sn = updDao.sn,
                Comments = updDao.Comments,
                shift = updDao.shift,
                processId = updDao.processId,
                eq_unitId = updDao.eq_unitId,
                eq_unit_partId = updDao.eq_unit_partId,
                defect_qty = updDao.defect_qty,
                defect_rate = updDao.defect_rate,
                mnt_user = updDao.mnt_user,
                mnt_minutes = updDao.mnt_minutes,
                typeId = updDao.typeId,
                yId = updDao.yId,
                subYId = updDao.subYId,
                xId = updDao.xId,
                subXId = updDao.subXId,
                rId = updDao.rId,
                statusId = updDao.statusId
            });

            return response;
        }

        public int UpdateEqpinfoByENG(EqpInfoDao updDao)
        {
            string sql = @"update eqpinfo set 
                xId = @xId ,
                subXId = @subXId ,
                rId = @rId ,
                priority = @priority ,
                engineer = @engineer ,
                memo = @memo ,
                statusId = @statusId  
                where sn = @sn ";

            var response = _dbHelper.ExecuteNonQuery(sql, new
            {
                sn = updDao.sn,
                xId = updDao.xId,
                subXId = updDao.subXId,
                rId = updDao.rId,
                priority = updDao.priority,
                engineer = updDao.engineer,
                memo = updDao.memo,
                statusId = updDao.statusId
            });

            return response;
        }
    }
}
