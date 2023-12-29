using MOD4.Web.Repostory;
using System;
using System.Collections.Generic;
using System.Net.Http;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using System.Linq;
using MOD4.Web.DomainService.Entity;
using Utility.Helper;
using MOD4.Web.Enum;
using MOD4.Web.Repostory.Dao;
using System.Transactions;
using Newtonsoft.Json;

namespace MOD4.Web.DomainService
{
    public class PCESCertificationDomainService : BaseDomainService, IPCESCertificationDomainService
    {
        private readonly IPCESCertificationRepository _pcesCertificationRepository;
        private readonly string[] _generalClassDic = new string[] { "IATF 16949", "IECQ QC 080000", "TISAX Prototype保護認知", "VDA6.3", "品質與安全意識", "管制計畫", "靜電防護", "交通安全", "勞工安全衛生", "危害通識", "友善職場", "自傷危機干預" };

        public PCESCertificationDomainService(IPCESCertificationRepository pcesCertificationRepository)
        {
            _pcesCertificationRepository = pcesCertificationRepository;
        }

        public List<PCESCertRecordEntity> GetRecordPCES(string opr, string station, string prod, string certStatus, string jobId, string status = null)
        {
            try
            {

                List<PCESCertRecordEntity> _pcesCertRecordEntityList = new List<PCESCertRecordEntity>();

                var _pcesCertRecordList = _pcesCertificationRepository.SelectByConditions(
                    oprList: opr?.Split(",").ToList() ?? null,
                    stationList: station?.Split(",").ToList() ?? null,
                    certStatusList: certStatus?.Split(",").ToList() ?? null,
                    statusList: status?.Split(",").ToList() ?? null,
                    jobId: jobId);

                if (string.IsNullOrEmpty(prod))
                    _pcesCertRecordEntityList = _pcesCertRecordList.Select(cert =>
                    {
                        PCESCertRecordEntity _tmpPCES = new PCESCertRecordEntity
                        {
                            ApplyNo = cert.apply_no,
                            ApplyName = cert.apply_name,
                            Shift = cert.shift,
                            MainOperation = cert.main_oper,
                            Station = cert.station,
                            StationType = cert.type,
                            Mtype = cert.mtype,
                            ClassName = cert.class_name,
                            LicType = cert.lic_type,
                            CertStatusId = cert.certStatus,
                            CertStatus = cert.certStatus.GetDescription(),
                            StatusId = cert.status,
                            Status = cert.status.GetDescription(),
                            SubjGrade = cert.subj_grade ?? 0,
                            //PassDate = cert.pass_date != null ? ((DateTime)cert.pass_date).ToString("yyyy/MM/dd") : "",
                            //ValidDate = cert.valid_date != null ? ((DateTime)cert.valid_date).ToString("yyyy/MM/dd") : "",
                            SkillGrade = cert.skill_grade ?? 0,
                            SkillStatusId = cert.skill_status,
                            SkillStatus = cert.skill_status != null ? ((CertStatusEnum)cert.skill_status).GetDescription() : "",
                            EngNo = cert.eng_no ?? "",
                            EngName = cert.eng_name ?? "",
                            Remark = cert.remark ?? "",
                            IsGeneealClass = _generalClassDic.Contains(cert.class_name)
                        };

                        if (_tmpPCES.CertStatusId == CertStatusEnum.Pass || _tmpPCES.CertStatusId == CertStatusEnum.Expied)
                        {
                            _tmpPCES.PassDate = cert.pass_date != null ? ((DateTime)cert.pass_date).ToString("yyyy/MM/dd") : "";
                            _tmpPCES.ValidDate = cert.valid_date != null ? ((DateTime)cert.valid_date).ToString("yyyy/MM/dd") : "";
                        }

                        return _tmpPCES;

                    }).ToList();
                else
                    _pcesCertRecordEntityList = _pcesCertRecordList.Where(w => w.class_name.Split("-")[0].Contains(prod)).Select(cert =>
                    {
                        PCESCertRecordEntity _tmpPCES = new PCESCertRecordEntity
                        {
                            ApplyNo = cert.apply_no,
                            ApplyName = cert.apply_name,
                            Shift = cert.shift,
                            MainOperation = cert.main_oper,
                            Station = cert.station,
                            StationType = cert.type,
                            Mtype = cert.mtype,
                            ClassName = cert.class_name,
                            LicType = cert.lic_type,
                            CertStatusId = cert.certStatus,
                            StatusId = cert.status,
                            Status = cert.status.GetDescription(),
                            SubjGrade = cert.subj_grade ?? 0,
                            //PassDate = cert.pass_date != null ? ((DateTime)cert.pass_date).ToString("yyyy/MM/dd") : "",
                            //ValidDate = cert.valid_date != null ? ((DateTime)cert.valid_date).ToString("yyyy/MM/dd") : "",
                            SkillGrade = cert.skill_grade ?? 0,
                            SkillStatus = cert.skill_status != null ? ((CertStatusEnum)cert.skill_status).GetDescription() : "",
                            EngNo = cert.eng_no ?? "",
                            EngName = cert.eng_name ?? "",
                            Remark = cert.remark ?? "",
                            IsGeneealClass = _generalClassDic.Contains(cert.class_name)
                        };

                        if (_tmpPCES.CertStatusId == CertStatusEnum.Pass || _tmpPCES.CertStatusId == CertStatusEnum.Expied)
                        {
                            _tmpPCES.PassDate = cert.pass_date != null ? ((DateTime)cert.pass_date).ToString("yyyy/MM/dd") : "";
                            _tmpPCES.ValidDate = cert.valid_date != null ? ((DateTime)cert.valid_date).ToString("yyyy/MM/dd") : "";
                        }

                        //_tmpPCES.Status = _tmpPCES.StatusId.GetDescription();

                        //if (_tmpPCES.StatusId == CertStatusEnum.Pass)
                        //{
                        //    _tmpPCES.PassDate = cert.pass_date != null ? ((DateTime)cert.pass_date).ToString("yyyy/MM/dd") : "";
                        //    _tmpPCES.ValidDate = cert.valid_date != null ? ((DateTime)cert.valid_date).ToString("yyyy/MM/dd") : "";
                        //    _tmpPCES.SkillGrade = cert.skill_grade ?? 0;
                        //    _tmpPCES.SkillStatus = cert.skill_status != null ? ((CertStatusEnum)cert.skill_status).GetDescription() : "";
                        //    _tmpPCES.EngNo = cert.eng_no ?? "";
                        //    _tmpPCES.EngName = cert.eng_name ?? "";
                        //    _tmpPCES.Remark = cert.remark ?? "";
                        //}

                        return _tmpPCES;

                    }).ToList();

                return _pcesCertRecordEntityList.OrderBy(ob => ob.ApplyNo).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string UpdateCertSkill(PCESCertRecordEntity updCertEntity, UserEntity userInfo)
        {
            try
            {
                string _updRes = "";

                var _origPCESCertList = _pcesCertificationRepository.SelectByConditions(
                        oprList: new List<string> { updCertEntity.MainOperation },
                        stationList: new List<string> { updCertEntity.Station },
                        jobId: updCertEntity.ApplyNo,
                        className: updCertEntity.ClassName,
                        mtype: updCertEntity.Mtype,
                        licType: updCertEntity.LicType);

                if (_origPCESCertList == null || _origPCESCertList.Count() > 1)
                    return "查無認證紀錄";

                var _origPCESCertData = _origPCESCertList.FirstOrDefault();

                PCESCertificationRecordDao _updPCESDao = new PCESCertificationRecordDao
                {
                    apply_no = _origPCESCertData.apply_no,
                    main_oper = _origPCESCertData.main_oper,
                    station = _origPCESCertData.station,
                    type = _origPCESCertData.type,
                    mtype = _origPCESCertData.mtype,
                    class_name = _origPCESCertData.class_name,
                    lic_type = _origPCESCertData.lic_type,
                    skill_grade = updCertEntity.SkillGrade,
                    skill_status = updCertEntity.SkillStatusId,
                    eng_no = updCertEntity.EngNo,
                    eng_name = updCertEntity.EngName,
                    remark = updCertEntity.Remark,
                };

                if (_origPCESCertData.status == CertStatusEnum.Pass && _updPCESDao.skill_status == CertStatusEnum.Pass)
                    _updPCESDao.certStatus = CertStatusEnum.Pass;
                else
                    _updPCESDao.certStatus = CertStatusEnum.Failed;

                using (TransactionScope scope = new TransactionScope())
                {
                    if (_pcesCertificationRepository.UpdateCertSkill(_updPCESDao) == 1)
                        scope.Complete();
                    else
                        _updRes = "更新異常";
                }

                return _updRes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public (bool, string) UpdatePCESRecord()
        {
            try
            {
                bool _syncStatus = false;
                string _syncRes = "";

                List<PCESCertificationRawDataDao> _pcesRawData = GetAllPCESRawData();

                List<PCESCertificationRecordDao> _updExpDatePCESCertRecords = new List<PCESCertificationRecordDao>();
                List<PCESCertificationRecordDao> _insExpDatePCESCertRecords = new List<PCESCertificationRecordDao>();
                List<PCESCertificationRecordDao> _pcesCertRecordList = _pcesCertificationRepository.SelectByConditions();
                List<PCESCertificationRawDataDao> _pcesCertRawData = new List<PCESCertificationRawDataDao>();

                // 已通過認證
                List<PCESCertificationRecordDao> _pcesCertRawPass = _pcesRawData
                        .Select(cert => new PCESCertificationRecordDao
                        {
                            apply_no = cert.apply_no,
                            apply_name = cert.apply_name,
                            shift = cert.shift,
                            main_oper = cert.main_oper,
                            station = cert.station,
                            type = cert.type,
                            mtype = cert.mtype,
                            class_name = cert.class_name,
                            lic_type = cert.lic_type,
                            status = System.Enum.GetValues(typeof(CertStatusEnum)).Cast<CertStatusEnum>().FirstOrDefault(f => f.GetDescription() == cert.status),
                            pass_date = cert.pass_date,
                            valid_date = cert.valid_date,
                            subj_grade = cert.subj_grade,
                            eng_name = cert.eng_name,
                            eng_no = cert.eng_no,
                            skill_grade = cert.skill_grade,
                            skill_status = cert.skill_status
                        }).ToList();

                // 認證狀態變更 (認證中 => 通過、通過 => 過期)
                _updExpDatePCESCertRecords = (from raw in _pcesRawData
                                              join record in _pcesCertRecordList
                                             on new { raw.apply_no, raw.apply_name, raw.main_oper, raw.station, raw.mtype, raw.class_name, raw.lic_type }
                                             equals new { record.apply_no, record.apply_name, record.main_oper, record.station, record.mtype, record.class_name, record.lic_type }
                                              where raw.status != record.status.GetDescription()
                                              select new PCESCertificationRecordDao
                                              {
                                                  apply_no = raw.apply_no,
                                                  apply_name = raw.apply_name,
                                                  shift = raw.shift,
                                                  main_oper = raw.main_oper,
                                                  station = raw.station,
                                                  type = raw.type,
                                                  mtype = raw.mtype,
                                                  class_name = raw.class_name,
                                                  lic_type = raw.lic_type,
                                                  status = System.Enum.GetValues(typeof(CertStatusEnum)).Cast<CertStatusEnum>().FirstOrDefault(f => f.GetDescription() == raw.status),
                                                  pass_date = raw.pass_date,
                                                  valid_date = raw.valid_date,
                                                  subj_grade = raw.subj_grade,
                                                  eng_name = raw.eng_name,
                                                  eng_no = raw.eng_no,
                                                  skill_grade = raw.skill_grade
                                              }).ToList();

                _updExpDatePCESCertRecords.ForEach(c =>
                {
                    c.certStatus = c.status;

                    if (!_generalClassDic.Contains(c.class_name))
                        c.skill_status = c.status;

                    //switch (c.status)
                    //{
                    //    case CertStatusEnum.Pass when _generalClassDic.Contains(c.class_name):
                    //        c.certStatus = CertStatusEnum.Pass;
                    //        break;
                    //    case CertStatusEnum.Pass:
                    //        //c.certStatus = CertStatusEnum.InProgress;
                    //        c.certStatus = CertStatusEnum.Pass;
                    //        break;
                    //    case CertStatusEnum.Failed:
                    //        c.certStatus = CertStatusEnum.Failed;
                    //        //c.skill_grade = null;
                    //        //c.eng_no = null;
                    //        //c.skill_status = null;
                    //        //c.remark = null;
                    //        break;
                    //    case CertStatusEnum.InProgress:
                    //        c.certStatus = CertStatusEnum.InProgress;
                    //        //c.skill_grade = null;
                    //        //c.eng_no = null;
                    //        //c.skill_status = null;
                    //        //c.remark = null;
                    //        break;
                    //    case CertStatusEnum.Expied:
                    //        c.certStatus = CertStatusEnum.Expied;
                    //        //c.skill_grade = null;
                    //        //c.eng_no = null;
                    //        //c.skill_status = null;
                    //        //c.remark = null;
                    //        break;
                    //    default:
                    //        break;
                    //}
                });


                // 新認證
                _insExpDatePCESCertRecords = (from raw in _pcesRawData
                                              join record in _pcesCertRecordList
                                             on new { raw.apply_no, raw.apply_name, raw.main_oper, raw.station, raw.mtype, raw.class_name, raw.lic_type }
                                             equals new { record.apply_no, record.apply_name, record.main_oper, record.station, record.mtype, record.class_name, record.lic_type } into r
                                              from record in r.DefaultIfEmpty()
                                              where record is null
                                              select new PCESCertificationRecordDao
                                              {
                                                  apply_no = raw.apply_no,
                                                  apply_name = raw.apply_name,
                                                  shift = raw.shift,
                                                  main_oper = raw.main_oper,
                                                  station = raw.station,
                                                  type = raw.type,
                                                  mtype = raw.mtype,
                                                  class_name = raw.class_name,
                                                  lic_type = raw.lic_type,
                                                  certStatus = CertStatusEnum.InProgress,
                                                  status = System.Enum.GetValues(typeof(CertStatusEnum)).Cast<CertStatusEnum>().FirstOrDefault(f => f.GetDescription() == raw.status),
                                                  pass_date = raw.pass_date,
                                                  valid_date = raw.valid_date,
                                                  subj_grade = raw.subj_grade,
                                                  eng_name = raw.eng_name,
                                                  eng_no = raw.eng_no,
                                                  skill_grade = raw.skill_grade
                                              }).ToList();

                _insExpDatePCESCertRecords.ForEach(c =>
                {
                    c.certStatus = c.status;

                    if (!_generalClassDic.Contains(c.class_name))
                        c.skill_status = c.status;

                    //switch (c.status)
                    //{
                    //    case CertStatusEnum.Pass when _generalClassDic.Contains(c.class_name):
                    //        c.certStatus = CertStatusEnum.Pass;
                    //        break;
                    //    case CertStatusEnum.Pass:
                    //        //c.certStatus = CertStatusEnum.InProgress;
                    //        c.certStatus = CertStatusEnum.Pass;
                    //        break;
                    //    case CertStatusEnum.Failed:
                    //        c.certStatus = CertStatusEnum.Failed;
                    //        //c.skill_grade = null;
                    //        //c.eng_no = null;
                    //        //c.skill_status = null;
                    //        //c.remark = null;
                    //        break;
                    //    case CertStatusEnum.InProgress:
                    //        c.certStatus = CertStatusEnum.InProgress;
                    //        //c.skill_grade = null;
                    //        //c.eng_no = null;
                    //        //c.skill_status = null;
                    //        //c.remark = null;
                    //        break;
                    //    case CertStatusEnum.Expied:
                    //        c.certStatus = CertStatusEnum.Expied;
                    //        //c.skill_grade = null;
                    //        //c.eng_no = null;
                    //        //c.skill_status = null;
                    //        //c.remark = null;
                    //        break;
                    //    default:
                    //        break;
                    //}
                });

                using (TransactionScope _scope = new TransactionScope())
                {
                    if (_pcesCertificationRepository.InsertPCESRaw(_pcesRawData) == _pcesRawData.Count &&
                        _pcesCertificationRepository.InsertPCESRecord(_insExpDatePCESCertRecords) == _insExpDatePCESCertRecords.Count &&
                        _pcesCertificationRepository.UpdatePCESRecord(_updExpDatePCESCertRecords) == _updExpDatePCESCertRecords.Count)
                    {
                        _scope.Complete();
                        _syncRes = $"raw data 筆數：{_pcesRawData.Count}, 更新紀錄筆數：{_updExpDatePCESCertRecords.Count}";
                        _syncStatus = true;
                    }
                    else
                        _syncRes = "資料更新異常";
                }

                return (_syncStatus, _syncRes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<PCESCertificationRawDataDao> GetAllPCESRawData()
        {
            try
            {
                var _request = new HttpRequestMessage(HttpMethod.Get, "http://pmfgform/pces_api/api/Data/GetPCESApplyCV?fab=M4");

                List<PCESCertificationRawDataDao> _response = new List<PCESCertificationRawDataDao>();

                using (HttpClient client = new HttpClient())
                {
                    var _res = client.SendAsync(_request).Result;

                    using (HttpContent content = _res.Content)
                    {
                        _response = JsonConvert.DeserializeObject<List<PCESCertificationRawDataDao>>(content.ReadAsStringAsync().Result);
                    }
                }

                return _response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
