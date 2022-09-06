using MOD4.Web.DomainService.Entity;
using System.Collections.Generic;

namespace MOD4.Web.DomainService
{
    public interface ITargetSettingDomainService
    {
        List<TargetSettingEntity> GetList(List<string> nodeList = null);

        string Update(List<TargetSettingEntity> settingList);
    }
}
