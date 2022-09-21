using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using MOD4.Web.Helper;
using MOD4.Web.Repostory;
using MOD4.Web.Repostory.Dao;
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

        public OptionDomainService(IEqSituationMappingRepository eqSituationMappingRepository,
            IEqEvanCodeMappingRepository eqEvanCodeMappingRepository)
        {
            _eqSituationMappingRepository = eqSituationMappingRepository;
            _eqEvanCodeMappingRepository = eqEvanCodeMappingRepository;
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
