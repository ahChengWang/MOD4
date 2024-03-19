using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Extension.Demand;
using MOD4.Web.Repostory;
using MOD4.Web.Repostory.Dao;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Utility.Helper;

namespace MOD4.Web.DomainService
{
    public class MTDDashboardDomainService : BaseDomainService, IMTDDashboardDomainService
    {
        private readonly IMTDProductionScheduleRepository _mtdProductionScheduleRepository;
        private readonly IUploadDomainService _uploadDomainService;
        private readonly IOptionDomainService _optionDomainService;
        private readonly IAlarmXmlRepository _alarmXmlRepository;
        private readonly IEqpInfoRepository _eqpInfoRepository;
        private readonly IEquipMappingRepository _equipMappingRepository;
        private readonly ITargetSettingDomainService _targetSettingDomainService;
        private readonly ILcmProductRepository _lcmProductRepository;
        private readonly IMTDProcessFactory _mtdProcessFactory;
        private readonly Dictionary<string, string> _processEqDic = new Dictionary<string, string>
        {
            {"1300", "AOLB2010"},
            {"1330", "AFOG2010"},
            {"1355", "CLAM2010"},
            {"1460", "ASSY2010"},
            {"1700", "ACKE2010"}
        };
        private readonly Dictionary<int, string> _ownerDic = new Dictionary<int, string>
        {
            {0, "'INT0'"},
            {1, "'QTAP','LCME','PRDG','PROD','RES0'"}
        };
        private readonly string _zipsum106Url = "http://zipsum/modreport/Report/SHOPMOD/OperPerfDataSet.asp?";
        private readonly string _zipsum108Url = "http://zipsum/modreport/Report/SHOPMOD/EquUtilizationDataSet.asp";
        private readonly string _zipsum109Url = "http://zipsum/modreport/Report/SHOPMOD/EntityStaSumDataSet.asp";

        public MTDDashboardDomainService(IMTDProductionScheduleRepository mtdProductionScheduleRepository,
            IUploadDomainService uploadDomainService,
            IOptionDomainService optionDomainService,
            IAlarmXmlRepository alarmXmlRepository,
            IEqpInfoRepository eqpInfoRepository,
            IEquipMappingRepository equipMappingRepository,
            ITargetSettingDomainService targetSettingDomainService,
            ILcmProductRepository lcmProductRepository,
            IMTDProcessFactory mtdProcessFactory)
        {
            _mtdProductionScheduleRepository = mtdProductionScheduleRepository;
            _uploadDomainService = uploadDomainService;
            _optionDomainService = optionDomainService;
            _alarmXmlRepository = alarmXmlRepository;
            _eqpInfoRepository = eqpInfoRepository;
            _equipMappingRepository = equipMappingRepository;
            _targetSettingDomainService = targetSettingDomainService;
            _lcmProductRepository = lcmProductRepository;
            _mtdProcessFactory = mtdProcessFactory;
        }

