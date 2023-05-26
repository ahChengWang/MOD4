using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;

namespace MOD4.Web.Repostory
{
    public interface IEqpInfoRepository
    {
        List<string> SelectToolList(string date);
        List<EqpInfoDao> SelectByConditions(string date, List<string> equipmentList, bool isDefault, bool showAuto, List<int> prodSnList = null);
        List<EqpInfoDao> SelectEqpinfoByConditions(int sn, List<string> equipmentList = null, DateTime? startTime = null, DateTime? endTime = null, List<int> prodSnList = null);
        List<EquipMappingDao> SelectUnRepaireEqList(string beginDate, string endDate);
        List<EquipMappingDao> SelectRepairedEqList(string beginDate, string endDate);
        List<EqpInfoDao> SelectForMTBFMTTR(DateTime beginDate, DateTime endDate, string equipment, int floor);
        int Insert(List<EqpInfoDao> eqpInfoList);
        int UpdateEqpinfoByPM(EqpInfoDao updDao);
        int UpdateEqpinfoByENG(EqpInfoDao updDao);
    }
}
