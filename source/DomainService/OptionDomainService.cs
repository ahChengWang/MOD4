using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using MOD4.Web.Helper;
using MOD4.Web.Repostory;
using MOD4.Web.Repostory.Dao;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MOD4.Web.DomainService
{
    public class OptionDomainService : IOptionDomainService
    {
        private readonly IEqSituationMappingRepository _eqSituationMappingRepository;

        public OptionDomainService(IEqSituationMappingRepository eqSituationMappingRepository)
        {
            _eqSituationMappingRepository = eqSituationMappingRepository;
        }
                

        public List<OptionEntity> GetOptionByType(OptionTypeEnum optionTypeId, int mainId = 0, int subId = 0)
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

        private List<OptionEntity> GetEqUnitOptionList(int id)
        {
            switch (id)
            {
                case 1:
                    return new List<OptionEntity> {
                        new OptionEntity{Id = 1,Value = "Loader" },
                        new OptionEntity{Id = 2,Value = "Clean"},
                        new OptionEntity{Id = 3,Value = "OLB"},
                        new OptionEntity{Id = 4,Value = "AOI"},
                        new OptionEntity{Id = 5,Value = "OLB Testing"},
                        new OptionEntity{Id = 6,Value = "DISP"},
                        new OptionEntity{Id = 7,Value = "PCB"},
                        new OptionEntity{Id = 8,Value = "PCBI"},
                        new OptionEntity{Id = 9,Value = "Unloader"}
                    };
                case 2:
                    return new List<OptionEntity> {
                        new OptionEntity{Id = 1,Value = "Loader" },
                        new OptionEntity{Id = 2,Value = "Clean"},
                        new OptionEntity{Id = 3,Value = "FOG"},
                        new OptionEntity{Id = 4,Value = "AOI"},
                        new OptionEntity{Id = 6,Value = "DISP"},
                        new OptionEntity{Id = 7,Value = "BT"},
                        new OptionEntity{Id = 8,Value = "VI"},
                        new OptionEntity{Id = 9,Value = "Unloader"}
                    };
                case 3:
                    return new List<OptionEntity> {
                        new OptionEntity{Id = 1,Value = "STH" },
                        new OptionEntity{Id = 2,Value = "STH API"},
                        new OptionEntity{Id = 3,Value = "貼膜機"},
                        new OptionEntity{Id = 4,Value = "乾擦機"},
                        new OptionEntity{Id = 5,Value = "HTH"},
                        new OptionEntity{Id = 6,Value = "HTH API"},
                        new OptionEntity{Id = 7,Value = "ACLV"},
                        new OptionEntity{Id = 8,Value = "PTVI"},
                        new OptionEntity{Id = 9,Value = "UVM"},
                        new OptionEntity{Id = 10,Value = "Unloader"}
                    };
                case 4:
                    return new List<OptionEntity> {
                        new OptionEntity{Id = 1,Value = "STH" },
                        new OptionEntity{Id = 2,Value = "STH API"},
                        new OptionEntity{Id = 5,Value = "HTH"},
                        new OptionEntity{Id = 6,Value = "HTH API"},
                        new OptionEntity{Id = 7,Value = "ACLV"},
                        new OptionEntity{Id = 8,Value = "PTVI"},
                        new OptionEntity{Id = 9,Value = "UVM"},
                        new OptionEntity{Id = 10,Value = "Unloader"}
                    };
                case 5:
                    return new List<OptionEntity> {
                        new OptionEntity{Id = 1,Value = "STH" },
                        new OptionEntity{Id = 2,Value = "STH API"},
                        new OptionEntity{Id = 3,Value = "Unloader"}
                    };
                case 6:
                    return new List<OptionEntity> {
                        new OptionEntity{Id = 1,Value = "STH" },
                        new OptionEntity{Id = 2,Value = "STH API"},
                        new OptionEntity{Id = 3,Value = "Unloader"}
                    };
                case 7:
                    return new List<OptionEntity> {
                        new OptionEntity{Id = 1,Value = "覆膜乾擦機" },
                        new OptionEntity{Id = 2,Value = "HTH"},
                        new OptionEntity{Id = 3,Value = "HTH API"},
                        new OptionEntity{Id = 4,Value = "ACLV"},
                        new OptionEntity{Id = 5,Value = "PTVI"},
                        new OptionEntity{Id = 6,Value = "UVM"},
                        new OptionEntity{Id = 7,Value = "Unloader"}
                    };
                default:
                    return new List<OptionEntity>();
            }
        }

        private List<OptionEntity> GetEqUnitPartOptionList(int parentId, int id)
        {
            switch (id)
            {
                case 1:
                    return new List<OptionEntity> {
                        new OptionEntity{Id = 1,Value = "Loader" }
                    };
                case 2:
                    return new List<OptionEntity> {
                        new OptionEntity{Id = 1,Value = "Clean" }
                    };
                case 3:
                    return new List<OptionEntity> {
                        new OptionEntity{Id = 1,Value = "OLB1-ACF" },
                        new OptionEntity{Id = 2,Value = "OLB1-PreB"},
                        new OptionEntity{Id = 3,Value = "OLB1-MB1"}
                    };
                case 4:
                    return new List<OptionEntity> {
                        new OptionEntity{Id = 1,Value = "AOI" },
                        new OptionEntity{Id = 2,Value = "AOI-Stage1"},
                        new OptionEntity{Id = 3,Value = "AOI-Stage2"},
                        new OptionEntity{Id = 4,Value = "AOI-Stage3"}
                    };
                case 5:
                    return new List<OptionEntity> {
                        new OptionEntity{Id = 1,Value = "OLB-Testing" }
                    };
                case 6:
                    return new List<OptionEntity> {
                        new OptionEntity{Id = 1,Value = "DISP1-B-ME" }
                    };
                case 7:
                    return new List<OptionEntity> {
                        new OptionEntity{Id = 1,Value = "PCB1-LD" },
                        new OptionEntity{Id = 2,Value = "PCB1-ACF"}
                    };
                case 8:
                    return new List<OptionEntity> {
                        new OptionEntity{Id = 1,Value = "PCB1" },
                        new OptionEntity{Id = 2,Value = "PCB1-API"}
                    };
                case 9:
                    return new List<OptionEntity> {
                        new OptionEntity{Id = 1,Value = "Unloader" }
                    };
                default:
                    return new List<OptionEntity>();
            }
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