        /// <summary>
        /// MTD 儀表板查詢
        /// </summary>
        /// <param name="floor">default 2F</param>
        /// <param name="date">default 昨日</param>
        /// <param name="time">default 24h</param>
        /// <returns></returns>
        public (string Result, List<MTDDashboardMainEntity> Entitys) DashboardSearch(int floor = 2, string date = "", decimal time = 24, int owner = 1, string shift = "ALL")
        {
            DateTime _nowTime = DateTime.Now.AddDays(-1).Date;
            DateTime _srchDate = _nowTime;

            // 是否有選擇日期
            if (DateTime.TryParseExact(date, "yyyy-MM-dd", null, DateTimeStyles.None, out _))
                DateTime.TryParseExact(date, "yyyy-MM-dd", null, DateTimeStyles.None, out _srchDate);

            // initial
            List<MTDDashboardSubEntity> _mtdDashboardList = new List<MTDDashboardSubEntity>();
            List<MTDPerformanceEntity> _mtdPerformanceDay = new List<MTDPerformanceEntity>();
            List<MTDPerformanceEntity> _mtdPerformanceMonth = new List<MTDPerformanceEntity>();
            List<MTDScheduleSettingDao> _mtdScheduleSetting = new List<MTDScheduleSettingDao>();
            List<MTDProdScheduleEntity> _mtdProdScheduleList = new List<MTDProdScheduleEntity>();
            Dictionary<string, decimal> _rpt108DownTimeDic = new Dictionary<string, decimal>();
            Dictionary<string, string> _upRateDic = new Dictionary<string, string>();
            Dictionary<string, string> _runRateDic = new Dictionary<string, string>();
            Dictionary<string, string> _uphRateDic = new Dictionary<string, string>();
            Dictionary<string, string> _oeeRateDic = new Dictionary<string, string>();

            // 取得當日MTD排程所有機種
            List<MTDProductionScheduleDao> _mtdScheduleDataList = _mtdProductionScheduleRepository.SelectMTDTodayPlan(floor, owner, _srchDate, _srchDate).ToList();

            _mtdScheduleSetting = _mtdProductionScheduleRepository.SelectSettingByConditions();

            var _todayProcess = _mtdScheduleDataList.ToDictionary(dic => ((dic.Process, dic.Node), dic.ProdId));

            //// 取得 MTD 本月有排程但今日無生產機種 (也需顯示在 Dashboard)
            //List<MTDProductionScheduleDao> _mtdTodayNoPlanList = _mtdProductionScheduleRepository.SelectMTDMonHavePlan(floor, owner, _srchDate, _srchDate).Except(_mtdScheduleDataList).ToList();

            //_mtdScheduleDataList.AddRange(_mtdTodayNoPlanList);

            _mtdProdScheduleList = (from allMTD in _mtdScheduleDataList
                                    join setting in _mtdScheduleSetting
                                    on new { allMTD.Sn, allMTD.LcmProdSn, PassNode = allMTD.Node } equals new { setting.Sn, setting.LcmProdSn, setting.PassNode }
                                    select new MTDProdScheduleEntity
                                    {
                                        Date = allMTD.Date,
                                        Sn = allMTD.Sn,
                                        Process = allMTD.Process,
                                        LcmProdSn = allMTD.LcmProdSn,
                                        ProdId = allMTD.ProdId,
                                        PassNode = allMTD.Node,
                                        WipNode = setting.WipNode,
                                        WipNode2 = setting.WipNode2,
                                        EqNo = setting.EqNo,
                                        Value = allMTD.Value,
                                        Model = allMTD.Model,
                                        MonthPlan = allMTD.MonthPlan,
                                        OwnerId = allMTD.OwnerId,
                                    }).ToList();

            // 取得MTD排程所有機種月計畫
            List<MTDProductionScheduleDao> _mtdMonthPlanList = _mtdProductionScheduleRepository.SelectMonthPlanQty(_srchDate.Year.ToString(), _srchDate.Month.ToString(), floor, owner);

            Task _106Task = Task.Run(() =>
            {
                // call zipsum 1.06 report 查詢 & 解析
                Parallel.ForEach(_mtdProdScheduleList.GroupBy(gb => gb.ProdId).Select(s => new
                {
                    ProdNo = s.Key,
                    Node = string.Join(",", s.Select(node => node.PassNode).Union(s.Select(node => node.WipNode)).Union(s.Select(node => node.WipNode2)).Distinct())
                }),
                new ParallelOptions { MaxDegreeOfParallelism = 6 },
                (prod) =>
                {
                    var _temp106Today = GetReport106TodayAsync(_srchDate, _srchDate, prod.ProdNo, prod.Node, shift);
                    var _temp106Month = GetReport106MonthlyAsync(DateTime.Parse($"{_srchDate:yyyy-MM-01}"), _srchDate, prod.ProdNo, prod.Node, shift);

                    Task.WaitAll(_temp106Today, _temp106Month);

                    lock (this)
                    {
                        _mtdPerformanceDay.AddRange(_temp106Today.Result);
                        _mtdPerformanceMonth.AddRange(_temp106Month.Result);
                    }
                });
            });


            Task _108109Task = Task.Run(() =>
            {
                var _downEqList = _mtdProdScheduleList.GroupBy(gb => gb.EqNo).Select(s => s.Key).ToList();
                _rpt108DownTimeDic = GetZipsum108TodayAsync(_srchDate, _downEqList).Result;
                var _tmpEqRateDicList = GetZipsum109TodayAsync(_srchDate, _downEqList).Result;
                _upRateDic = _tmpEqRateDicList.UPHRateDic;
                _runRateDic = _tmpEqRateDicList.RUNRateDic;
                _uphRateDic = _tmpEqRateDicList.UPHRateDic;
                _oeeRateDic = _tmpEqRateDicList.OEERateDic;
            });
            // 取得機況 (未處理、已處理)
            List<AlarmXmlDao> _alarmOverList = _alarmXmlRepository.SelectForMTD(_srchDate, _mtdProdScheduleList.Select(s => s.EqNo).Distinct().ToList(), _mtdProdScheduleList.Select(s => s.EqNo).Distinct().ToList());

            Task.WaitAll(_106Task, _108109Task);

            // 資料合併
            Parallel.ForEach(_mtdProdScheduleList.GroupBy(g => new { g.Process, g.EqNo }),
                new ParallelOptions { MaxDegreeOfParallelism = 6 },
                (detail) =>
                {
                    //_mtdProdScheduleList.GroupBy(g => new { g.Process, g.EqNo }).ToList().ForEach(detail =>
                    //{

                    List<MTDDashboardDetailEntity> _tempDetailList = detail.Select(dt =>
                    {
                        var _currSchedule = _mtdProdScheduleList.FirstOrDefault(f => f.Process == dt.Process && f.PassNode == dt.PassNode && f.ProdId == dt.ProdId) ??
                        new MTDProdScheduleEntity
                        {
                            Date = _srchDate,
                            Value = 0
                        };

                        var _currMonth = _mtdMonthPlanList.Where(w => w.Process == dt.Process && w.Node == dt.PassNode && w.ProdId == dt.ProdId);
                        var _currMonthRpt106 = _mtdPerformanceMonth.Where(w => w.Node == dt.PassNode.ToString() && w.Product == dt.ProdId);
                        var _currAlarmData = _alarmOverList.FirstOrDefault(f => f.tool_id == dt.EqNo && f.prod_id == dt.ProdId);
                        var _currRpt106PassQty = _mtdPerformanceDay.FirstOrDefault(f => f.Node == dt.PassNode.ToString() && f.Product == dt.ProdId);
                        var _currRpt106WipQty = _mtdPerformanceDay.Where(w => (w.Node == dt.WipNode.ToString() || (dt.WipNode2.ToString() != "0" && w.Node == dt.WipNode2.ToString())) && w.Product == dt.ProdId).ToList();

                        MTDDashboardDetailEntity _test = new MTDDashboardDetailEntity
                        {
                            Date = _currSchedule.Date.ToString("MM/dd"),
                            Equipment = dt.EqNo,
                            BigProduct = dt.Model,
                            Node = dt.PassNode.ToString(),
                            PlanProduct = dt.ProdId,
                            Output = _currRpt106PassQty?.Qty ?? 0,
                            Wip = _currRpt106WipQty?.Sum(sum => sum.Wip) ?? 0,
                            DayPlan = _currSchedule.Value,
                            RangPlan = Convert.ToInt32(_currSchedule.Value * (time / 24)),
                            RangDiff = Convert.ToInt32((_currRpt106PassQty?.Qty ?? 0) - (_currSchedule.Value * (time / 24))),
                            MonthPlan = _currMonth.Sum(sum => sum.Value), /*+ _currSchedule.Value,*/
                            MTDPlan = _currMonth.Where(mon => mon.Date <= _srchDate).Sum(sum => sum.Value), /*+ _currSchedule.Value,*/
                            MTDActual = _currMonthRpt106.Sum(sum => sum.Qty),
                            MTDDiff = _currMonthRpt106.Sum(sum => sum.Qty) - (_currMonth.Where(mon => mon.Date <= _srchDate).Sum(sum => sum.Value)), /*+ _currSchedule.Value),*/
                            EqAbnormal = _currAlarmData == null ? "" : _currAlarmData.comment,
                            RepaireTime = _currAlarmData == null ? "" : _currAlarmData.spend_time.ToString(),
                            Status = _currAlarmData == null ? "" : _currAlarmData.end_time == null ? "處理中" : "已排除"
                        };

                        return _test;

                    }).ToList();

                    //List<MTDDashboardDetailEntity> _tempDetailList = new List<MTDDashboardDetailEntity>();

                    lock (this)
                    {
                        var _eqNo = _tempDetailList.FirstOrDefault(f => !string.IsNullOrEmpty(f.Equipment))?.Equipment.Substring(4, 4) ?? "0";

                        _mtdDashboardList.Add(new MTDDashboardSubEntity
                        {
                            Sn = detail.FirstOrDefault().Sn,
                            EqNo = Convert.ToInt32(_eqNo),
                            Process = detail.Key.Process,
                            Plan = _tempDetailList.Sum(sum => sum.RangPlan),
                            Actual = _tempDetailList.Sum(sum => sum.Output),
                            DownTime = _rpt108DownTimeDic.ContainsKey(detail.Key.EqNo) ? _rpt108DownTimeDic[detail.Key.EqNo].ToString("0.00") : "0.0",
                            DownPercent = _rpt108DownTimeDic.ContainsKey(detail.Key.EqNo) ? $"{_rpt108DownTimeDic[detail.Key.EqNo] / (time * 60) * 100:0.00}%" : "0%",
                            UPPercent = _upRateDic.ContainsKey(detail.Key.EqNo) ? _upRateDic[detail.Key.EqNo] : "0%",
                            RUNPercent = _runRateDic.ContainsKey(detail.Key.EqNo) ? _runRateDic[detail.Key.EqNo] : "0%",
                            UPHPercent = _uphRateDic.ContainsKey(detail.Key.EqNo) ? _uphRateDic[detail.Key.EqNo] : "0%",
                            OEEPercent = _oeeRateDic.ContainsKey(detail.Key.EqNo) ? _oeeRateDic[detail.Key.EqNo] : "0%",
                            MTDDetail = _tempDetailList.OrderByDescending(o => o.BigProduct).ThenBy(b => b.PlanProduct).ToList()
                        });
                    }
                });

            _mtdDashboardList = _mtdDashboardList.Select(data =>
            {
                data.Diff = data.Actual - data.Plan;
                data.MTDDetail = data.MTDDetail.Where(detail => detail.MonthPlan != 0 || detail.MTDActual != 0).ToList();

                return data;
            }).OrderBy(ob => ob.Sn).ThenBy(tb => tb.EqNo).ToList();

            return ("", _mtdDashboardList.GroupBy(g => g.Process).Select(data => new MTDDashboardMainEntity
            {
                Process = data.Key,
                MTDSubList = data.ToList()
            }).ToList());
        }

