using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.Repostory
{
    public class LcmProductRepository : BaseRepository, ILcmProductRepository
    {

        public List<LcmProductDao> SelectByConditions()
        {
            string sql = "select * from vw_lcm_prod where 1=1 ";

            var dao = _dbHelper.ExecuteQuery<LcmProductDao>(sql);

            return dao;
        }
    }
}
