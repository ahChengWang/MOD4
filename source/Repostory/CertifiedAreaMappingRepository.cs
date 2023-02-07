using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public class CertifiedAreaMappingRepository : BaseRepository, ICertifiedAreaMappingRepository
    {

        public List<CertifiedAreaMappingDao> SelectByConditions(int areaId = 0, int subjectId = 0)
        {
            string sql = "select * from certified_area_mapping where 1=1 ";

            if (areaId != 0)
                sql += " and AreaId = @AreaId ";

            if (subjectId != 0)
                sql += " and SubjectId = @SubjectId ";

            var dao = _dbHelper.ExecuteQuery<CertifiedAreaMappingDao>(sql, new
            {
                AreaId = areaId,
                SubjectId = subjectId
            });

            return dao;
        }
    }
}