        private async Task<List<MTDPerformanceEntity>> GetReport106TodayAsync(DateTime startDate, DateTime endDate, string prod, string node, string shift)
        {
            List<MTDPerformanceEntity> _tempZipsunEntity = new List<MTDPerformanceEntity>();

            var _rpt106List = await _inxReportService.Get106NewReportAsync<INXRpt106Entity>(startDate, endDate, shift, "ALL", new List<string> { prod });

            Parallel.ForEach(_rpt106List.Date.Data.Table,
                new ParallelOptions { MaxDegreeOfParallelism = 8 },
                (rpt) =>
                {
                    if (node.Contains(Convert.ToInt32(rpt.WORK_CTR).ToString()))
                        _tempZipsunEntity.Add(new MTDPerformanceEntity
                        {
                            Node = Convert.ToInt32(rpt.WORK_CTR).ToString(),
                            Product = prod,
                            Wip = Convert.ToInt32(rpt.WIP_QTY),
                            Qty = Convert.ToInt32(rpt.PASS_QTY)
                        });
                });

            //foreach (var rpt in _rpt106List)
            //{
            //    if (node.Contains(Convert.ToInt32(rpt.WORK_CTR).ToString()))
            //        _tempZipsunEntity.Add(new MTDPerformanceEntity
            //        {
            //            Node = Convert.ToInt32(rpt.WORK_CTR).ToString(),
            //            Product = prod,
            //            Wip = Convert.ToInt32(rpt.WIP_QTY),
            //            Qty = Convert.ToInt32(rpt.PASS_QTY)
            //        });
            //}

            //string _qStr = $"Calendar1={startDate}&calendar2={endDate}&shift=&rwktype=ALL&floor=&prod_type=ALL&source_fab=1=1&wo_type=1=1&WO_NBR=&psize=&message=" +
            //    $"&prod_size={prod.Substring(2, 3)}&big_prod={prod.Substring(0, 9)}&Shop=MOD4&G_FAC=6&lcm_owner=({_ownerDic[owner]})&calendar_1={startDate}" +
            //    $"&calendar_2={endDate}&vSelGroup=('{prod}')&LCDGrade=()";

            //var data = new StringContent(_qStr, Encoding.UTF8, "application/x-www-form-urlencodedn");

            //string[] array;

            //using (var client = new HttpClient())
            //{
            //    var response = client.PostAsync(_zipsum106Url + _qStr, data).Result;
            //    response.Content.Headers.ContentType.CharSet = "Big5";
            //    string result = response.Content.ReadAsStringAsync().Result;

            //    result = result.Remove(0, 12200);

            //    array = result.Split("<script language= \"VBScript\">");
            //}

            //_tempZipsunEntity.AddRange(Process(array, node, prod));

            return _tempZipsunEntity;
        }

        private async Task<List<MTDPerformanceEntity>> GetReport106MonthlyAsync(DateTime startDate, DateTime endDate, string prod, string node, string shift)
        {
            List<MTDPerformanceEntity> _tempZipsunEntity = new List<MTDPerformanceEntity>();

            var _rpt106List = await _inxReportService.Get106NewReportAsync<INXRpt106Entity>(startDate, endDate, shift, "ALL", new List<string> { prod });

            Parallel.ForEach(_rpt106List.Date.Data.Table,
                new ParallelOptions { MaxDegreeOfParallelism = 8 },
                (rpt) =>
                {
                    if (node.Contains(Convert.ToInt32(rpt.WORK_CTR).ToString()))
                        _tempZipsunEntity.Add(new MTDPerformanceEntity
                        {
                            Node = Convert.ToInt32(rpt.WORK_CTR).ToString(),
                            Product = prod,
                            Qty = Convert.ToInt32(rpt.PASS_QTY)
                        });
                });

            //foreach (var rpt in _rpt106List)
            //{
            //    if (node.Contains(Convert.ToInt32(rpt.WORK_CTR).ToString()))
            //        _tempZipsunEntity.Add(new MTDPerformanceEntity
            //        {
            //            Node = Convert.ToInt32(rpt.WORK_CTR).ToString(),
            //            Product = prod,
            //            Qty = Convert.ToInt32(rpt.PASS_QTY)
            //        });
            //}

            //string _qStr = $"Calendar1={startDate}&calendar2={endDate}&shift=&rwktype=ALL&floor=&prod_type=ALL&source_fab=1=1&wo_type=1=1&WO_NBR=&psize=&message=" +
            //    $"&prod_size={prod.Substring(2, 3)}&big_prod={prod.Substring(0, 9)}&Shop=MOD4&G_FAC=6&lcm_owner=({_ownerDic[owner]})&calendar_1={startDate}" +
            //    $"&calendar_2={endDate}&vSelGroup=('{prod}')&LCDGrade=()";

            //var data = new StringContent(_qStr, Encoding.UTF8, "application/x-www-form-urlencodedn");

            //string[] array;

            //using (var client = new HttpClient())
            //{
            //    var response = client.PostAsync(_zipsum106Url + _qStr, data).Result;
            //    response.Content.Headers.ContentType.CharSet = "Big5";

            //    string result = response.Content.ReadAsStringAsync().Result;

            //    result = result.Remove(0, 12200);

            //    array = result.Split("<script language= \"VBScript\">");

            //}

            //_tempZipsunEntity.AddRange(Process(array, node, prod));

            return _tempZipsunEntity;
        }

        private List<MTDPerformanceEntity> Process(string[] detailStr, string node, string prod)
        {
            if (detailStr.Length == 1 || detailStr[1].Contains("No Data For Your Query"))
                return new List<MTDPerformanceEntity>();

            List<MTDPerformanceEntity> _tempEntity = new List<MTDPerformanceEntity>();
            string[] _temp;
            string[] _tempArray;
            int _tempInt = 0;
            detailStr = detailStr.Skip(1).ToArray();
            //List<MTDPerformanceEntity> _tempResult = new List<MTDPerformanceEntity>();

            //if (detailStr.Length < 2 || detailStr[0].Contains("No Data For Your Query"))
            //    return;

            //Parallel.ForEach(detailStr, (output) =>
            //{
            //    _temp = output.Replace("\r\n\t\t", "").Split("ReportGrid1.TextMatrix");

            //    if (node.Contains(_temp[2].Split("=")[1].Replace("\"", "").Substring(0, 4)))
            //    {
            //        _tempArray = _temp[3].Split("=");
            //        _tempInt = int.Parse(_tempArray[1].Replace("\"", ""));

            //        _tempResult.Add(new MTDPerformanceEntity
            //        {
            //            Node = _temp[2].Split("=")[1].Replace("\"", "").Substring(0, 4),
            //            Product = prod,
            //            Qty = int.Parse(_tempArray[1].Replace("\"", ""))
            //        });
            //    }
            //});

            //lock (mtdPerformancesList)
            //{
            //    mtdPerformancesList.AddRange(_tempResult);
            //}

            foreach (var output in detailStr)
            {
                _temp = output.Replace("\r\n\t\t", "").Split("ReportGrid1.TextMatrix");

                if (node.Contains(_temp[2].Split("=")[1].Replace("\"", "").Substring(0, 4)))
                {
                    _tempArray = _temp[3].Split("=");
                    _tempInt = int.Parse(_tempArray[1].Replace("\"", ""));

                    _tempEntity.Add(new MTDPerformanceEntity
                    {
                        Node = _temp[2].Split("=")[1].Replace("\"", "").Substring(0, 4),
                        Product = prod,
                        Qty = int.Parse(_tempArray[1].Replace("\"", ""))
                    });
                }
            }

            return _tempEntity;
        }

