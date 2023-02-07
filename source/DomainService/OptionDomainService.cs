using Microsoft.Extensions.Options;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using MOD4.Web.Helper;
using MOD4.Web.Repostory;
using MOD4.Web.Repostory.Dao;
using MOD4.Web.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOD4.Web.DomainService
{
    public class OptionDomainService : IOptionDomainService
    {
        private readonly IEqSituationMappingRepository _eqSituationMappingRepository;
        private readonly IEqEvanCodeMappingRepository _eqEvanCodeMappingRepository;
        private readonly IEquipMappingRepository _equipMappingRepository;
        private readonly IAccountInfoRepository _accountInfoRepository;
        private readonly IMenuRepository _menuRepository;
        private readonly ILcmProductRepository _lcmProductRepository;
        private readonly ICertifiedAreaMappingRepository _certifiedAreaMappingRepository;

        public OptionDomainService(IEqSituationMappingRepository eqSituationMappingRepository,
            IEqEvanCodeMappingRepository eqEvanCodeMappingRepository,
            IEquipMappingRepository equipMappingRepository,
            IAccountInfoRepository accountInfoRepository,
            IMenuRepository menuRepository,
            ILcmProductRepository lcmProductRepository,
            ICertifiedAreaMappingRepository certifiedAreaMappingRepository)
        {
            _eqSituationMappingRepository = eqSituationMappingRepository;
            _eqEvanCodeMappingRepository = eqEvanCodeMappingRepository;
            _equipMappingRepository = equipMappingRepository;
            _accountInfoRepository = accountInfoRepository;
            _menuRepository = menuRepository;
            _lcmProductRepository = lcmProductRepository;
            _certifiedAreaMappingRepository = certifiedAreaMappingRepository;
        }


        public List<OptionEntity> GetEqProcessOptionByType(OptionTypeEnum optionTypeId, int mainId = 0, int subId = 0)
        {
            try
            {
                var _catchEqMapping = CatchHelper.Get($"eqMappingList");
                List<EqSituationMappingDao> _eqMappingList = new List<EqSituationMappingDao>();

                if (_catchEqMapping == null)
                {
                    _eqMappingList = _eqSituationMappingRepository.SelectList();
                    CatchHelper.Set($"eqMappingList", JsonConvert.SerializeObject(_eqMappingList), 86400);
                }
                else
                {
                    _eqMappingList = JsonConvert.DeserializeObject<List<EqSituationMappingDao>>(_catchEqMapping);
                }


                switch (optionTypeId)
                {
                    case OptionTypeEnum.ProcessOption:
                        return _eqMappingList.GroupBy(eq => new { eq.Process, eq.ProcessId })
                            .Select(s => new OptionEntity { Id = s.Key.ProcessId, Value = s.Key.Process }).ToList();
                    case OptionTypeEnum.EqUnit:
                        return _eqMappingList.Where(w => w.ProcessId == mainId).GroupBy(eq => new { eq.Unit, eq.UnitId })
                            .Select(s => new OptionEntity { Id = s.Key.UnitId, Value = s.Key.Unit }).ToList();
                    case OptionTypeEnum.EqUnitPart:
                        return _eqMappingList.Where(w => w.ProcessId == mainId && w.UnitId == subId).GroupBy(eq => new { eq.UnitPart, eq.UnitPartId })
                            .Select(s => new OptionEntity { Id = s.Key.UnitPartId, Value = s.Key.UnitPart }).ToList();
                    case OptionTypeEnum.EqProduct:
                        return GetEqProdOptionList(mainId);
                    default:
                        return new List<OptionEntity>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EqSituationMappingEntity> GetAllEqProcessOption()
        {
            try
            {
                var _catchEqMapping = CatchHelper.Get($"eqMappingList");
                List<EqSituationMappingDao> _eqMappingList = new List<EqSituationMappingDao>();

                if (_catchEqMapping == null)
                {
                    _eqMappingList = _eqSituationMappingRepository.SelectList();
                    CatchHelper.Set($"eqMappingList", JsonConvert.SerializeObject(_eqMappingList), 86400);
                }
                else
                {
                    _eqMappingList = JsonConvert.DeserializeObject<List<EqSituationMappingDao>>(_catchEqMapping);
                }

                return _eqMappingList.CopyAToB<EqSituationMappingEntity>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<OptionEntity> GetShiftOptionList()
            => new List<OptionEntity> {
                new OptionEntity { Id = 1, Value = "A" },
                new OptionEntity { Id = 2, Value = "B" },
                new OptionEntity { Id = 3, Value = "C" },
                new OptionEntity { Id = 4, Value = "D" }
            };

        public List<OptionEntity> GetPriorityOptionList()
            => new List<OptionEntity> {
                new OptionEntity { Id = 1, Value = "一般" },
                new OptionEntity { Id = 2, Value = "嚴重" },
                new OptionEntity { Id = 3, Value = "追蹤" }
            };

        public List<OptionEntity> GetDemandCategoryOptionList()
            => new List<OptionEntity> {
                new OptionEntity { Id = (int)DemandCategoryEnum.Setting, Value = "設定" },
                new OptionEntity { Id = (int)DemandCategoryEnum.NewItems, Value = "新增" },
                new OptionEntity { Id = (int)DemandCategoryEnum.Other, Value = "其它" }
            };

        public List<OptionEntity> GetEqEvenCodeOptionList(int typeId = 0, int yId = 0, int subyId = 0, int xId = 0, int subxId = 0, int rId = 0)
        {
            var _catchEqMapping = CatchHelper.Get($"eqEvenCodeList");
            List<EqEvanCodeMappingDao> _eqEvenCodeList = new List<EqEvanCodeMappingDao>();

            if (_catchEqMapping == null)
            {
                _eqEvenCodeList = _eqEvanCodeMappingRepository.SelectList();
                CatchHelper.Set($"eqEvenCodeList", JsonConvert.SerializeObject(_eqEvenCodeList), 86400);
            }
            else
            {
                _eqEvenCodeList = JsonConvert.DeserializeObject<List<EqEvanCodeMappingDao>>(_catchEqMapping);
            }

            if (subxId != 0)
            {
                _eqEvenCodeList =
                    _eqEvenCodeList.Where(w => w.TypeId == typeId && w.YId == yId && w.SubYId == subyId &&
                                               w.XId == xId && w.SubXId == subxId && w.RId != 0).ToList();
                var _tempOption = _eqEvenCodeList.GroupBy(eq => new { eq.R, eq.RId })
                        .Select(s => new OptionEntity { Id = s.Key.RId, Value = s.Key.R }).ToList();
                _tempOption.Add(new OptionEntity { Id = 99, Value = "Other" });
                return _tempOption;
            }
            else if (xId != 0)
            {
                _eqEvenCodeList =
                        _eqEvenCodeList.Where(w => w.TypeId == typeId && w.YId == yId && w.SubYId == subyId && w.XId == xId && w.SubXId != 0).ToList();
                var _tempOption = _eqEvenCodeList.GroupBy(eq => new { eq.SubXId, eq.SubX })
                                .Select(s => new OptionEntity { Id = s.Key.SubXId, Value = s.Key.SubX }).ToList();
                _tempOption.Add(new OptionEntity { Id = 99, Value = "Other" });
                return _tempOption;
            }
            else if (subyId != 0)
            {
                _eqEvenCodeList = _eqEvenCodeList.Where(w => w.TypeId == typeId && w.YId == yId && w.SubYId == subyId && w.XId != 0).ToList();
                var _tempOption = _eqEvenCodeList.GroupBy(eq => new { eq.X, eq.XId })
                                .Select(s => new OptionEntity { Id = s.Key.XId, Value = s.Key.X }).ToList();
                _tempOption.Add(new OptionEntity { Id = 99, Value = "Other" });
                return _tempOption;
            }
            else if (yId != 0)
            {
                _eqEvenCodeList = _eqEvenCodeList.Where(w => w.TypeId == typeId && w.YId == yId && w.SubYId != 0).ToList();
                var _tempOption = _eqEvenCodeList.GroupBy(eq => new { eq.SubY, eq.SubYId })
                            .Select(s => new OptionEntity { Id = s.Key.SubYId, Value = s.Key.SubY }).ToList();
                _tempOption.Add(new OptionEntity { Id = 99, Value = "Other" });
                return _tempOption;
            }
            else if (typeId != 0)
            {
                _eqEvenCodeList = _eqEvenCodeList.Where(w => w.TypeId == typeId && w.YId != 0).ToList();
                var _tempOption = _eqEvenCodeList.GroupBy(eq => new { eq.Y, eq.YId })
                            .Select(s => new OptionEntity { Id = s.Key.YId, Value = s.Key.Y }).ToList();
                _tempOption.Add(new OptionEntity { Id = 99, Value = "Other" });
                return _tempOption;
            }
            else
            {
                var _tempOption = _eqEvenCodeList.GroupBy(eq => new { eq.Type, eq.TypeId })
                            .Select(s => new OptionEntity { Id = s.Key.TypeId, Value = s.Key.Type }).ToList();
                _tempOption.Add(new OptionEntity { Id = 99, Value = "Other" });
                return _tempOption;
            }
        }

        public List<EqEvanCodeMappingEntity> GetAllEqEvenCodeOptionList()
        {
            var _catchEqMapping = CatchHelper.Get($"eqEvenCodeList");
            List<EqEvanCodeMappingDao> _eqEvenCodeList = new List<EqEvanCodeMappingDao>();

            if (_catchEqMapping == null)
            {
                _eqEvenCodeList = _eqEvanCodeMappingRepository.SelectList();
                CatchHelper.Set($"eqEvenCodeList", JsonConvert.SerializeObject(_eqEvenCodeList), 86400);
            }
            else
                _eqEvenCodeList = JsonConvert.DeserializeObject<List<EqEvanCodeMappingDao>>(_catchEqMapping);

            return _eqEvenCodeList.CopyAToB<EqEvanCodeMappingEntity>();
        }

        public List<EqEvanCodeMappingEntity> GetEqEvenCode(int typeId = 0, int yId = 0, int subyId = 0, int xId = 0, int subxId = 0, int rId = 0)
        {
            var _catchEqMapping = CatchHelper.Get($"eqEvenCodeList");
            List<EqEvanCodeMappingDao> _eqEvenCodeList = new List<EqEvanCodeMappingDao>();

            if (_catchEqMapping == null)
            {
                _eqEvenCodeList = _eqEvanCodeMappingRepository.SelectList();
                CatchHelper.Set($"eqEvenCodeList", JsonConvert.SerializeObject(_eqEvenCodeList), 86400);
            }
            else
            {
                _eqEvenCodeList = JsonConvert.DeserializeObject<List<EqEvanCodeMappingDao>>(_catchEqMapping);
            }

            if (typeId != 0 && typeId != 99)
                _eqEvenCodeList = _eqEvenCodeList.Where(w => w.TypeId == typeId).ToList();
            if (yId != 0 && yId != 99)
                _eqEvenCodeList = _eqEvenCodeList.Where(w => w.YId == yId).ToList();
            if (subyId != 0 && subyId != 99)
                _eqEvenCodeList = _eqEvenCodeList.Where(w => w.SubYId == subyId).ToList();
            if (xId != 0 && xId != 99)
                _eqEvenCodeList = _eqEvenCodeList.Where(w => w.XId == xId).ToList();
            if (subxId != 0 && subxId != 99)
                _eqEvenCodeList = _eqEvenCodeList.Where(w => w.SubXId == subxId).ToList();
            if (rId != 0 && rId != 99)
                _eqEvenCodeList = _eqEvenCodeList.Where(w => w.RId == rId).ToList();

            return _eqEvenCodeList.CopyAToB<EqEvanCodeMappingEntity>();
        }

        public List<(string, List<string>)> GetAreaEqGroupOptions()
        => _equipMappingRepository.SelectAll().GroupBy(gb => gb.AREA).Select(s => (s.Key, s.Select(ss => ss.EQUIP_NBR).ToList())).ToList();

        public List<(string, List<OptionEntity>)> GetAccessFabOptions()
        {
            return new List<(string, List<OptionEntity>)>
            {
                ("statusList",
                new List<OptionEntity>
                {
                    new OptionEntity { Id = (int)FabInOutStatusEnum.Processing, Value = FabInOutStatusEnum.Processing.GetDescription() },
                    new OptionEntity { Id = (int)FabInOutStatusEnum.Rejected, Value = FabInOutStatusEnum.Rejected.GetDescription() },
                    new OptionEntity { Id = (int)FabInOutStatusEnum.Completed, Value = FabInOutStatusEnum.Completed.GetDescription() },
                    new OptionEntity { Id = (int)FabInOutStatusEnum.Cancel, Value = FabInOutStatusEnum.Cancel.GetDescription() }
                } ),
                ("fabInTypeList",
                new List<OptionEntity>
                {
                    new OptionEntity { Id = (int)FabInTypeEnum.Visit, Value = FabInTypeEnum.Visit.GetDescription() },
                    new OptionEntity { Id = (int)FabInTypeEnum.Audit, Value = FabInTypeEnum.Audit.GetDescription() },
                    new OptionEntity { Id = (int)FabInTypeEnum.MoveIn, Value = FabInTypeEnum.MoveIn.GetDescription() },
                    new OptionEntity { Id = (int)FabInTypeEnum.Other, Value = FabInTypeEnum.Other.GetDescription() },
                } ),
                ("fabInCategoryList",
                new List<OptionEntity>
                {
                    new OptionEntity { Id = (int)FabInCategoryEnum.Guest, Value = FabInCategoryEnum.Guest.GetDescription() },
                    new OptionEntity { Id = (int)FabInCategoryEnum.OtherFab, Value = FabInCategoryEnum.OtherFab.GetDescription() }
                } )
            };
        }

        public List<OptionEntity> GetLevelOptionList()
            => new List<OptionEntity> {
                new OptionEntity { Id = (int)JobLevelEnum.FactoryManager, Value = JobLevelEnum.FactoryManager.GetDescription() },
                new OptionEntity { Id = (int)JobLevelEnum.DepartmentManager, Value = JobLevelEnum.DepartmentManager.GetDescription() },
                new OptionEntity { Id = (int)JobLevelEnum.SectionManager, Value = JobLevelEnum.SectionManager.GetDescription() },
                new OptionEntity { Id = (int)JobLevelEnum.Employee, Value = JobLevelEnum.Employee.GetDescription() },
            };

        public List<OptionEntity> GetDepartmentOptionList(int parentDeptId, int levelId)
        {
            var _catchDeptInfo = CatchHelper.Get($"deptList");

            List<OptionEntity> _deptOptionList = new List<OptionEntity>();
            List<DefinitionDepartmentDao> _allDepartmentList = new List<DefinitionDepartmentDao>();

            if (_catchDeptInfo == null)
            {
                _allDepartmentList = _accountInfoRepository.SelectDefinitionDepartment();
                CatchHelper.Set("deptList", JsonConvert.SerializeObject(_allDepartmentList), 604800);
            }
            else
                _allDepartmentList = JsonConvert.DeserializeObject<List<DefinitionDepartmentDao>>(_catchDeptInfo);

            if (levelId == 1)
                _deptOptionList = _allDepartmentList.Where(w => w.ParentDeptId == parentDeptId)
                    .Select(s => new OptionEntity
                    {
                        Id = s.DeptSn,
                        SubId = s.ParentDeptId,
                        Value = s.DepartmentName
                    }).ToList();
            else
                _deptOptionList = _allDepartmentList//.Where(w => w.LevelId == levelId && w.ParentDeptId == parentDeptId)
                    .Select(s => new OptionEntity
                    {
                        Id = s.DeptSn,
                        SubId = s.ParentDeptId,
                        Value = s.DepartmentName
                    }).ToList();

            return _deptOptionList;
        }

        public List<OptionEntity> GetMenuOptionList()
        {
            var _catchDeptInfo = CatchHelper.Get($"menuList");

            List<OptionEntity> _menuOptionList = new List<OptionEntity>();

            if (_catchDeptInfo == null)
            {
                var _allMenuList = _menuRepository.SelectAllMenu();

                CatchHelper.Set("menuList", JsonConvert.SerializeObject(_allMenuList), 604800);

                _menuOptionList = _allMenuList.Where(w => w.href != "#")
                    .Select(s => new OptionEntity
                    {
                        Id = s.sn,
                        Value = s.page_name
                    }).ToList();
            }
            else
            {
                _menuOptionList = JsonConvert.DeserializeObject<List<MenuInfoDao>>(_catchDeptInfo)
                    .Where(w => w.href != "#")
                    .Select(s => new OptionEntity
                    {
                        Id = s.sn,
                        Value = s.page_name
                    }).ToList();
            }

            return _menuOptionList;
        }

        public List<MenuPermissionViewModel> GetCreatePermissionList()
        {
            var _catchDeptInfo = CatchHelper.Get($"menuList");

            List<OptionEntity> _menuOptionList = new List<OptionEntity>();

            List<MenuPermissionViewModel> _response = new List<MenuPermissionViewModel>();

            if (_catchDeptInfo == null)
            {
                var _allMenuList = _menuRepository.SelectAllMenu();

                CatchHelper.Set("menuList", JsonConvert.SerializeObject(_allMenuList), 604800);

                _response = _allMenuList.Select(menu => new MenuPermissionViewModel
                {
                    IsMenuActive = false,
                    MenuId = (MenuEnum)menu.sn,
                    Menu = menu.page_name,
                    MenuActionList = EnumHelper.GetEnumValue<PermissionEnum>().Select(action =>
                    new MenuActionViewModel
                    {
                        IsActionActive = false,
                        ActionId = action,
                        Action = action.GetDescription()
                    }).ToList()
                }).ToList();
            }
            else
            {
                List<MenuInfoDao> _menuInfoList = JsonConvert.DeserializeObject<List<MenuInfoDao>>(_catchDeptInfo).Where(w => w.href != "#").ToList();
                _response = _menuInfoList.Select(menu => new MenuPermissionViewModel
                {
                    IsMenuActive = false,
                    MenuId = (MenuEnum)menu.sn,
                    Menu = menu.page_name,
                    MenuActionList = EnumHelper.GetEnumValue<PermissionEnum>().Select(action =>
                    new MenuActionViewModel
                    {
                        IsActionActive = false,
                        ActionId = action,
                        Action = action.GetDescription()
                    }).ToList()
                }).ToList();

            }

            return _response;
        }

        public List<(string, List<(int, string)>)> GetLcmProdOptions()
        {
            return _lcmProductRepository.SelectByConditions().GroupBy(g => g.ProdSize)
                .Select(prod => (prod.Key, prod.Select(p => (p.sn, $"{p.ProdNo}-{p.Descr}")).ToList()))
                .OrderBy(ob => ob.Key)
                .ToList();
        }

        public List<EqMappingEntity> GetEqIDAreaList()
        {
            return _equipMappingRepository.SelectEqByConditions(2).OrderBy(o => o.EQUIP_GROUP).CopyAToB<EqMappingEntity>();
        }

        public List<CertifiedAreaMappingEntity> GetCertifiedAreaOptions()
        {
            return _certifiedAreaMappingRepository.SelectByConditions().CopyAToB<CertifiedAreaMappingEntity>();
        }

        private List<OptionEntity> GetEqProdOptionList(int id)
        {
            switch (id)
            {
                case 1:
                    return new List<OptionEntity> {
                        new OptionEntity{Id = 1,Value = "LCD+OCA" },
                        new OptionEntity{Id = 2,Value = "TPM+LCD"},
                        new OptionEntity{Id = 3,Value = "SG+OCA"},
                        new OptionEntity{Id = 4,Value = "VCS SG"},
                    };
                case 2:
                    return new List<OptionEntity> {
                        new OptionEntity{Id = 1,Value = "PADI+LCD" },
                        new OptionEntity{Id = 2,Value = "CG+SG"},
                        new OptionEntity{Id = 3,Value = "SG"}
                    };
                case 3:
                    return new List<OptionEntity> {
                        new OptionEntity{Id = 1,Value = "OCA" },
                        new OptionEntity{Id = 2,Value = "TPM"},
                        new OptionEntity{Id = 3,Value = "FILM"}
                    };
                case 4:
                    return new List<OptionEntity> {
                        new OptionEntity{Id = 1,Value = "AZV" },
                        new OptionEntity{Id = 2,Value = "PADI"},
                        new OptionEntity{Id = 3,Value = "GM"}
                    };
                case 5:
                    return new List<OptionEntity> {
                        new OptionEntity{Id = 1,Value = "0" },
                        new OptionEntity{Id = 2,Value = "1"},
                        new OptionEntity{Id = 3,Value = "2"},
                        new OptionEntity{Id = 4,Value = "3"}
                    };
                default:
                    return new List<OptionEntity>();
            }
        }
    }
}
