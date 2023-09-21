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

namespace MOD4.Web.DomainService
{
    public class PCESCertificationDomainService : BaseDomainService, IPCESCertificationDomainService
    {
        private readonly IPCESCertificationRepository _pcesCertificationRepository;

        public PCESCertificationDomainService(IPCESCertificationRepository pcesCertificationRepository)
        {
            _pcesCertificationRepository = pcesCertificationRepository;
        }

        public List<PCESCertRecordEntity> GetRecordPCES(string opr, string station, string prod, string status, string jobId)
        {
            try
            {

                List<PCESCertRecordEntity> _pcesCertRecordEntityList = new List<PCESCertRecordEntity>();

                var _pcesCertRecordList = _pcesCertificationRepository.SelectByConditions(
                    oprList: opr?.Split(",").ToList(),
                    stationList: station?.Split(",").ToList(),
                    certStatusList: status?.Split(",").ToList(),
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
                            StatusId = cert.status,
                            Status = cert.status.GetDescription(),
                            SubjGrade = cert.subj_grade ?? 0,
                            //PassDate = cert.pass_date != null ? ((DateTime)cert.pass_date).ToString("yyyy/MM/dd") : "",
                            //ValidDate = cert.valid_date != null ? ((DateTime)cert.valid_date).ToString("yyyy/MM/dd") : "",
                            SkillGrade = cert.skill_grade ?? 0,
                            SkillStatus = cert.skill_status != null ? ((CertStatusEnum)cert.skill_status).GetDescription() : "",
                            EngNo = cert.eng_no ?? "",
                            EngName = cert.eng_name ?? "",
                            Remark = cert.remark ?? ""
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
                            Remark = cert.remark ?? ""
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

                return _pcesCertRecordEntityList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