        private async Task<Dictionary<string, decimal>> GetZipsum108TodayAsync(DateTime date, List<string> passEqList)
        {
            Dictionary<string, decimal> _respList = new Dictionary<string, decimal>();

            var _rpt108List = (await _inxReportService.Get108NewReportAsync<INXRpt108Entity>(date, passEqList)).Date.Data1.Table;

            //FormUrlEncodedContent formUrlEncodedContent = new FormUrlEncodedContent(new[]
            //{
            //    new KeyValuePair<string, string>("Calendar1", date),
            //    new KeyValuePair<string, string>("calendar2", date),
            //    new KeyValuePair<string, string>("shift", ""),
            //    new KeyValuePair<string, string>("intercond", "day"),
            //    new KeyValuePair<string, string>("floor", ""),
            //    new KeyValuePair<string, string>("eq_stat", "ALL"),
            //    new KeyValuePair<string, string>("big_eqp", $"{passEq.Substring(0, 4)}"),
            //    new KeyValuePair<string, string>("Shop", "MOD4"),
            //    new KeyValuePair<string, string>("G_FAC", "6"),
            //    new KeyValuePair<string, string>("calendar_1", date),
            //    new KeyValuePair<string, string>("calendar_2", date),
            //    new KeyValuePair<string, string>("eq_nbr", $"('{passEq}')"),
            //    new KeyValuePair<string, string>("pallet_nbr", ""),
            //});

            //string[] array;

            //using (var client = new HttpClient())
            //{
            //    var response = client.PostAsync(_zipsum108Url, formUrlEncodedContent).Result;
            //    response.Content.Headers.ContentType.CharSet = "Big5";
            //    string result = response.Content.ReadAsStringAsync().Result;

            //    result = result.Remove(0, 18758);

            //    array = result.Split("ReportGrid1.TextMatrix(row,");
            //}

            //array = array[array.Length - 2].Split("\r\n");

            //var _downSum = (array[4].Replace(" ", "").Replace("\"", "").Split("="))[1];

            foreach (var rpt108 in _rpt108List)
            {
                _respList.Add(rpt108.TRANS_DATE, rpt108.DOWN_TIME);
            }

            return _respList;
        }

        private async Task<(Dictionary<string, string> UPRateDic,
            Dictionary<string, string> RUNRateDic,
            Dictionary<string, string> UPHRateDic,
            Dictionary<string, string> OEERateDic)>
            GetZipsum109TodayAsync(DateTime date, List<string> passEqList)
        {
            Dictionary<string, string> _tmpUPRateDic = new Dictionary<string, string>();
            Dictionary<string, string> _tmpRUNRateDic = new Dictionary<string, string>();
            Dictionary<string, string> _tmpUPHRateDic = new Dictionary<string, string>();
            Dictionary<string, string> _tmpOEERateDic = new Dictionary<string, string>();

            var _rpt108List = (await _inxReportService.Get109NewReportAsync<INXRpt109Entity>(date, passEqList)).api01.Data.Table;

            //FormUrlEncodedContent formUrlEncodedContent = new FormUrlEncodedContent(new[]
            //{
            //    new KeyValuePair<string, string>("Calendar1", date),
            //    new KeyValuePair<string, string>("calendar2", date),
            //    new KeyValuePair<string, string>("shift", ""),
            //    new KeyValuePair<string, string>("big_eqp", $"{passEq.Substring(0, 4)}"),
            //    new KeyValuePair<string, string>("Shop", "MOD4"),
            //    new KeyValuePair<string, string>("G_FAC", "6"),
            //    new KeyValuePair<string, string>("vDate", ""),
            //    new KeyValuePair<string, string>("vDate_s", date),
            //    new KeyValuePair<string, string>("vDate_e", date),
            //    new KeyValuePair<string, string>("vSelGrp", $"{passEq.Substring(0, 4)}"),
            //    new KeyValuePair<string, string>("vSelMac", $"('{passEq}')"),
            //    new KeyValuePair<string, string>("vSelR1", "")
            //});

            //string[] array;

            //using (var client = new HttpClient())
            //{
            //    var response = client.PostAsync(_zipsum109Url, formUrlEncodedContent).Result;
            //    response.Content.Headers.ContentType.CharSet = "Big5";
            //    string result = response.Content.ReadAsStringAsync().Result;

            //    result = result.Remove(0, 21580);

            //    array = result.Split("ReportGrid1.TextMatrix");
            //}

            foreach (var rpt in _rpt108List)
            {
                _tmpUPRateDic.Add(rpt.EQUIP_NBR, $"{rpt.UP_RATE_MODIFY}%");
                _tmpRUNRateDic.Add(rpt.EQUIP_NBR, $"{rpt.RUN_RATE_MODIFY}%");
                _tmpUPHRateDic.Add(rpt.EQUIP_NBR, $"{rpt.UPH_RATE}%");
                _tmpOEERateDic.Add(rpt.EQUIP_NBR, $"{rpt.OEE_RATE_MODIFY}%");
            }

            return (_tmpUPRateDic, _tmpRUNRateDic, _tmpUPHRateDic, _tmpOEERateDic);
        }

        #region ======== schedule ========

