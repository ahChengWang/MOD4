﻿using MOD4.Web.Repostory.Dao;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface IMonitorSettingRepository
    {
        int Insert(List<MonitorSettingDao> insSettingList);
        List<MonitorSettingDao> SelectSettings();
        int Update(List<MonitorSettingDao> updMonitorList);

        int Delete();
    }
}