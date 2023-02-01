using MOD4.Web.Repostory.Dao;
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

        public EqpInfoDao SelectEqpinfoByConditions(int sn)
        {
            string sql = "select * from eqpinfo where 1=1";

            if (sn != 0)
            {
                sql += " and sn = @sn ";
            }

            var dao = _dbHelper.ExecuteQuery<EqpInfoDao>(sql, new
            {
                sn = sn
            });

            return dao.FirstOrDefault();
        }

        public List<EquipMappingDao> SelectUnRepaireEqList(string beginDate, string endDate)
        {
            string sql = @"WITH CTE AS
(select tool_id from alarm_xml_unrepaired where MFG_Day between @BeginDate and @EndDate
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

        public List<EquipMappingDao> SelectRepairedEqList(string beginDate,string endDate)
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

        public int Insert(EqpInfoDao eqpinfo)
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
,[isManual])
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
,@isManual); ";

            var dao = _dbHelper.ExecuteNonQuery(sql, eqpinfo);

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