        public List<ManufactureScheduleEntity> Search(string dateRange = "", int floor = 2, int owner = 1)
        {
            try
            {
                DateTime _nowTime = DateTime.Now;
                List<DateTime> _timeList = new List<DateTime>();
                DateTime _startDate = DateTime.Parse($"{_nowTime.ToString("yyyy/MM")}/01").AddDays(-10);
                DateTime _endDate = DateTime.Parse($"{_nowTime.AddMonths(1).ToString("yyyy/MM")}/15");

                if (!string.IsNullOrEmpty(dateRange))
                {
                    string[] _dateAry = dateRange.Split("-");

                    if (!DateTime.TryParseExact(_dateAry[0].Trim(), "yyyy/MM/dd", null, DateTimeStyles.None, out _) ||
                        !DateTime.TryParseExact(_dateAry[1].Trim(), "yyyy/MM/dd", null, DateTimeStyles.None, out _))
                    {
                        throw new Exception("日期異常");
                    }
                    DateTime.TryParseExact(_dateAry[0].Trim(), "yyyy/MM/dd", null, DateTimeStyles.None, out _startDate);
                    DateTime.TryParseExact(_dateAry[1].Trim(), "yyyy/MM/dd", null, DateTimeStyles.None, out _endDate);
                }

                List<ManufactureScheduleEntity> _manufactureSchedules = new List<ManufactureScheduleEntity>();
                List<MTDProductionScheduleDao> _mtdScheduleTest = new List<MTDProductionScheduleDao>();
                List<MTDProductionScheduleDao> _mtdScheduleDataList = _mtdProductionScheduleRepository.SelectByConditions(floor, owner, 0, _startDate, _endDate);
                //List<LcmProductDao> _defLcmProd = new List<LcmProductDao>(); //_lcmProductRepository.SelectByConditions(_mtdScheduleDataList.Select(mtd => mtd.ProdId).Distinct().ToList());
                List<MTDProductionScheduleDao> _mtdMonthPlanList = _mtdProductionScheduleRepository.SelectMonthPlanQty(_nowTime.Year.ToString(), _nowTime.Month.ToString(), floor, owner);

                _startDate = _mtdScheduleDataList.Select(s => s.Date).Min();
                _endDate = _mtdScheduleDataList.Select(s => s.Date).Max();

                for (DateTime d = _startDate; d <= _endDate; d = d.AddDays(1))
                    _timeList.Add(d);

                List<MTDProductionScheduleDao> _mtdScheduleHaveLoss = _mtdScheduleDataList.GroupBy(g => new { g.Sn, g.Process, g.LcmProdSn, g.Model, g.Node, g.ProdId })
                    .Where(w => w.Count() < _timeList.Count()).Select(s => new MTDProductionScheduleDao
                    {
                        Sn = s.Key.Sn,
                        Process = s.Key.Process,
                        LcmProdSn = s.Key.LcmProdSn,
                        Model = s.Key.Model,
                        Node = s.Key.Node,
                        ProdId = s.Key.ProdId
                    }).ToList();

                _mtdScheduleDataList.Where(w => _mtdScheduleHaveLoss.Select(s => s.Sn).Contains(w.Sn) && _mtdScheduleHaveLoss.Select(s => s.LcmProdSn).Contains(w.LcmProdSn) &&
                                                _mtdScheduleHaveLoss.Select(s => s.Node).Contains(w.Node))
                    .GroupBy(g => new { g.Sn, g.Process, g.LcmProdSn, g.Model, g.Node, g.ProdId })
                    .Select(s => new
                    {
                        s.Key.Sn,
                        s.Key.Process,
                        s.Key.LcmProdSn,
                        s.Key.Model,
                        s.Key.Node,
                        s.Key.ProdId,
                        Detail = s
                    }).ToList().ForEach(f =>
                    {
                        _mtdScheduleTest.AddRange(from d in _timeList
                                                  join schedule in f.Detail
                                                  on d equals schedule.Date into tmp
                                                  from schedule in tmp.DefaultIfEmpty()
                                                  where schedule is null
                                                  select new MTDProductionScheduleDao
                                                  {
                                                      Date = schedule?.Date ?? d.Date,
                                                      Sn = schedule?.Sn ?? f.Sn,
                                                      Process = schedule?.Process ?? f.Process,
                                                      Model = schedule?.Model ?? f.Model,
                                                      Node = schedule?.Node ?? f.Node,
                                                      ProdId = schedule?.ProdId ?? f.ProdId,
                                                      EqNo = schedule?.EqNo ?? "",
                                                      Value = schedule?.Value ?? 0,
                                                      Floor = schedule?.Floor ?? 2,
                                                      OwnerId = schedule?.OwnerId ?? 1,
                                                  });
                    });

                _mtdScheduleDataList.AddRange(_mtdScheduleTest);

                _manufactureSchedules = _mtdScheduleDataList.GroupBy(gb => new { gb.Process, gb.Model, gb.Node, gb.ProdId })
                    .Select(mtd => new ManufactureScheduleEntity
                    {
                        Process = mtd.Key.Process,
                        Category = mtd.Key.Model,
                        MonthPlan = _mtdMonthPlanList.Where(w => w.Process == mtd.Key.Process && w.Model == mtd.Key.Model && w.Node == mtd.Key.Node && w.ProdId == mtd.Key.ProdId).Sum(sum => sum.Value).ToString("#,0"),
                        ProductName = mtd.Key.ProdId,
                        Node = mtd.Key.Node,
                        PlanDetail = mtd.Select(s => new ManufactureDetailEntity
                        {
                            Date = s.Date.ToString("MM/dd"),
                            Quantity = s.Value,
                            IsToday = s.Date == _nowTime.Date
                        }).OrderBy(o => o.Date).ToList()
                    }).ToList();

                return _manufactureSchedules;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string Upload(IFormFile formFile, int floor, int owner, UserEntity userEntity)
        {
            try
            {
                string _uploadResult = "";
                string _errMsgTitle = "以下導致排程異常請人員確認\n";

                if (formFile.Length > 0)   // 檢查 <input type="file"> 是否輸入檔案？
                {
                    XSSFWorkbook workbook;
                    List<MTDProductionScheduleDao> _tmpMTDScheduleDao = new List<MTDProductionScheduleDao>();
                    List<MTDProductionScheduleDao> _insMTDScheduleDao = new List<MTDProductionScheduleDao>();
                    MTDScheduleUpdateHistoryDao _mtdScheduleUpdateHistoryDao = new MTDScheduleUpdateHistoryDao();
                    Dictionary<int, DateTime> _dateDictionary = new Dictionary<int, DateTime>();
                    DateTime _nowTime = DateTime.Now;
                    List<int> _cellChkIdx = new List<int> { 0, 1, 4 };
                    string[] _fileName = formFile.FileName.Split(".");
                    string _newFileName = $"{_fileName[0]}_{_nowTime:HHmmss}.{_fileName[1]}";
                    _ftpService.FTP_Upload(formFile, $"FTP_MTDSchedule/{_nowTime:yyMMdd}", _newFileName, false, "");
                    //string _newFileName = DoFileCopy(formFile, _nowTime);
                    (int, string) _processInfo = (0, "");

                    var _mtdScheduleSetting = _mtdProductionScheduleRepository.SelectSettingByConditions();

                    using (var ms = new MemoryStream())
                    {
                        formFile.CopyTo(ms);

                        // 上傳檔案，不用存檔。直接讓NOPI 讀取檔案內容（Stream串流）
                        Stream stream = new MemoryStream(ms.ToArray());

                        workbook = new XSSFWorkbook(stream); // 將剛剛的Excel (Stream）讀取到工作表裡面
                        // XSSFWorkbook() 只能讀取 System.IO.Stream
                    }

                    // 讀取 Excel裡面的工作表（跟以前 MVC 5完全相同）
                    #region
                    XSSFSheet _scheduleSheet = (XSSFSheet)workbook.GetSheetAt(0); // 生產排線 sheet
                    //XSSFSheet _verifySheet = (XSSFSheet)workbook.GetSheetAt(1); // 驗證 sheet

                    StringBuilder SB = new StringBuilder(); // System.Text命名空間

                    XSSFRow _headerDate = (XSSFRow)_scheduleSheet.GetRow(0); // 取得表頭日期

                    // 表頭列，共有幾個 "欄位"?（取得最後一欄的數字）
                    for (int k = 5; k < _headerDate.LastCellNum; k++)
                    {
                        if (_headerDate.GetCell(k) != null && _headerDate.GetCell(k).DateCellValue.Year != 1)
                        {
                            _dateDictionary.Add(k - 5, _headerDate.GetCell(k).DateCellValue);
                            //SB.Append(_headerDate.GetCell(k).StringCellValue + "   ");
                        }
                    }

                    // for迴圈的「啟始值」要加一，表示不包含 Excel表頭列
                    for (int i = (_scheduleSheet.FirstRowNum + 1); i <= _scheduleSheet.LastRowNum; i++)
                    {
                        // 每一列做迴圈
                        XSSFRow row = (XSSFRow)_scheduleSheet.GetRow(i); // 不包含 Excel表頭列

                        if (row.Count() < 5)
                            break;

                        if (!(row.Cells.Where(w => _cellChkIdx.Contains(w.ColumnIndex)).All(cell => cell.CellType == NPOI.SS.UserModel.CellType.String) &&
                            row.GetCell(4).StringCellValue.Length > 10))
                            continue;

                        for (int j = 0; j < _dateDictionary.Count; j++)
                        {
                            _processInfo = ConvertProcessToSn(row.GetCell(0).StringCellValue);

                            _tmpMTDScheduleDao.Add(new MTDProductionScheduleDao
                            {
                                Sn = _processInfo.Item1,
                                Process = row.GetCell(0).StringCellValue.Trim(),
                                Date = _dateDictionary[j],
                                Model = row.GetCell(1).StringCellValue.Trim(),
                                Node = Convert.ToInt32(row.GetCell(2).NumericCellValue),
                                //EqNo = row.GetCell(3).StringCellValue.Trim(),
                                ProdId = row.GetCell(4).StringCellValue.Trim(),
                                Value = Convert.ToInt32(row.GetCell(j + 5).NumericCellValue),
                                Floor = floor,
                                OwnerId = owner,
                                UpdateUser = userEntity.Name,
                                UpdateTime = _nowTime,
                            });
                        }
                    }
                    #endregion

                    IEnumerable<string> _uplMTDProdList = _tmpMTDScheduleDao.Select(s => s.ProdId).Distinct();

                    var _dupProd = _tmpMTDScheduleDao.Where(w => w.Date == _dateDictionary[0]).Select(tmp => new { tmp.Process, tmp.Node, tmp.ProdId }).GroupBy(g => new { g.Process, g.Node, g.ProdId, })
                        .Select(s => new
                        {
                            s.Key.Process,
                            s.Key.Node,
                            s.Key.ProdId,
                            detail = s
                        });

                    if (_dupProd.Any(a => a.detail.Count() > 1))
                    {
                        _uploadResult += "\n機種排程重複, 或無設定node\n";
                        _uploadResult += string.Join(", ", _dupProd.Where(w => w.detail.Count() > 1).Select(s => $"{s.Process}\\{s.ProdId}"));
                    }

                    var _defLcmProd = _lcmProductRepository.SelectByConditions(prodNoList: _uplMTDProdList.ToList());

                    List<string> _nonExistentProd = _uplMTDProdList.Except(_defLcmProd.Select(def => def.ProdNo).ToList()).ToList();

                    if (_nonExistentProd.Any())
                    {
                        _uploadResult += "\n系統未存在機種\n";
                        _uploadResult += string.Join(", ", _nonExistentProd);
                    }

                    var _nonNodeProdSchedule = from non in _tmpMTDScheduleDao.Where(w => w.Node == 0)
                                               join def in _defLcmProd
                                               on non.ProdId equals def.ProdNo
                                               join setting in _mtdScheduleSetting
                                               on new { non.Process, LcmProdSn = def.sn } equals new { setting.Process, setting.LcmProdSn } into r
                                               from setting in r.DefaultIfEmpty()
                                               select new MTDProductionScheduleDao
                                               {
                                                   Sn = setting?.Sn ?? 0,
                                                   Process = setting?.Process ?? non.Process,
                                                   Date = non.Date,
                                                   Model = non.Model,
                                                   Node = setting?.PassNode ?? 0,
                                                   EqNo = setting?.EqNo,
                                                   LcmProdSn = def.sn,
                                                   ProdId = non.ProdId,
                                                   Value = non.Value,
                                                   Floor = non.Floor,
                                                   OwnerId = non.OwnerId,
                                                   UpdateUser = non.UpdateUser,
                                                   UpdateTime = non.UpdateTime,
                                               };

                    var _nodeProdSchedule = from non in _tmpMTDScheduleDao.Where(w => w.Node != 0)
                                            join def in _defLcmProd
                                            on non.ProdId equals def.ProdNo
                                            join setting in _mtdScheduleSetting
                                            on new { non.Process, LcmProdSn = def.sn, PassNode = non.Node } equals new { setting.Process, setting.LcmProdSn, setting.PassNode } into r
                                            from setting in r.DefaultIfEmpty()
                                            select new MTDProductionScheduleDao
                                            {
                                                Sn = setting?.Sn ?? 0,
                                                Process = setting?.Process ?? non.Process,
                                                Date = non.Date,
                                                Model = non.Model,
                                                Node = setting?.PassNode ?? non.Node,
                                                EqNo = setting?.EqNo,
                                                LcmProdSn = def.sn,
                                                ProdId = non.ProdId,
                                                Value = non.Value,
                                                Floor = non.Floor,
                                                OwnerId = non.OwnerId,
                                                UpdateUser = non.UpdateUser,
                                                UpdateTime = non.UpdateTime,
                                            };

                    if (_nonNodeProdSchedule.Any(a => a.Sn == 0))
                    {
                        _uploadResult += "\nMTD參數未設定node\n";
                        _uploadResult += string.Join(", ", _nonNodeProdSchedule.Where(w => w.Sn == 0).GroupBy(g => new { g.Process, g.ProdId }).Select(s => $"{s.Key.Process}\\{s.Key.ProdId}"));
                    }

                    if (_nodeProdSchedule.Any(a => a.Sn == 0))
                    {
                        _uploadResult += "\nMTD參數多站點機種未設定node\n";
                        _uploadResult += string.Join(", ", _nodeProdSchedule.Where(w => w.Sn == 0).GroupBy(g => new { g.Process, g.ProdId, g.Node }).Select(s => $"{s.Key.Process}\\{s.Key.Node}\\{s.Key.ProdId}"));
                    }

                    _insMTDScheduleDao = _nonNodeProdSchedule.Union(_nodeProdSchedule).Where(w => w.Sn != 0).ToList();

                    //var _settingResult = VerifySettings(_uplMTDScheduleEntity, floor);

                    //if (!string.IsNullOrEmpty(_settingResult.Item1))
                    //    return _settingResult.Item1;

                    //List<MTDProductionScheduleDao> _updMTDScheduleDao = (from mtd in _uplMTDScheduleEntity
                    //                                                     join setting in _settingResult.Item2
                    //                                                     on new { mtd.ProdNo, mtd.Process } equals new { setting.ProdNo, setting.Process }
                    //                                                     select new MTDProductionScheduleDao
                    //                                                     {
                    //                                                         Sn = mtd.Sn,
                    //                                                         Process = setting.Process,
                    //                                                         Date = mtd.Date,
                    //                                                         Model = mtd.Model,
                    //                                                         ProdId = setting.LcmProdSn,
                    //                                                         Value = mtd.Qty,
                    //                                                         Floor = mtd.Floor,
                    //                                                         OwnerId = mtd.OwnerId,
                    //                                                         UpdateUser = mtd.UpdateUser,
                    //                                                         UpdateTime = mtd.UpdateTime,
                    //                                                     }).ToList();


                    _mtdScheduleUpdateHistoryDao.FileName = _newFileName;
                    _mtdScheduleUpdateHistoryDao.Floor = floor;
                    _mtdScheduleUpdateHistoryDao.OwnerId = owner;
                    _mtdScheduleUpdateHistoryDao.UpdateUser = userEntity.Name;
                    _mtdScheduleUpdateHistoryDao.UpdateTime = _nowTime;

                    using (TransactionScope _scope = new TransactionScope())
                    {
                        bool _updRes = false;
                        bool _insHisRes = false;

                        _mtdProductionScheduleRepository.DeleteSchedule(owner, _insMTDScheduleDao.Min(min => min.Date));
                        _updRes = _mtdProductionScheduleRepository.InsertSchedule(_insMTDScheduleDao) == _insMTDScheduleDao.Count;
                        _insHisRes = _mtdProductionScheduleRepository.InsertScheduleHistory(_mtdScheduleUpdateHistoryDao) == 1;


                        if (_updRes && _insHisRes)
                        {
                            if (!string.IsNullOrEmpty(_uploadResult))
                                _uploadResult = $"{_errMsgTitle}{_uploadResult}\n其餘更新成功";
                            else
                                _uploadResult += "更新成功";

                            _scope.Complete();
                        }
                        else
                        {
                            _uploadResult += "\n更新異常";
                        }
                    }

                }

                return _uploadResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetLatestUpdate(int floor = 2, int owner = 1)
        {
            try
            {
                MTDScheduleUpdateHistoryDao _mtdScheduleHis = _mtdProductionScheduleRepository.SelectHistory(floor, owner);

                return $@"{(_mtdScheduleHis != null ? _mtdScheduleHis.UpdateTime.ToString("yyyy/MM/dd HH:mm") + "- by " + _mtdScheduleHis.UpdateUser : "")}";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string DoFileCopy(IFormFile uploadFile, DateTime nowTime)
        {
            var _fileNameStr = "";

            if (uploadFile.Length > 0)
            {
                var _url = _uploadDomainService.GetFileServerUrl();
                var _folder = $@"MTDupload\{nowTime.ToString("yyMMdd")}";

                if (!Directory.Exists($@"{_url}\{_folder}"))
                {
                    Directory.CreateDirectory($@"{_url}\{_folder}");
                }

                var _fileArray = uploadFile.FileName.Split(".");

                _fileNameStr = $"{_fileArray[0]}_{nowTime.ToString("ffff")}.{_fileArray[1]}";

                var path = $@"{_url}\{_folder}\{_fileNameStr}";

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    uploadFile.CopyTo(stream);
                }
            }

            return _fileNameStr;
        }

        private (int, string) ConvertProcessToSn(string process)
        {
            switch (process)
            {
                case "BOND":
                    return (1, "1300");
                case "FOG":
                    return (2, "1330");
                case "LAM":
                    return (3, "1355");
                case "ASSY":
                    return (4, "1460");
                case "CDP":
                    return (5, "1700");
                case "SHIP":
                    return (6, "1910");
                default:
                    return (0, "");
            }
        }

        private (string, List<MTDProcessSettingEntity>) VerifySettings(List<MTDProdScheduleEntity> uplMTDScheduleEntity, int floor)
        {
            string _verifyResult = "";

            var _settingList = _targetSettingDomainService.GetUploadMTDSettings(uplMTDScheduleEntity.Select(mtd => mtd.EqNo).Distinct(), uplMTDScheduleEntity.Select(mtd => mtd.Process).Distinct());

            var _dupProdSettings = _settingList.GroupBy(g => new { g.ProdNo, g.Process }).Where(w => w.Count() > 1).Select(s => s.Key).ToList();

            if (_dupProdSettings.Any())
            {
                _verifyResult += "請確認以下機種設定是否異常\n";
                foreach (var setting in _dupProdSettings)
                {
                    _verifyResult += $"process({setting.Process}),機種({setting.ProdNo})\n";
                }
            }

            //switch (floor)
            //{
            //    case 2:
            //        var _nonSetting2 = _settingList.GroupBy(g => new { g.ProdNo }).Where(w => w.Count() < 5).Select(s => s.Key).ToList();
            //        if (_nonSetting2.Any())
            //        {
            //            _verifyResult += "\n確認以下機種是否皆已設定\n";
            //            foreach (var setting in _nonSetting2)
            //            {
            //                _verifyResult += $"機種({setting.ProdNo})\n";
            //            }
            //        }
            //        break;
            //    case 3:
            //        var _nonSetting3 = _settingList.GroupBy(g => new { g.ProdNo }).Where(w => w.Count() < 4).Select(s => s.Key).ToList();
            //        if (_nonSetting3.Any())
            //        {
            //            _verifyResult += "\n確認以下機種是否皆已設定\n";
            //            foreach (var setting in _nonSetting3)
            //            {
            //                _verifyResult += $"機種序號({setting.ProdNo})\n";
            //            }
            //        }
            //        break;
            //    default:
            //        break;
            //}

            return (_verifyResult, _settingList);
        }

        #endregion

        #region ======== schedule setting ========
        public List<MTDScheduleSettingEntity> GetMTDSetting(int prodSn = 1206)
        {
            try
            {
                List<MTDScheduleSettingDao> _mtdSettingList = _mtdProductionScheduleRepository.SelectSettingByConditions(prodSn);

                return _mtdSettingList.CopyAToB<MTDScheduleSettingEntity>();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public string UpdateMTDSetting(List<MTDScheduleSettingEntity> updEntity, UserEntity userEntity)
        {
            try
            {
                string _updResult = "";
                var _nowTime = DateTime.Now;
                var _defNodeList = _optionDomainService.GetAllNodeList()
                    .Select(s => new
                    {
                        Sn = s.Id,
                        Process = s.Value
                    }).Distinct();

                var _prodMTDScheduleList = _mtdProductionScheduleRepository.SelectByConditions(0, 0, prodSn: updEntity.FirstOrDefault().LcmProdSn, null, null)
                    .GroupBy(g => new { g.Sn, g.Process, g.Node, g.EqNo, g.LcmProdSn, g.Floor })
                    .Select(s => new
                    {
                        s.Key.Sn,
                        s.Key.Process,
                        s.Key.Node,
                        s.Key.EqNo,
                        s.Key.LcmProdSn,
                        s.Key.Floor
                    });

                var _updMTDProdSchedule = (from schedule in _prodMTDScheduleList
                                           join upd in updEntity
                                           on new { schedule.Sn, schedule.LcmProdSn, schedule.Node } equals new { upd.Sn, upd.LcmProdSn, Node = upd.OldPassNode }
                                           where schedule.Node != upd.PassNode
                                           select new MTDProductionScheduleDao
                                           {
                                               Sn = schedule.Sn,
                                               LcmProdSn = schedule.LcmProdSn,
                                               Node = upd.PassNode,
                                               EqNo = upd.EqNo,
                                               UpdateTime = _nowTime,
                                               UpdateUser = userEntity.Name
                                           }).ToList();

                List<MTDScheduleSettingDao> _updMTDSetting = updEntity.Select(s => new MTDScheduleSettingDao
                {
                    Sn = s.Sn,
                    LcmProdSn = s.LcmProdSn,
                    PassNode = s.PassNode,
                    WipNode = s.WipNode,
                    WipNode2 = s.WipNode2,
                    EqNo = s.EqNo ?? "",
                    Process = _defNodeList.FirstOrDefault(f => f.Sn == s.Sn).Process,
                    UpdateTime = _nowTime,
                    UpdateUser = userEntity.Name
                }).ToList();

                using (var scope = new TransactionScope())
                {
                    bool _updRes = false;

                    if (_updMTDProdSchedule.Any())
                        _mtdProductionScheduleRepository.UpdateMTDSchedule(_updMTDProdSchedule);

                    _mtdProductionScheduleRepository.DeleteSetting(_updMTDSetting.FirstOrDefault().LcmProdSn);

                    _updRes = _mtdProductionScheduleRepository.InsertSetting(_updMTDSetting) == _updMTDSetting.Count;

                    if (_updRes)
                        scope.Complete();
                    else
                        _updResult = "更新失敗";
                }

                return _updResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region === MTBF、MTTR ===
        public MTBFMTTRDashboardEntity GetMTBFMTTRList(string beginDate, string endDate, string equipment, int floor)
        {
            try
            {
                DateTime _now = DateTime.Now.Date;
                DateTime _beginDate = _now;
                DateTime _endDate = DateTime.Parse($"{_now.AddDays(1).AddSeconds(-1)}");

                if (DateTime.TryParse(beginDate, out _) && DateTime.TryParse(endDate, out _))
                {
                    _beginDate = DateTime.Parse(beginDate);
                    _endDate = DateTime.Parse(endDate).AddDays(1).AddSeconds(-1);
                }

                var _eqpinfoSetting = _equipMappingRepository.SelectEqByConditions(floor, equipNo: equipment);
                var _eqpinfoList = _eqpInfoRepository.SelectForMTBFMTTR(_beginDate, _endDate, equipment, floor);

                if (!_eqpinfoList.Any())
                    return null;

                double _sumWorkTime = 0;
                // 計算 MTBF actual
                for (int i = 1; i < _eqpinfoList.Count; i++)
                {
                    _sumWorkTime += _eqpinfoList[i].End_Time.Subtract(_eqpinfoList[i - 1].Start_Time).TotalSeconds / 60;
                }

                MTBFMTTRDashboardEntity _dashboardEntity = new MTBFMTTRDashboardEntity
                {
                    MTBFTarget = "250",
                    MTBFActual = (_sumWorkTime / _eqpinfoList.Count).ToString("0.00"),
                    MTTRTarget = "25",
                    MTTRActual = (_eqpinfoList.Sum(sum => Convert.ToDecimal(sum.Repair_Time)) / _eqpinfoList.Count).ToString("0.00"),
                    MTTRDetail = _eqpinfoList.GroupBy(g => g.Code).Select(s => new MTTRDetailEntity
                    {
                        DownCode = s.Key,
                        AvgTime = (s.Select(ss => Convert.ToDecimal(ss.Repair_Time)).Sum() / s.Count())
                    }).ToList(),
                    EqpInfoDetail = _eqpinfoList.Select(eqDao => new EquipmentEntity
                    {
                        ToolStatus = eqDao.Code,
                        StatusCdsc = eqDao.Code_Desc,
                        Comment = eqDao.Comments,
                        UserId = eqDao.Operator,
                        LmTime = eqDao.Start_Time,
                        RepairTime = eqDao.Repair_Time
                    }).ToList()
                };

                return _dashboardEntity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string UpdateMTBFMTTRSetting(EqMappingEntity updateEntity, UserEntity userEntity)
        {
            try
            {
                string _updRes = "";

                using (var scope = new TransactionScope())
                {
                    if (_equipMappingRepository.UpdateTarget(new EquipMappingDao
                    {
                        EQUIP_NBR = updateEntity.EQUIP_NBR,
                        MTBFTarget = updateEntity.MTBFTarget,
                        MTTRTarget = updateEntity.MTTRTarget,
                        Floor = updateEntity.Floor,
                        UpdateTime = DateTime.Now,
                        UpdateUser = userEntity.Name
                    }) == 1)
                    {
                        scope.Complete();
                    }
                    else
                        _updRes = "更新失敗";
                }

                return _updRes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Monitor
        public async Task<List<MTDProcessDailyEntity>> GetMonitorDailyMTDAsync()
        {
            try
            {
                DateTime _srchDate = DateTime.Now.AddMinutes(-450).Date;

                // 取得當日MTD排程所有機種
                List<MTDProductionScheduleDao> _mtdScheduleDataList = _mtdProductionScheduleRepository.SelectMTDTodayPlan(2, 1, _srchDate, _srchDate).ToList();
                var _mtdProdProcessSettings = _targetSettingDomainService.GetSettingForMTD(_mtdScheduleDataList.Select(s => s.LcmProdSn).ToList());
                var _rpt106Task = _inxReportService.Get106NewReportAsync<INXRpt106Entity>(_srchDate, _srchDate, "ALL", "2", _mtdProdProcessSettings.Select(d => d.ProdNo).Distinct().ToList());

                var _dailyMTD = from plan in _mtdScheduleDataList
                                join setting in _mtdProdProcessSettings
                                on new { plan.Sn, plan.LcmProdSn } equals new { setting.Sn, setting.LcmProdSn }
                                select new
                                {
                                    plan.Sn,
                                    plan.Process,
                                    setting.Node,
                                    plan.ProdId,
                                    setting.ProdNo,
                                    plan.Value,
                                    setting.DownEq
                                };

                var _rpt106DataList = (await _rpt106Task).Date.Data.Table;

                List<MTDProcessDailyEntity> mtdDailyList = _dailyMTD.GroupBy(gb => new { gb.Sn, gb.Process, gb.Node }).Select(mtd => new MTDProcessDailyEntity
                {
                    Sn = mtd.Key.Sn,
                    Process = mtd.Key.Process,
                    DayPlanQty = mtd.Sum(sum => sum.Value),
                    DayActQty = Convert.ToInt32(_rpt106DataList.Where(w => Convert.ToInt32(w.WORK_CTR) == Convert.ToInt32(mtd.Key.Node))?.Select(s => s.PASS_QTY).Sum() ?? 0),
                }).ToList();

                return mtdDailyList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}