using MOD4.Web.DomainService.Entity;
using System.Collections.Generic;

namespace MOD4.Web.DomainService
{
    public interface ITargetSettingDomainService
    {
        List<TargetSettingEntity> GetList(List<int> prodSn = null, List<int> nodeList = null);

        string Update(int prodSn, List<TargetSettingEntity> settingList, UserEntity userEntity);

        List<MTDProcessSettingEntity> GetSettingForMTD(List<int> prodSnList);

        void Migration();
    }
}
