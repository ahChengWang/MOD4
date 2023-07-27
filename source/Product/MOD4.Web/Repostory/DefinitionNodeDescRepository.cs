using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public class DefinitionNodeDescRepository : BaseRepository, IDefinitionNodeDescRepository
    {

        public List<DefinitionNodeDescDao> SelectByConditions(int eqNo = 0, int isActive = 1)
        {
            string sql = "select * from definition_node_desc where 1=1 ";

            if (eqNo != 0)
                sql += " and eqNo = @EqNo ";
            if (isActive == 1)
                sql += " and isActive=@IsActive ";

            var dao = _dbHelper.ExecuteQuery<DefinitionNodeDescDao>(sql, new
            {
                IsActive = isActive,
                EqNo = eqNo
            });

            return dao;
        }
    }
}
