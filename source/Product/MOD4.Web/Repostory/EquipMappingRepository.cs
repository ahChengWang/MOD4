using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.Repostory
{
    public class EquipMappingRepository : BaseRepository, IEquipMappingRepository
    {

        public List<EquipMappingDao> SelectAll()
        {
            string sql = "select * from equip_mapping where ENABLE = 1; ";

            var dao = _dbHelper.ExecuteQuery<EquipMappingDao>(sql);

            return dao;
        }

        public List<EquipMappingDao> SelectEqByConditions(int floor = 0, string equipNo = "", string operation = "")
        {
            string sql = @"
select * from 
(select * from equip_mapping where Floor = 2
union
select * from MOD4_ENG.dbo.equip_mapping where Floor = 3) em
where 1=1 ";

            if (floor != 0)
                sql += " and em.FLOOR = @FLOOR ";
            if (!string.IsNullOrEmpty(equipNo))
                sql += " and em.EQUIP_NBR = @EQUIP_NBR ";
            if (!string.IsNullOrEmpty(operation))
                sql += " and em.OPERATION = @OPERATION ";

            var dao = _dbHelper.ExecuteQuery<EquipMappingDao>(sql, new
            {
                FLOOR = floor,
                EQUIP_NBR = equipNo,
                OPERATION = operation
            });

            return dao;
        }

        public List<EquipMappingDao> SelectEqByEqIdList(List<string> equipList = null)
        {
            string sql = "select * from equip_mapping where 1=1 ";

            if (equipList != null && equipList.Any())
                sql += " and EQUIP_NBR in @EQUIP_NBR ";

            var dao = _dbHelper.ExecuteQuery<EquipMappingDao>(sql, new 
            {
                EQUIP_NBR = equipList
            });

            return dao;
        }

        public int UpdateTarget(EquipMappingDao updDao)
        {
            string sql = $@"UPDATE [{(updDao.Floor == 3 ? "MOD4_ENG" : "carUX_2f")}].[dbo].[equip_mapping]
   SET mtbfTarget = @mtbfTarget
      ,mttrTarget = @mttrTarget
      ,UpdateTime = @UpdateTime
      ,UpdateUser = @UpdateUser
  WHERE EQUIP_NBR = @EQUIP_NBR; ";

            var dao = _dbHelper.ExecuteNonQuery(sql, updDao);

            return dao;
        }
    }
}
