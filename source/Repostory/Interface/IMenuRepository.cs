﻿using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface IMenuRepository
    {
        List<MenuDao> SelectByConditions(int sn);
    }
}
