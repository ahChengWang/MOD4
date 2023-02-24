﻿using MOD4.Web.DomainService.Entity;
using MOD4.Web.Helper;
using MOD4.Web.Repostory;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MOD4.Web.DomainService
{
    public class SPCReportDomainService : ISPCReportDomainService
    {
        private readonly ISPCMicroScopeDataRepository _spcMicroScopeDataRepository;
        private readonly ISPCChartSettingRepository _spcChartSettingRepository;

        public SPCReportDomainService(ISPCMicroScopeDataRepository spcMicroScopeDataRepository,
            ISPCChartSettingRepository spcChartSettingRepository)
        {
            _spcMicroScopeDataRepository = spcMicroScopeDataRepository;
            _spcChartSettingRepository = spcChartSettingRepository;
        }

        public List<SPCMainEntity> Search(int floor, string chartgrade, string dateRange, string equpId, string prodId, string dataGroup)
        {
            try
            {
                DateTime _startDate;
                DateTime _endDate;

                if (string.IsNullOrEmpty(dateRange))
                    throw new Exception("日期異常");

                string[] _dateAry = dateRange.Split("-");

                if (!DateTime.TryParseExact(_dateAry[0].Trim(), "yyyy/MM/dd HH:mm", null, DateTimeStyles.None, out _) ||
                    !DateTime.TryParseExact(_dateAry[1].Trim(), "yyyy/MM/dd HH:mm", null, DateTimeStyles.None, out _))
                {
                    throw new Exception("日期異常");
                }
                DateTime.TryParseExact(_dateAry[0].Trim(), "yyyy/MM/dd HH:mm", null, DateTimeStyles.None, out _startDate);
                DateTime.TryParseExact(_dateAry[1].Trim(), "yyyy/MM/dd HH:mm", null, DateTimeStyles.None, out _endDate);

                List<SPCMicroScopeDataDao> _spcMicroScopeDataList = _spcMicroScopeDataRepository.SelectByConditions(equpId, _startDate, _endDate, prodId, dataGroup);
                var _spcSetting = _spcChartSettingRepository.SelectByConditions(chartgrade, floor,
                        _spcMicroScopeDataList.Select(s => s.DataGroup).Distinct().ToList(),
                        _spcMicroScopeDataList.Select(s => s.ProductId).Distinct().ToList(),
                        _spcMicroScopeDataList.Select(s => s.EquipmentId).Distinct().ToList());

                _spcMicroScopeDataList.ForEach(spc =>
                {
                    var _tmpSetting = _spcSetting.FirstOrDefault(f => f.PECD == spc.ProductId && f.PEQPT_ID == spc.EquipmentId && f.DataGroup == spc.DataGroup);
                    if (_tmpSetting == null)
                        return;
                    spc.OOC1 = spc.DTX > _tmpSetting.UCL1 || spc.DTX < _tmpSetting.LCL1;
                    spc.OOC2 = spc.DTRM > _tmpSetting.UCL2 || spc.DTX < _tmpSetting.LCL2;
                    spc.OOS = spc.DTX > _tmpSetting.USPEC || spc.DTX < _tmpSetting.LSPEC;
                });

                var _spcGroupData = _spcMicroScopeDataList.GroupBy(g => new { g.EquipmentId, g.ProductId, g.DataGroup }).Select(s => new SPCMainEntity
                {
                    EquipmentId = s.Key.EquipmentId,
                    ProductId = s.Key.ProductId,
                    DataGroup = s.Key.DataGroup,
                    Count = s.Count(),
                    OOCCount = s.Where(w => w.OOC1).Count() + s.Where(w => w.OOC2).Count(),
                    OOSCount = s.Where(w => w.OOS).Count(),
                    OORCount = s.Where(w => w.OOR1).Count() + s.Where(w => w.OOR2).Count()
                }).ToList();

                return _spcGroupData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SPCOnlineChartEntity Detail(int floor, string chartgrade, string dateRange, string equpId, string prodId, string dataGroup)
        {
            try
            {
                float _lastDTX = 0;

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
                var _spcSetting = _spcChartSettingRepository.SelectByConditions(chartgrade, floor,
                        _spcMicroScopeDataList.Select(s => s.DataGroup).ToList(),
                        _spcMicroScopeDataList.Select(s => s.ProductId).ToList(),
                        _spcMicroScopeDataList.Select(s => s.EquipmentId).ToList()).FirstOrDefault();

                if (_spcSetting == null)
                    throw new Exception("查無 SPC 設定");

                float _sumDTX = _spcMicroScopeDataList.Sum(x => x.DTX);
                float _sumDTRM = _spcMicroScopeDataList.Sum(x => x.DTRM);
                int _allCnt = _spcMicroScopeDataList.Count;
                var _xBar = _sumDTRM / _allCnt; // DTX 的平均值
                var _rmBar = _sumDTRM / _allCnt; // DTRM 的平均值
                var _sigma = _rmBar / 1.128;
                double _calculateS = 0;

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
                    fe.Target = _spcSetting.Target;
                    fe.USL = _spcSetting.USPEC;
                    fe.LSL = _spcSetting.LSPEC;
                    fe.UCL1 = _spcSetting.UCL1;
                    fe.CL1 = _spcSetting.CL1;
                    fe.LCL1 = _spcSetting.LCL1;
                    fe.OOC1 = fe.DTX > _spcSetting.UCL1 || fe.DTX < _spcSetting.LCL1;
                    fe.OOS = fe.DTX > _spcSetting.USPEC || fe.DTX < _spcSetting.LSPEC;
                    fe.UCL2 = _spcSetting.UCL2;
                    fe.CL2 = _spcSetting.CL2;
                    fe.LCL2 = _spcSetting.LCL2;
                    fe.OOC2 = fe.DTRM > _spcSetting.UCL2 || fe.DTRM < _spcSetting.LCL2;

                    _calculateS += Math.Pow(fe.DTX - _xBar, 2);
                });

                var _sVal = Math.Sqrt(_calculateS / (_allCnt - 1));

                SPCOnlineChartEntity _spcOnlineChartEntity = new SPCOnlineChartEntity
                {
                    ChartId = _spcSetting.ONCHID,
                    TypeStr = _spcSetting.ONCHTYPE,
                    TestItem = dataGroup,
                    XBarBar = _xBar.ToString("0.####"),
                    Sigma = _sigma.ToString("0.####"),
                    Ca = (Math.Abs(((_spcSetting.USPEC + _spcSetting.LSPEC) / 2) - _xBar) / ((_spcSetting.USPEC - _spcSetting.LSPEC) / 2)).ToString("0.####"),
                    Cp = ((_spcSetting.USPEC - _spcSetting.LSPEC) / (6 * _sigma)).ToString("0.####"),
                    Cpk = Math.Min((_spcSetting.USPEC - _xBar) / (3 * _sigma), (_xBar - _spcSetting.LSPEC) / (3 * _sigma)).ToString("0.####"),
                    Sample = _allCnt.ToString(),
                    n = "1",
                    RMBar = _rmBar.ToString("0.####"),
                    PpkBar = _xBar.ToString("0.####"),
                    PpkSigma = "1.083488881217",
                    Pp = ((_spcSetting.USPEC - _spcSetting.LSPEC) / (6 * _sVal)).ToString("0.####"),
                    Ppk = Math.Min((_spcSetting.USPEC - _xBar)/ (3 * _sVal), (_xBar - _spcSetting.LSPEC) / (3 * _sVal)).ToString("0.####"),
                    DetailList = _spcMicroScopeDataList.CopyAToB<SPCMicroScopeDataEntity>()
                };

                return _spcOnlineChartEntity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<SPCChartSettingEntity> GetSettingList(int sn, int floor, string chartgrade, string prodIdList)
        {
            var _spcSettingList = _spcChartSettingRepository.SelectByConditions(chartgrade, floor, prodList: string.IsNullOrEmpty(prodIdList) ? null : prodIdList.Split(",").ToList(), sn: sn);

            return _spcSettingList.CopyAToB<SPCChartSettingEntity>();
        }
    }
}
