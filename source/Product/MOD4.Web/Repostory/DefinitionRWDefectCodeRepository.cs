using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public class DefinitionRWDefectCodeRepository : BaseRepository, IDefinitionRWDefectCodeRepository
    {

        public List<DefinitionRWDefectcodeDao> SelectByConditions()
        {
            string sql = "select * from definition_rw_defect_code where 1=1 ";

            var dao = _dbHelper.ExecuteQuery<DefinitionRWDefectcodeDao>(sql);

            return dao;
        }
    }
}
