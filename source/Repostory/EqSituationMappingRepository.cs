using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.Repostory
{
    public class EqSituationMappingRepository : BaseRepository, IEqSituationMappingRepository
    {

        public List<EqSituationMappingDao> SelectList()
        {
            string sql = "select * from eq_situation_mapping ";

            var dao = _dbHelper.ExecuteQuery<EqSituationMappingDao>(sql);

            return dao;
        }
    }
}
