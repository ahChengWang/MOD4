using MOD4.Web.Enum;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.Repostory
{
    public class ReclaimWTRptRepository : BaseRepository, IReclaimWTRptRepository
    {

        public List<ReclaimWTRptDao> SelectByConditions(List<string> eqIdList, List<string> prodList)
        {
            string sql = "select * from reclaim_wt_rpt where 1=1 ";

            if (eqIdList != null && eqIdList.Any())
                sql += " and equip_id in @equip_id ";
            if (prodList != null && prodList.Any())
                sql += " and mes_prod_id in @mes_prod_id ";

            var dao = _dbHelper.ExecuteQuery<ReclaimWTRptDao>(sql, new
            {
                equip_id = eqIdList,
                mes_prod_id = prodList
            });

            return dao;
        }

    }
}
