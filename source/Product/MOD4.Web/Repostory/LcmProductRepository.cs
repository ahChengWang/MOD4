using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.Repostory
{
    public class LcmProductRepository : BaseRepository, ILcmProductRepository
    {

        public List<LcmProductDao> SelectByConditions(List<int> snList = null)
        {
            string sql = "select * from vw_lcm_prod where 1=1 ";

            if (snList != null && snList.Any())
            {
                sql += " and sn in @Sn ";
            }

            var dao = _dbHelper.ExecuteQuery<LcmProductDao>(sql, new
            {
                Sn = snList
            });

            return dao;
        }

        public LcmProductDao Insert(LcmProductDao prodDao)
        {
            string sql = @"INSERT INTO [dbo].[definition_lcm_prod]
([prodNo]
,[descr])
VALUES
(@prodNo
,@descr); 
select TOP 1 sn from definition_lcm_prod order by sn desc;
";

            var dao = _dbHelper.ExecuteQuery<LcmProductDao>(sql, prodDao);

            return dao.FirstOrDefault();
        }
    }
}
