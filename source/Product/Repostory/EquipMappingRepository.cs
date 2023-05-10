using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;

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

        public List<EquipMappingDao> SelectEqByConditions(int floor)
        {
            string sql = "select * from equip_mapping where AREA is not null and FLOOR = @FLOOR; ";

            var dao = _dbHelper.ExecuteQuery<EquipMappingDao>(sql, new 
            {
                FLOOR = floor
            });

            return dao;
        }

        public int UpdateTarget(EquipMappingDao updDao)
        {
            string sql = @"UPDATE [dbo].[equip_mapping]
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
