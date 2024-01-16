using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using MOD4.Web.ViewModel;
using System.Collections.Generic;

namespace MOD4.Web.DomainService
{
    public interface IOptionDomainService
    {
        List<OptionEntity> GetEqProcessOptionByType(OptionTypeEnum optionTypeId, int mainId = 0, int subId = 0);

        List<EqSituationMappingEntity> GetAllEqProcessOption();

        List<OptionEntity> GetShiftOptionList();

        List<OptionEntity> GetPriorityOptionList();

        List<OptionEntity> GetDemandCategoryOptionList();

        List<OptionEntity> GetEqEvenCodeOptionList(int typeId = 0, int yId = 0, int subyId = 0, int xId = 0, int subxId = 0, int rId = 0);

        List<EqEvanCodeMappingEntity> GetEqEvenCode(int typeId = 0, int yId = 0, int subyId = 0, int xId = 0, int subxId = 0, int rId = 0);

        List<EqEvanCodeMappingEntity> GetAllEqEvenCodeOptionList();

        List<(string, List<OptionEntity>)> GetAccessFabOptions();

        List<(string, List<string>)> GetAreaEqGroupOptions();

        List<OptionEntity> GetLevelOptionList();

        List<OptionEntity> GetMenuOptionList();

        List<OptionEntity> GetLightingCategory();

        List<MenuPermissionViewModel> GetCreatePermissionList();

        List<(string, List<(int, string)>)> GetLcmProdOptions();

        List<OptionEntity> GetNodeList(int isActive = 1);

        List<EqMappingEntity> GetEqIDAreaList();

        List<OptionEntity> GetDepartmentOptionList(int parentDeptId, int levelId);

        List<CertifiedAreaMappingEntity> GetCertifiedAreaOptions();        

        List<(string, List<OptionEntity>)> GetSPCChartCategoryOptions();

        List<SPCChartSettingEntity> GetSPCMainChartOptions(int floor, string chartgrade);

        List<OptionEntity> GetMESPermission();

        List<OptionEntity> GetMESType();

        List<OptionEntity> GetAllSections();

        List<OptionEntity> GetAllNodeList();
    }
}
