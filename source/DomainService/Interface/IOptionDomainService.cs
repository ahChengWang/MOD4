using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using System.Collections.Generic;

namespace MOD4.Web.DomainService
{
    public interface IOptionDomainService
    {
        List<OptionEntity> GetOptionByType(OptionTypeEnum optionTypeId, int mainId = 0, int subId = 0);
    }
}
