using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public class EquipMappingRepository : BaseRepository, IEquipMappingRepository
    {

        public List<EquipMappingDao> SelectAll()
        {
            string sql = "select * from equip_mapping ";

            var dao = _dbHelper.ExecuteQuery<EquipMappingDao>(sql);

            return dao;
        }
    }
}
