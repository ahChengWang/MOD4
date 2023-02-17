using Microsoft.AspNetCore.Http;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using System;
using System.IO;
using Ionic.Zip;
using MOD4.Web.Repostory;
using System.Collections.Generic;
using MOD4.Web.Repostory.Dao;
using MOD4.Web.Helper;
using System.Linq;
using System.Globalization;

namespace MOD4.Web.DomainService
{
    public class SPCReportDomainService : ISPCReportDomainService
    {
        private readonly ISPCMicroScopeDataRepository _spcMicroScopeDataRepository;

        public SPCReportDomainService(ISPCMicroScopeDataRepository spcMicroScopeDataRepository)
        {
            _spcMicroScopeDataRepository = spcMicroScopeDataRepository;
        }

        public List<SPCMainEntity> Search(string dateRange, string equpId, string prodId, string dataGroup)
        {
            try
            {
                var _defaultSetting = new
                {
                    OOS = 0.05,
                    OOC = 0.02,
                    OOR = 0.04,
                };
                DateTime _startDate;
                DateTime _endDate;

                if (string.IsNullOrEmpty(dateRange))
                {
                    throw new Exception("日期異常");
                }

                string[] _dateAry = dateRange.Split("-");

                if (!DateTime.TryParseExact(_dateAry[0].Trim(), "yyyy/MM/dd HH:mm", null, DateTimeStyles.None, out _) ||
                    !DateTime.TryParseExact(_dateAry[1].Trim(), "yyyy/MM/dd HH:mm", null, DateTimeStyles.None, out _))
                {
                    throw new Exception("日期異常");
                }
                DateTime.TryParseExact(_dateAry[0].Trim(), "yyyy/MM/dd HH:mm", null, DateTimeStyles.None, out _startDate);
                DateTime.TryParseExact(_dateAry[1].Trim(), "yyyy/MM/dd HH:mm", null, DateTimeStyles.None, out _endDate);

                List<SPCMicroScopeDataDao> _spcMicroScopeDataList = _spcMicroScopeDataRepository.SelectByConditions(equpId, _startDate, _endDate, prodId, dataGroup);

                var _spcGroupData = _spcMicroScopeDataList.GroupBy(g => new { g.EquipmentId, g.ProductId, g.DataGroup }).Select(s => new SPCMainEntity
                {
                    EquipmentId = s.Key.EquipmentId,
                    ProductId = s.Key.ProductId,
                    DataGroup = s.Key.DataGroup,
                    Count = s.Count(),
                    OOCCount = s.Where(w => w.DTX > _defaultSetting.OOC || (w.DTX < _defaultSetting.OOC * -1)).Count(),
                    OOSCount = s.Where(w => w.DTX > _defaultSetting.OOS || (w.DTX < _defaultSetting.OOS * -1)).Count(),
                    OORCount = s.Where(w => w.DTX > _defaultSetting.OOR || (w.DTX < _defaultSetting.OOR * -1)).Count(),
                }).ToList();

                SPCOnlineChartEntity _spcOnlineChartEntity = new SPCOnlineChartEntity
                {
                    ChartId = dataGroup,
                    TypeStr = "XXRM",
                    TestItem = dataGroup,
                    XBarBar = "0.40604",
                    Sigma = "0.00959",
                    Ca = "1.000",
                    Cp = "17386767844",
                    Cpk = "7.1646",
                    Sample = "78",
                    n = "1",
                    RMBar = "0.01818",
                    PpkBar = "0.40604",
                    PpkSigma = "1.083488881217",
                    Pp = "45382706736899.6",
                    Ppk = "6.33878216814284",
                    DetailList = _spcMicroScopeDataList.CopyAToB<SPCMicroScopeDataEntity>()
                };

                return _spcGroupData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SPCOnlineChartEntity Detail(string dateRange, string equpId, string prodId, string dataGroup)
        {
            try
            {
                double _lastDTX = 0;

                DateTime _startDate;
                DateTime _endDate;

                if (string.IsNullOrEmpty(dateRange))
                {
                    throw new Exception("日期異常");
                }

                string[] _dateAry = dateRange.Split("-");

                if (!DateTime.TryParseExact(_dateAry[0], "yyyy/MM/dd HH:mm", null, DateTimeStyles.None, out _) ||
                    !DateTime.TryParseExact(_dateAry[1], "yyyy/MM/dd HH:mm", null, DateTimeStyles.None, out _))
                {
                    throw new Exception("日期異常");
                }
                DateTime.TryParseExact(_dateAry[0], "yyyy/MM/dd HH:mm", null, DateTimeStyles.None, out _startDate);
                DateTime.TryParseExact(_dateAry[0], "yyyy/MM/dd HH:mm", null, DateTimeStyles.None, out _endDate);


                List<SPCMicroScopeDataDao> _spcMicroScopeDataList = _spcMicroScopeDataRepository.SelectByConditions(equpId, _startDate, _endDate, prodId, dataGroup);

                _spcMicroScopeDataList.ForEach(fe =>
                {
                    if (_lastDTX == 0)
                    {
                        fe.DTRM = fe.DTX;
                        _lastDTX = fe.DTX;
                    }
                    else
                    {
                        fe.DTRM = Math.Abs(fe.DTX - _lastDTX);
                        _lastDTX = fe.DTX;
                    }
                    fe.Target = 1.23;
                    fe.USL = 1.6;
                    fe.UCL1 = 1.4;
                    fe.CL1 = 1.3;
                    fe.LCL1 = -1.4;
                    fe.LSL = -1.6;
                    fe.UCL2 = 15000000;
                    fe.CL2 = 8000000;
                    fe.LCL2 = -15000000;
                });

                SPCOnlineChartEntity _spcOnlineChartEntity = new SPCOnlineChartEntity
                {
                    ChartId = dataGroup,
                    TypeStr = "XXRM",
                    TestItem = dataGroup,
                    XBarBar = "0.40604",
                    Sigma = "0.00959",
                    Ca = "1.000",
                    Cp = "17386767844",
                    Cpk = "7.1646",
                    Sample = "78",
                    n = "1",
                    RMBar = "0.01818",
                    PpkBar = "0.40604",
                    PpkSigma = "1.083488881217",
                    Pp = "45382706736899.6",
                    Ppk = "6.33878216814284",
                    DetailList = _spcMicroScopeDataList.CopyAToB<SPCMicroScopeDataEntity>()
                };

                return _spcOnlineChartEntity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
