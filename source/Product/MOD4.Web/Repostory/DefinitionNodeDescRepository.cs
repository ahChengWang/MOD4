using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public class DefinitionNodeDescRepository : BaseRepository, IDefinitionNodeDescRepository
    {

        public List<DefinitionNodeDescDao> SelectByConditions(int eqNo = 0)
        {
            string sql = "select * from definition_node_desc where isActive=1 ";

            if (eqNo != 0)
            {
                sql += " and eqNo = @EqNo ";
            }

            var dao = _dbHelper.ExecuteQuery<DefinitionNodeDescDao>(sql, new
            {
                EqNo = eqNo
            });

            return dao;
        }
    }
}
