using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using System.Collections.Generic;

namespace MOD4.Web.DomainService
{
    public interface IEquipmentDomainService
    {
        List<string> GetUnrepairedEqDropdown();

        List<string> GetRepairedEqDropdown(string date = null);

        List<EquipmentEntity> GetUnrepairedEqList(string date = null, string toolId = null);

        List<EquipmentEntity> GetRepairedEqList(string date = null, string toolId = null, string statusIdList = null);

        (int, int) GetTodayRepairedEqPendingList();

        EquipmentEditEntity GetEditEqpinfo(int sn, UserEntity userEntity = null);

        EquipmentEditEntity GetDetailEqpinfo(int sn);

        string VerifyEqpStatus(int sn, EqIssueStatusEnum statusId, UserEntity userEntity);

        List<EquipmentEntity> GetEntityHistoryDetail(string mfgDay, List<string> eqpListStr);

        string UpdateEqpinfo(EquipmentEditEntity editEntity);
    }
}
