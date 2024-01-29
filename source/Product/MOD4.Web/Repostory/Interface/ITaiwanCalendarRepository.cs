using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface ITaiwanCalendarRepository
    {
        List<TaiwanCalendarDao> SelectByConditions(int year);
    }
}