using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using MOD4.Web.ViewModel;
using System.Collections.Generic;

namespace MOD4.Web.DomainService
{
    public interface IOptionDomainService
    {
        List<OptionEntity> GetOptionByType(OptionTypeEnum optionTypeId, int mainId = 0, int subId = 0);

        List<OptionEntity> GetShiftOptionList();

        List<OptionEntity> GetPriorityOptionList();

        List<OptionEntity> GetDemandCategoryOptionList();

        List<OptionEntity> GetEqEvenCodeOptionList(int typeId = 0, int yId = 0, int subyId = 0, int xId = 0, int subxId = 0, int rId = 0);

        List<EqEvanCodeMappingEntity> GetEqEvenCode(int typeId = 0, int yId = 0, int subyId = 0, int xId = 0, int subxId = 0, int rId = 0);

        List<(string, List<OptionEntity>)> GetAccessFabOptions();

        List<(string, List<string>)> GetAreaEqGroupOptions();

        List<OptionEntity> GetLevelOptionList();

        List<OptionEntity> GetMenuOptionList();

        List<MenuPermissionViewModel> GetCreatePermissionList();

        List<(string, List<(int, string)>)> GetLcmProdOptions();

        List<OptionEntity> GetDepartmentOptionList(int parentDeptId, int levelId);
    }
}
