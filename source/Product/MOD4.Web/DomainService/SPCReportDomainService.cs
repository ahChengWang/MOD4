﻿using MOD4.Web.DomainService.Entity;
using Utility.Helper;
using MOD4.Web.Repostory;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Transactions;
using NLog;

namespace MOD4.Web.DomainService
{
    public class SPCReportDomainService : BaseDomainService, ISPCReportDomainService
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
                    if (_tmpSetting.ONCHTYPE == "CX")
                        spc.OOS = spc.DTX > _tmpSetting.USPEC || spc.DTX < _tmpSetting.LSPEC;
                    else if (_tmpSetting.ONCHTYPE == "XXRM")
                    {
                        spc.OOC1 = spc.DTX > _tmpSetting.UCL1 || spc.DTX < _tmpSetting.LCL1;
                        spc.OOC2 = spc.DTRM > _tmpSetting.UCL2 || spc.DTRM < _tmpSetting.LCL2;
                        spc.OOS = spc.DTX > _tmpSetting.USPEC || spc.DTX < _tmpSetting.LSPEC;
                    }
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
                _logHelper.WriteLog(LogLevel.Error, this.GetType().Name, ex.Message);
                throw ex;
            }
            finally
            {
                _logHelper.WriteLog(LogLevel.Info, this.GetType().Name, $"使用者查詢");
            }
        }

        public SPCOnlineChartEntity Detail(int floor, string chartgrade, string dateRange, string equpId, string prodId, string dataGroup)
        {
            try
            {
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
                var _xBar = _sumDTX / _allCnt; // DTX 的平均值
                var _rmBar = _sumDTRM / _allCnt; // DTRM 的平均值
                var _sigma = _rmBar / 1.128;
                double _calculateS = 0;

                _spcMicroScopeDataList.ForEach(fe =>
                {
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
                    fe.OOR1 = fe.OOR1;
                    fe.OOR2 = fe.OOR2;

                    _calculateS += Math.Pow(fe.DTX - _xBar, 2);
                });

                var _sVal = Math.Sqrt(_calculateS / (_allCnt - 1));

                SPCOnlineChartEntity _spcOnlineChartEntity = new SPCOnlineChartEntity
                {
                    ChartId = _spcSetting.ONCHID,
                    TypeStr = _spcSetting.ONCHTYPE,
                    TestItem = dataGroup,
                    //PpkBar = _xBar.ToString("0.####"),
                    //PpkSigma = "1.083488881217",
                    //Pp = ((_spcSetting.USPEC - _spcSetting.LSPEC) / (6 * _sVal)).ToString("0.####"),
                    //Ppk = Math.Min((_spcSetting.USPEC - _xBar) / (3 * _sVal), (_xBar - _spcSetting.LSPEC) / (3 * _sVal)).ToString("0.####"),
                    DetailList = _spcMicroScopeDataList.CopyAToB<SPCMicroScopeDataEntity>()
                };

                if (_spcOnlineChartEntity.TypeStr == "XXRM")
                {
                    _spcOnlineChartEntity.XBarBar = _xBar.ToString("0.#####");
                    _spcOnlineChartEntity.Sigma = _sigma.ToString("0.#####");
                    _spcOnlineChartEntity.Ca = (Math.Abs(((_spcSetting.USPEC + _spcSetting.LSPEC) / 2) - _xBar) / ((_spcSetting.USPEC - _spcSetting.LSPEC) / 2)).ToString("0.#####");
                    _spcOnlineChartEntity.Cp = ((_spcSetting.USPEC - _spcSetting.LSPEC) / (6 * _sigma)).ToString("0.#####");
                    _spcOnlineChartEntity.Cpk = Math.Min((_spcSetting.USPEC - _xBar) / (3 * _sigma), (_xBar - _spcSetting.LSPEC) / (3 * _sigma)).ToString("0.#####");
                    _spcOnlineChartEntity.Sample = _allCnt.ToString();
                    _spcOnlineChartEntity.n = "1";
                    _spcOnlineChartEntity.RMBar = _rmBar.ToString("0.#####");
                }

                return _spcOnlineChartEntity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<SPCChartSettingEntity> GetSettingList(int sn, int floor = 0, string chartgrade = "", string prodIdList = "")
        {
            var _spcSettingList = _spcChartSettingRepository.SelectByConditions(chartgrade, floor, prodList: string.IsNullOrEmpty(prodIdList) ? null : prodIdList.Split(",").ToList(), sn: sn);

            return _spcSettingList.CopyAToB<SPCChartSettingEntity>();
        }

        public SPCChartSettingEntity GetSettingEdit(int sn)
        {
            var _spcSetting = GetSettingList(sn).FirstOrDefault();

            DateTime _startDate = DateTime.Parse($"{DateTime.Now.AddMonths(-3).ToString("yyyy-MM")}-01");

            List<SPCMicroScopeDataDao> _spcMicroScopeDataList = _spcMicroScopeDataRepository.SelectByConditions("", _startDate, DateTime.Now, _spcSetting.PECD, _spcSetting.DataGroup);

            if (!_spcMicroScopeDataList.Any())
                return _spcSetting;

            var _spcMicroScopeDatarLast3Mon = _spcMicroScopeDataList.Where(w => w.MeasureDate >= _startDate && w.MeasureDate < _startDate.AddMonths(1));
            var _spcMicroScopeDataLast2Mon = _spcMicroScopeDataList.Where(w => w.MeasureDate >= _startDate.AddMonths(1) && w.MeasureDate < _startDate.AddMonths(2));
            var _spcMicroScopeDataLastMon = _spcMicroScopeDataList.Where(w => w.MeasureDate >= _startDate.AddMonths(2) && w.MeasureDate < _startDate.AddMonths(3));
            var _xBar = 0.0; // DTX 的平均值
            var _rmBar = 0.0; // DTRM 的平均值
            var _sigma = 0.0;

            if (_spcMicroScopeDatarLast3Mon.Any())
            {
                _xBar = _spcMicroScopeDatarLast3Mon.Sum(spc => spc.DTX) / _spcMicroScopeDatarLast3Mon.Count();
                _rmBar = _spcMicroScopeDatarLast3Mon.Sum(spc => spc.DTRM) / _spcMicroScopeDatarLast3Mon.Count();
                _sigma = _rmBar / 1.128;
                _spcSetting.Last3MonCPK = Math.Min((_spcSetting.USPEC - _xBar) / (3 * _sigma), (_xBar - _spcSetting.LSPEC) / (3 * _sigma));
            }

            if (_spcMicroScopeDataLast2Mon.Any())
            {
                _xBar = _spcMicroScopeDataLast2Mon.Sum(spc => spc.DTX) / _spcMicroScopeDataLast2Mon.Count();
                _rmBar = _spcMicroScopeDataLast2Mon.Sum(spc => spc.DTRM) / _spcMicroScopeDataLast2Mon.Count();
                _sigma = _rmBar / 1.128;
                _spcSetting.Last2MonCPK = Math.Min((_spcSetting.USPEC - _xBar) / (3 * _sigma), (_xBar - _spcSetting.LSPEC) / (3 * _sigma));
            }

            if (_spcMicroScopeDataLastMon.Any())
            {
                _xBar = _spcMicroScopeDataLastMon.Sum(spc => spc.DTX) / _spcMicroScopeDataLastMon.Count();
                _rmBar = _spcMicroScopeDataLastMon.Sum(spc => spc.DTRM) / _spcMicroScopeDataLastMon.Count();
                _sigma = _rmBar / 1.128;
                _spcSetting.LastMonCPK = Math.Min((_spcSetting.USPEC - _xBar) / (3 * _sigma), (_xBar - _spcSetting.LSPEC) / (3 * _sigma));

                // 計算建議值
                _spcSetting.LastMonUCL1 = _xBar + (_sigma * 3);
                _spcSetting.LastMonCL1 = _xBar;
                _spcSetting.LastMonLCL1 = _xBar - (_sigma * 3);
                _spcSetting.LastMonUCL2 = _rmBar * 3.267;
                _spcSetting.LastMonCL2 = _rmBar;
                _spcSetting.LastMonLCL2 = 0;

                _spcSetting.NewUCL1 = Math.Round(_xBar + (_sigma * 3), 5);
                _spcSetting.NewCL1 = Math.Round(_xBar, 5);
                _spcSetting.NewLCL1 = Math.Round(_xBar - (_sigma * 3), 5);
                _spcSetting.NewUCL2 = Math.Round(_rmBar * 3.267, 5);
                _spcSetting.NewCL2 = Math.Round(_rmBar, 5);
                _spcSetting.NewLCL2 = 0;
            }

            return _spcSetting;
        }

        public string UpdateSPCSetting(SPCChartSettingEntity updEntity, UserEntity userEntity)
        {
            try
            {
                string _result = "";

                var _spcSetting = GetSettingList(updEntity.sn).FirstOrDefault();

                SPCChartSettingDao _updSPCChartSettingDao = new SPCChartSettingDao();

                _updSPCChartSettingDao.sn = updEntity.sn;
                _updSPCChartSettingDao.UCL1 = Convert.ToSingle(updEntity.NewUCL1);
                _updSPCChartSettingDao.CL1 = Convert.ToSingle(updEntity.NewCL1);
                _updSPCChartSettingDao.LCL1 = Convert.ToSingle(updEntity.NewLCL1);
                _updSPCChartSettingDao.UCL2 = Convert.ToSingle(updEntity.NewUCL2);
                _updSPCChartSettingDao.CL2 = Convert.ToSingle(updEntity.NewCL2);
                _updSPCChartSettingDao.LCL2 = Convert.ToSingle(updEntity.NewLCL2);
                _updSPCChartSettingDao.Memo = updEntity.Memo;
                _updSPCChartSettingDao.UpdateUser = userEntity.Name;
                _updSPCChartSettingDao.UpdateTime = DateTime.Now;

                using (TransactionScope _scope = new TransactionScope())
                {
                    bool _updRes = _spcChartSettingRepository.Update(_updSPCChartSettingDao) == 1;

                    if (_updRes)
                        _scope.Complete();
                    else
                        _result = "更新異常";
                }

                return _result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
    }
}