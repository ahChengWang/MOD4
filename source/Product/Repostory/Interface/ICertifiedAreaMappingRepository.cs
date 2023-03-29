using MOD4.Web.Enum;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface ICertifiedAreaMappingRepository
    {
        List<CertifiedAreaMappingDao> SelectByConditions(int areaId = 0, int subjectId = 0);
    }
}
