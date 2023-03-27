using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using System;
using System.Collections.Generic;

namespace MOD4.Web.DomainService
{
    public interface IEquipmentDomainService
    {
        List<(string, List<string>)> GetUnRepaireEqOptions();

        List<(string, List<string>)> GetRepairedEqDropdown(string date = null);

        List<EquipmentEntity> GetUnrepairedEqList(string date = null, string toolId = null);

        List<EquipmentEntity> GetRepairedEqList(string date = null, string toolId = null, string statusIdList = null, bool showAuto = false);

        List<(string, List<string>)> GetRepairedEqOptions(string mfgDate);

        (int, int) GetTodayRepairedEqPendingList();

        EquipmentEditEntity GetEditEqpinfo(int sn, UserEntity userEntity = null);

        EquipmentEditEntity GetDetailEqpinfo(int sn);

        string VerifyEqpStatus(int sn, EqIssueStatusEnum statusId, UserEntity userEntity);

        List<EquipmentEntity> GetEntityHistoryDetail(DateTime startTime, DateTime endTime, List<string> eqpListStr, List<int> prodSnList);

        string Create(EquipmentEditEntity editEntity, UserEntity userEntity);

        string UpdateEqpinfo(EquipmentEditEntity editEntity, UserEntity userEntity);
    }
}
