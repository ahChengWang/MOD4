﻿using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface IDefinitionRWDefectCodeRepository
    {
        List<DefinitionRWDefectcodeDao> SelectByConditions();
    }
}