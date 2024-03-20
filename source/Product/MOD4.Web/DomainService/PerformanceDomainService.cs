using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using MOD4.Web.Repostory;
using MOD4.Web.Repostory.Dao;
using NLog;
//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Utility.Helper;

namespace MOD4.Web.DomainService
{
    public class PerformanceDomainService : BaseDomainService, IPerformanceDomainService
    {
        private readonly IDailyEquipmentRepository _dailyEquipmentRepository;
        private readonly ITargetSettingDomainService _targetSettingDomainService;
        private readonly IEquipmentDomainService _equipmentDomainService;
        private readonly ILineTTRepository _lineTTRepository;
        private readonly IOptionDomainService _optionDomainService;
        private readonly IDefinitionNodeDescRepository _definitionNodeDescRepository;
        private readonly ILcmProductRepository _lcmProductRepository;
        private readonly IEfficiencySettingRepository _efficiencySettingRepository;
        private readonly IDailyEfficiencyRepository _dailyEfficiencyRepository;
        private readonly ITakeBackWTRepository _takeBackWTRepository;
        private readonly IReclaimWTRptRepository _reclaimWTRptRepository;
        private readonly Dictionary<int, string> _defaultNodeDic = new Dictionary<int, string>
        {
            {1330,"FOG" },
            {1355,"OCA硬對硬"},
            {1415,"ASSY(BL外購)"},
            {1460,"ACCD (模組UV膠檢驗)"},
            {1700,"D KEN" },
            {1910,"SHIPPING" }
        };
        private readonly Dictionary<string, (string, int)> _allNodeDictionary = new Dictionary<string, (string, int)>
        {
            {"1300",("PCBI(HMT)",1) },
            {"1330",("FOG",2) },
            {"1355",("OCA硬對硬",3) },
            {"1415",("ASSY(BL外購)",4) },
            {"1420",("AAFC(同C-)",5) },
            {"1460",("ACCD (模組UV膠檢驗)",6) },
            {"1500",("AGING",7) },
            {"1600",("(A+B) Panel C-",8) },
            {"1720",("D2",9) },
            {"1700",("D KEN",10) },
            {"1910",("SHIPPING",11) }
        };
        private readonly string[] _passTransType = new string[] { "MVOT", "MRWK", "SHIP" };
        private readonly Dictionary<int, string> _ownerDic = new Dictionary<int, string>
        {
            {1,"'QTAP','LCME','PRDG','PROD','RES0'" },
            {2,"'INT0'" },
        };
        private readonly Dictionary<ProcessEnum, List<int>> _processNodeDic = new Dictionary<ProcessEnum, List<int>>
        {
            {ProcessEnum.BOND, new List<int>{ 1100 }},
            {ProcessEnum.FOG, new List<int>{ 1330 }},
            {ProcessEnum.LAM, new List<int>{ 1355 }},
            {ProcessEnum.ASSY,new List<int>{ 1400, 1415 }},
            {ProcessEnum.CDP, new List<int>{ 1600, 1700 }},
        };

        #region time block
        private TimeSpan _time0730 = new TimeSpan(7, 30, 0);
        private TimeSpan _time0830 = new TimeSpan(8, 30, 0);
        private TimeSpan _time0930 = new TimeSpan(09, 30, 0);
        private TimeSpan _time1030 = new TimeSpan(10, 30, 0);
        private TimeSpan _time1130 = new TimeSpan(11, 30, 0);
        private TimeSpan _time1230 = new TimeSpan(12, 30, 0);
        private TimeSpan _time1330 = new TimeSpan(13, 30, 0);
        private TimeSpan _time1430 = new TimeSpan(14, 30, 0);
        private TimeSpan _time1530 = new TimeSpan(15, 30, 0);
        private TimeSpan _time1630 = new TimeSpan(16, 30, 0);
        private TimeSpan _time1730 = new TimeSpan(17, 30, 0);
        private TimeSpan _time1830 = new TimeSpan(18, 30, 0);
        private TimeSpan _time1930 = new TimeSpan(19, 30, 0);
        private TimeSpan _time2030 = new TimeSpan(20, 30, 0);
        private TimeSpan _time2130 = new TimeSpan(21, 30, 0);
        private TimeSpan _time2230 = new TimeSpan(22, 30, 0);
        private TimeSpan _time2330 = new TimeSpan(23, 30, 0);
        private TimeSpan _time0030 = new TimeSpan(00, 30, 0);
        private TimeSpan _time0130 = new TimeSpan(01, 30, 0);
        private TimeSpan _time0230 = new TimeSpan(02, 30, 0);
        private TimeSpan _time0330 = new TimeSpan(03, 30, 0);
        private TimeSpan _time0430 = new TimeSpan(04, 30, 0);
        private TimeSpan _time0530 = new TimeSpan(05, 30, 0);
        private TimeSpan _time0630 = new TimeSpan(06, 30, 0);
        #endregion

        public PerformanceDomainService(IDailyEquipmentRepository dailyEquipmentRepository,
            ITargetSettingDomainService targetSettingDomainService,
            IEquipmentDomainService equipmentDomainService,
            ILineTTRepository lineTTRepository,
            IOptionDomainService optionDomainService,
            IDefinitionNodeDescRepository definitionNodeDescRepository,
            ILcmProductRepository lcmProductRepository,
            IEfficiencySettingRepository efficiencySettingRepository,
            IDailyEfficiencyRepository dailyEfficiencyRepository,
            ITakeBackWTRepository takeBackWTRepository,
            IReclaimWTRptRepository reclaimWTRptRepository)
        {
            _dailyEquipmentRepository = dailyEquipmentRepository;
            _targetSettingDomainService = targetSettingDomainService;
            _equipmentDomainService = equipmentDomainService;
            _lineTTRepository = lineTTRepository;
            _optionDomainService = optionDomainService;
            _definitionNodeDescRepository = definitionNodeDescRepository;
            _lcmProductRepository = lcmProductRepository;
            _efficiencySettingRepository = efficiencySettingRepository;
            _dailyEfficiencyRepository = dailyEfficiencyRepository;
            _takeBackWTRepository = takeBackWTRepository;
            _reclaimWTRptRepository = reclaimWTRptRepository;
        }

        public List<PassQtyEntity> GetList(string mfgDTE = "", string prodList = "1206", string shift = "", string nodeAryStr = "", int ownerId = 1)
        {
            DateTime _nowTime = DateTime.Now;
            DateTime _startTime = new DateTime();
            DateTime _endTime = new DateTime();
            Dictionary<int, string> _nodeDic = new Dictionary<int, string>();
            mfgDTE = string.IsNullOrEmpty(mfgDTE) ? _nowTime.ToString("yyyy-MM-dd") : mfgDTE;
            var _mfgDteEnd = DateTime.Parse(mfgDTE).AddDays(1).ToString("yyyy-MM-dd");
            shift = string.IsNullOrEmpty(shift)
                    ? _nowTime.TimeOfDay >= _time0730 && _nowTime.TimeOfDay < _time1930 ? "A" : "B"
                    : shift;

            switch (shift)
            {
                case "A":
                    _startTime = DateTime.Parse($"{mfgDTE} 07:30:00");
                    _endTime = DateTime.Parse($"{mfgDTE} 19:29:59");
                    break;
                case "B":
                    _startTime = DateTime.Parse($"{mfgDTE} 19:30:00");
                    _endTime = DateTime.Parse($"{_mfgDteEnd} 07:29:59");
                    break;
            }

            if (string.IsNullOrEmpty(nodeAryStr))
                _nodeDic = _defaultNodeDic;
            else
            {
                var _defNodeList = _definitionNodeDescRepository.SelectByConditions();
                _nodeDic = (from node in nodeAryStr.Split(",").Select(node => Convert.ToInt32(node)).ToList()
                            join defNode in _defNodeList
                            on node equals defNode.EqNo
                            select new
                            {
                                defNode.EqNo,
                                defNode.Descr
                            }).ToDictionary(dic => dic.EqNo, dic => dic.Descr);

                //_defNodeList.Where(w => nodeAryStr.Contains(w.Key)).ToList().ForEach(f => _nodeDic.Add(Convert.ToInt32(f.Key), f.Value));
            }

            List<int> _prodOptionList = (prodList ?? "1206").Split(",").Select(s => Convert.ToInt32(s)).ToList();

            List<(int, string)> _allLcmProdOptions = _optionDomainService.GetLcmProdDescOptions().SelectMany(sm => sm.Item2).ToList();

            _allLcmProdOptions = _allLcmProdOptions.Where(option => _prodOptionList.Contains(option.Item1)).ToList();


            var _targetSettingList = _targetSettingDomainService.GetList(prodSn: _prodOptionList, nodeList: _nodeDic.Select(s => s.Key).ToList());

            var _eqpHistoryList =
                _equipmentDomainService.GetEntityHistoryDetail(_startTime, _endTime, _targetSettingList.Where(we => we.DownEquipment != "").Select(s => s.DownEquipment).ToList(), _prodOptionList);

            List<PassQtyEntity> _response = new List<PassQtyEntity>();
            var url = "http://zipsum/modreport/Report/SHOPMOD/OperPerfDetail8.asp?";

            //foreach (var node in _nodeDic)
            //{
            Parallel.ForEach(_nodeDic, (node) =>
            {
                List<PerformanceDetailEntity> _performanceDetail = new List<PerformanceDetailEntity>();

                Dictionary<string, List<string>> _prodEqDic = new Dictionary<string, List<string>>();

                foreach (string prod in _allLcmProdOptions.Select(s => s.Item2.Split("-")[0]))
                {
                    _prodEqDic.Add(prod, GetNodeAllEquiomentNo($"{node.Key}-{node.Value}", prod, shift, mfgDTE, _mfgDteEnd, ownerId));
                }

                //foreach (var prodEq in _prodEqDic)
                //{

                Parallel.ForEach(_prodEqDic, (prodEq) =>
                {
                    foreach (var eq in prodEq.Value)
                    {
                        string _qStr = $"Shop=MOD4&G_FAC=6&StrSql_w=+and+prod_nbr+in+('{prodEq.Key}')+and+lcm_owner+in+({_ownerDic[ownerId]})&StrSql_w4=&col0=&col1=&row0={prodEq.Key}" +
                                        $"&row2={eq}&row3={shift}&row1={node.Key}&sql_m=+and+acct_date+>=+'{mfgDTE}'+and+acct_date+<=+'{mfgDTE}'" +
                                        $"&sql_m1=+and+trans_date+>=+'{mfgDTE}+07:30:00.000000'+and+trans_date+<=+'{_mfgDteEnd}+07:30:00.000000'" +
                                        $"&sqlbu2=+and+shift_id='{shift}'&vdate_s=&vdate_e=";

                        var data = new StringContent(_qStr, Encoding.UTF8, "application/x-www-form-urlencodedn");

                        string[] array;

                        using (var client = new HttpClient())
                        {
                            //var response = client.PostAsync(url, data);
                            var response = client.PostAsync(url + _qStr, data).Result;
                            //var response = client.GetAsync(url);
                            response.Content.Headers.ContentType.CharSet = "Big5";

                            string result = response.Content.ReadAsStringAsync().Result;

                            result = result.Remove(0, 12200);

                            array = result.Split("<SCRIPT LANGUAGE=vbscript >");

                        }

                        //string text = File.ReadAllText("D:\\response.txt");

                        //array = text.Split("<SCRIPT LANGUAGE=vbscript >");

                        var cnt = (array.Count() - 3) / 14;

                        for (int i = 0; i < cnt; i++)
                        {
                            _performanceDetail.Add(Process(array.Skip((i * 14) + 1).Take(14).ToArray(), node.Key));
                        }
                    }
                });

                //}

                PassQtyEntity _tResponse = new PassQtyEntity();

                //var _currentLineTT = _lineTTList.Where(w => _dailyEqpList.FirstOrDefault(daily => daily.Node == _dailyEqp.Node).Equipments.Split(",").Contains(w.line))
                //        .Select(s => new LineTTEntity
                //        {
                //            sn = s.sn,
                //            line = s.line,
                //            MFG_Day = s.MFG_Day,
                //            MFG_HR = s.MFG_HR,
                //            Line_TT = s.Line_TT
                //        }).ToList();

                var _currentProdTarget = _targetSettingList.Where(w => w.Node == node.Key).ToList();

                var _lineSetting = new TargetSettingEntity
                {
                    Node = node.Key,
                    Descr = string.Join("/", _currentProdTarget.Select(s => s.Descr)),
                    DownEquipment = string.Join(",", _currentProdTarget.Select(s => s.DownEquipment)),
                    Time0730 = _currentProdTarget.Sum(s => s.Time0730),
                    Time0830 = _currentProdTarget.Sum(s => s.Time0830),
                    Time0930 = _currentProdTarget.Sum(s => s.Time0930),
                    Time1030 = _currentProdTarget.Sum(s => s.Time1030),
                    Time1130 = _currentProdTarget.Sum(s => s.Time1130),
                    Time1230 = _currentProdTarget.Sum(s => s.Time1230),
                    Time1330 = _currentProdTarget.Sum(s => s.Time1330),
                    Time1430 = _currentProdTarget.Sum(s => s.Time1430),
                    Time1530 = _currentProdTarget.Sum(s => s.Time1530),
                    Time1630 = _currentProdTarget.Sum(s => s.Time1630),
                    Time1730 = _currentProdTarget.Sum(s => s.Time1730),
                    Time1830 = _currentProdTarget.Sum(s => s.Time1830),
                    Time1930 = _currentProdTarget.Sum(s => s.Time1930),
                    Time2030 = _currentProdTarget.Sum(s => s.Time2030),
                    Time2130 = _currentProdTarget.Sum(s => s.Time2130),
                    Time2230 = _currentProdTarget.Sum(s => s.Time2230),
                    Time2330 = _currentProdTarget.Sum(s => s.Time2330),
                    Time0030 = _currentProdTarget.Sum(s => s.Time0030),
                    Time0130 = _currentProdTarget.Sum(s => s.Time0130),
                    Time0230 = _currentProdTarget.Sum(s => s.Time0230),
                    Time0330 = _currentProdTarget.Sum(s => s.Time0330),
                    Time0430 = _currentProdTarget.Sum(s => s.Time0430),
                    Time0530 = _currentProdTarget.Sum(s => s.Time0530),
                    Time0630 = _currentProdTarget.Sum(s => s.Time0630),
                    TimeTarget = _currentProdTarget.Sum(s => s.TimeTarget)
                };

                var _eqpHistory = _eqpHistoryList.Where(w => _lineSetting.DownEquipment.Contains(w.ToolId)).ToList();

                _tResponse.Node = node.Key;
                _tResponse.NodeName = node.Value;
                _tResponse.DetailList = new List<PassQtyDetailEntity>();

                if (shift == "A")
                {
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay >= _time0730 && w.Time.TimeOfDay < _time0830)?.ToList(), "0730",
                                    _lineSetting.Time0730, _lineSetting.TimeTarget, _eqpHistory.Where(w => w.LmTime.TimeOfDay >= _time0730 && w.LmTime.TimeOfDay < _time0830).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay >= _time0830 && w.Time.TimeOfDay < _time0930)?.ToList(), "0830",
                                    _lineSetting.Time0830, _lineSetting.TimeTarget, _eqpHistory.Where(w => w.LmTime.TimeOfDay >= _time0830 && w.LmTime.TimeOfDay < _time0930).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay >= _time0930 && w.Time.TimeOfDay < _time1030)?.ToList(), "0930",
                                    _lineSetting.Time0930, _lineSetting.TimeTarget, _eqpHistory.Where(w => w.LmTime.TimeOfDay >= _time0930 && w.LmTime.TimeOfDay < _time1030).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay >= _time1030 && w.Time.TimeOfDay < _time1130)?.ToList(), "1030",
                                    _lineSetting.Time1030, _lineSetting.TimeTarget, _eqpHistory.Where(w => w.LmTime.TimeOfDay >= _time1030 && w.LmTime.TimeOfDay < _time1130).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay >= _time1130 && w.Time.TimeOfDay < _time1230)?.ToList(), "1130",
                                    _lineSetting.Time1130, _lineSetting.TimeTarget, _eqpHistory.Where(w => w.LmTime.TimeOfDay >= _time1130 && w.LmTime.TimeOfDay < _time1230).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay >= _time1230 && w.Time.TimeOfDay < _time1330)?.ToList(), "1230",
                                    _lineSetting.Time1230, _lineSetting.TimeTarget, _eqpHistory.Where(w => w.LmTime.TimeOfDay >= _time1230 && w.LmTime.TimeOfDay < _time1330).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay >= _time1330 && w.Time.TimeOfDay < _time1430)?.ToList(), "1330",
                                    _lineSetting.Time1330, _lineSetting.TimeTarget, _eqpHistory.Where(w => w.LmTime.TimeOfDay >= _time1330 && w.LmTime.TimeOfDay < _time1430).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay >= _time1430 && w.Time.TimeOfDay < _time1530)?.ToList(), "1430",
                                    _lineSetting.Time1430, _lineSetting.TimeTarget, _eqpHistory.Where(w => w.LmTime.TimeOfDay >= _time1430 && w.LmTime.TimeOfDay < _time1530).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay >= _time1530 && w.Time.TimeOfDay < _time1630)?.ToList(), "1530",
                                    _lineSetting.Time1530, _lineSetting.TimeTarget, _eqpHistory.Where(w => w.LmTime.TimeOfDay >= _time1530 && w.LmTime.TimeOfDay < _time1630).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay >= _time1630 && w.Time.TimeOfDay < _time1730)?.ToList(), "1630",
                                    _lineSetting.Time1630, _lineSetting.TimeTarget, _eqpHistory.Where(w => w.LmTime.TimeOfDay >= _time1630 && w.LmTime.TimeOfDay < _time1730).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay >= _time1730 && w.Time.TimeOfDay < _time1830)?.ToList(), "1730",
                                    _lineSetting.Time1730, _lineSetting.TimeTarget, _eqpHistory.Where(w => w.LmTime.TimeOfDay >= _time1730 && w.LmTime.TimeOfDay < _time1830).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay >= _time1830 && w.Time.TimeOfDay < _time1930)?.ToList(), "1830",
                                    _lineSetting.Time1830, _lineSetting.TimeTarget, _eqpHistory.Where(w => w.LmTime.TimeOfDay >= _time1830 && w.LmTime.TimeOfDay < _time1930).ToList()));
                }
                else
                {
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay >= _time1930 && w.Time.TimeOfDay < _time2030)?.ToList(), "1930",
                                    _lineSetting.Time1930, _lineSetting.TimeTarget, _eqpHistory.Where(w => w.LmTime.TimeOfDay >= _time1930 && w.LmTime.TimeOfDay < _time2030).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay >= _time2030 && w.Time.TimeOfDay < _time2130)?.ToList(), "2030",
                                    _lineSetting.Time2030, _lineSetting.TimeTarget, _eqpHistory.Where(w => w.LmTime.TimeOfDay >= _time2030 && w.LmTime.TimeOfDay < _time2130).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay >= _time2130 && w.Time.TimeOfDay < _time2230)?.ToList(), "2130",
                                    _lineSetting.Time2130, _lineSetting.TimeTarget, _eqpHistory.Where(w => w.LmTime.TimeOfDay >= _time2130 && w.LmTime.TimeOfDay < _time2230).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay >= _time2230 && w.Time.TimeOfDay < _time2330)?.ToList(), "2230",
                                    _lineSetting.Time2230, _lineSetting.TimeTarget, _eqpHistory.Where(w => w.LmTime.TimeOfDay >= _time2230 && w.LmTime.TimeOfDay < _time2330).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay >= _time2330 && w.Time.TimeOfDay < _time0030)?.ToList(), "2330",
                                    _lineSetting.Time2330, _lineSetting.TimeTarget, _eqpHistory.Where(w => w.LmTime.TimeOfDay >= _time2330 && w.LmTime.TimeOfDay < _time0030).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay >= _time0030 && w.Time.TimeOfDay < _time0130)?.ToList(), "0030",
                                    _lineSetting.Time0030, _lineSetting.TimeTarget, _eqpHistory.Where(w => w.LmTime.TimeOfDay >= _time0030 && w.LmTime.TimeOfDay < _time0130).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay >= _time0130 && w.Time.TimeOfDay < _time0230)?.ToList(), "0130",
                                    _lineSetting.Time0130, _lineSetting.TimeTarget, _eqpHistory.Where(w => w.LmTime.TimeOfDay >= _time0130 && w.LmTime.TimeOfDay < _time0230).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay >= _time0230 && w.Time.TimeOfDay < _time0330)?.ToList(), "0230",
                                    _lineSetting.Time0230, _lineSetting.TimeTarget, _eqpHistory.Where(w => w.LmTime.TimeOfDay >= _time0230 && w.LmTime.TimeOfDay < _time0330).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay >= _time0330 && w.Time.TimeOfDay < _time0430)?.ToList(), "0330",
                                    _lineSetting.Time0330, _lineSetting.TimeTarget, _eqpHistory.Where(w => w.LmTime.TimeOfDay >= _time0330 && w.LmTime.TimeOfDay < _time0430).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay >= _time0430 && w.Time.TimeOfDay < _time0530)?.ToList(), "0430",
                                    _lineSetting.Time0430, _lineSetting.TimeTarget, _eqpHistory.Where(w => w.LmTime.TimeOfDay >= _time0430 && w.LmTime.TimeOfDay < _time0530).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay >= _time0530 && w.Time.TimeOfDay < _time0630)?.ToList(), "0530",
                                    _lineSetting.Time0530, _lineSetting.TimeTarget, _eqpHistory.Where(w => w.LmTime.TimeOfDay >= _time0530 && w.LmTime.TimeOfDay < _time0630).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay >= _time0630 && w.Time.TimeOfDay < _time0730)?.ToList(), "0630",
                                    _lineSetting.Time0630, _lineSetting.TimeTarget, _eqpHistory.Where(w => w.LmTime.TimeOfDay >= _time0630 && w.LmTime.TimeOfDay < _time0730).ToList()));
                }

                _response.Add(_tResponse);

            });
            //};

            _logHelper.WriteLog(LogLevel.Info, this.GetType().Name, $"使用者查詢");

            return _response.OrderBy(ob => ob.Node).ToList();
        }

        private PerformanceDetailEntity Process(string[] detailStr, int node)
        {
            PerformanceDetailEntity _entity = new PerformanceDetailEntity();

            string[] _tempArray;
            string _tempStr;

            if (_passTransType.Contains(detailStr[3].Split("=")[2].Split("\r\n")[0].Replace("\"", "")))
            {
                _tempArray = detailStr[3].Split("=");
                _tempArray = _tempArray[2].Split("\r\n");
                _tempStr = _tempArray[0].Replace("\"", "");

                for (int i = 0; i < 15; i++)
                {
                    switch (i)
                    {
                        case 0:
                            _tempArray = detailStr[i].Split("=");
                            _tempArray = _tempArray[2].Split("\r\n");
                            _tempStr = _tempArray[0].Replace("\"", "");
                            _entity.Prod = _tempStr;
                            break;
                        case 2:
                            _tempArray = detailStr[i].Split("=");
                            _tempArray = _tempArray[2].Split("\r\n");
                            _tempStr = _tempArray[0].Replace("\"", "");
                            DateTime _outTime;
                            DateTime.TryParse(_tempStr, out _outTime);
                            _entity.Time = _outTime;
                            break;
                        case 4:
                            _tempArray = detailStr[i].Split("=");
                            _tempArray = _tempArray[2].Split("\r\n");
                            _tempStr = _tempArray[0].Replace("\"", "");
                            _entity.Equipment = _tempStr;
                            break;
                        case 12:
                            _tempArray = detailStr[i].Split("=");
                            _tempArray = _tempArray[2].Split("\r\n");
                            _tempStr = _tempArray[0].Replace("\"", "");
                            _entity.NG = _tempStr == "_" ? 0 : 1;
                            break;
                    }
                }

                _entity.Qty = 1;
                _entity.Node = node;

            }

            return _entity;
        }


        private PassQtyDetailEntity GroupDetail(
            List<PerformanceDetailEntity> entityList,
            string _time,
            int targetQty,
            int timeTarget,
            List<EquipmentEntity> eqpEntities)
        //,List<LineTTEntity> lineTTList)
        {
            PassQtyDetailEntity _result = new PassQtyDetailEntity
            {
                Time = _time,
                Target = targetQty,
                TimeTarget = timeTarget,
                EqHistoryList = new List<EqHistory>()
            };

            eqpEntities = eqpEntities.OrderByDescending(od => Convert.ToDouble(od.RepairTime)).ToList();

            var _top1 = eqpEntities.Take(1).FirstOrDefault();
            var _top2 = eqpEntities.Skip(1).Take(1).FirstOrDefault();
            var _top3 = eqpEntities.Skip(2).Take(1).FirstOrDefault();

            if (entityList == null || !entityList.Any())
            {
                _result.InQty = 0;
                _result.NGQty = 0;
                _result.OKQty = 0;
                _result.Yield = "0.00%";
                _result.DefectRate = "0.00%";
                _result.Diff = targetQty * -1;
            }
            else
            {
                _result.InQty = entityList.Count;
                _result.NGQty = entityList.Sum(sum => sum.NG);
                _result.OKQty = entityList.Count - entityList.Sum(sum => sum.NG);
                _result.Yield = _result.InQty == 0 ? "0.00%" : (((float)_result.OKQty / (float)_result.InQty) * 100).ToString("0.00") + "%";
                _result.DefectRate = _result.InQty == 0 ? "0.00%" : (((float)_result.NGQty / (float)_result.InQty) * 100).ToString("0.00") + "%";
                _result.Diff = _result.InQty - _result.Target;
            }

            eqpEntities.ForEach(eqInfo =>
            {
                _result.EqHistoryList.Add(new EqHistory
                {
                    Status = eqInfo.IEStatusDesc,
                    Desc = string.IsNullOrEmpty(eqInfo.Comment.Trim()) ? eqInfo.StatusCdsc : eqInfo.Comment,
                    Time = Convert.ToDecimal(eqInfo.RepairTime).ToString("0.#")
                });
            });

            //_result.eqpInfoTOP1 = _top1 == null ? "" : $"{_top1.StatusCdsc} {_top1.Comment} ({_top1.RepairTime} min.)";
            //_result.eqpInfoTOP2 = _top2 == null ? "" : $"{_top1.StatusCdsc} {_top2.Comment} ({_top2.RepairTime} min.)";
            //_result.eqpInfoTOP3 = _top3 == null ? "" : $"{_top1.StatusCdsc} {_top3.Comment} ({_top3.RepairTime} min.)";

            return _result;
        }


        private List<string> GetNodeAllEquiomentNo(string node, string prodList, string shift, string startDTE, string endDTE, int ownerId)
        {
            List<string> _nodeEqList = new List<string>();

            string _qStr = $"Shop=MOD4&G_FAC=6&StrSql_w2={(string.IsNullOrEmpty(prodList) ? "" : $"+And+Prod_Nbr+in+('{prodList}')")}+and+lcm_owner+in+({_ownerDic[ownerId]})+and++work_ctr+>+1000++" +
                $"&StrSql_w1={(string.IsNullOrEmpty(prodList) ? "" : $"+And+Prod_Nbr+in+('{prodList}')")}+and+lcm_owner+in+({_ownerDic[ownerId]})+and++to_wc+>+1000+++and+shift_id='A'" +
                $"&StrSql_w={(string.IsNullOrEmpty(prodList) ? "" : $"+And+Prod_Nbr+in+('{prodList}')")}+and+lcm_owner+in+({_ownerDic[ownerId]})&StrSql_tvset=&col0=Pass+Q%27ty&col1=&row0=" +
                $"&row1={node}&sql_p=&sql_m=+and+acct_date+>='{startDTE}'+and+acct_date+<=+'{endDTE}'" +
                $"&sql_m1=+and+trans_date+%3E%3D+%27{startDTE}+07%3A30%3A00.000000%27+and+trans_date+%3C%3D+%272022-11-30+07%3A30%3A00.000000%27&sqlbu1=&sqlbu2=+and+shift_id%3D%27{shift}%27" +
                $"&vdate_s={startDTE}&vdate_e={endDTE}";

            var data = new StringContent(_qStr, Encoding.UTF8, "application/x-www-form-urlencodedn");

            string[] array;

            using (var client = new HttpClient())
            {
                //var response = client.PostAsync(url, data);
                var response = client.PostAsync("http://zipsum/modreport/Report/SHOPMOD/OperPerfDetail3.asp?" + _qStr, data).Result;
                //var response = client.GetAsync(url);
                response.Content.Headers.ContentType.CharSet = "Big5";

                string result = response.Content.ReadAsStringAsync().Result;

                result = result.Remove(0, 10000);

                array = result.Split("<SCRIPT LANGUAGE=vbscript >");

            }

            for (int i = 1; i < array.Length - 1; i++)
            {
                var _eq = array[i].Split("\r\n");
                _eq = _eq[4].Split("=");
                _nodeEqList.Add(_eq[1].Replace("\"", ""));
            }

            return _nodeEqList;
        }

        private void Get106NewReport()
        {
            string _qStr = "apiJob=[{'name':'Date','apiName':'TN_OperationPerformance','FactoryType':'CARUX','FacId':'A','Building':'A','DateFrom':'2023-07-05','DateTo':'2023-07-05','Shift':'ALL','Floor':'ALL','WorkOrder':'','LcmProductType':'ALL','Size':'ALL','BigProduct':'ALL','LcmOwner':'','LcdGrade':'','Product':'ALL','ProdId':'','OptionProduct':'GDD340IA0090S','#optionmenu':'','prod_nbr':'GDD340IA0090S','owner_code':\"RES0','QTAP','PROD','PRDG','LCME\"}]";

            var data = new StringContent(_qStr, Encoding.UTF8, "text/plain");
            data.Headers.Add("Reporttoken", "VE5VSTIyMDA4MTYzMjAyMy0wNy0wNg==");

            using (var client = new HttpClient())
            {
                var response = client.PostAsync("http://ptnreportapi.innolux.com/SQLAgent/ApiWorkMulti?" + _qStr, data);

                var _tesRes = response.Result.Content.ReadAsStringAsync().Result;
            }
        }

        #region Efficiency

        public List<EfficiencyEntity> GetDailyEfficiencyList(string mfgDate, int floor = 3)
        {
            try
            {
                DateTime _mfgDTE = string.IsNullOrEmpty(mfgDate) ? DateTime.Now.AddDays(-1) : DateTime.Parse(mfgDate);

                List<EfficiencyEntity> _response = new List<EfficiencyEntity>();
                List<DailyPerformanceDao> _dailyEffList = _dailyEfficiencyRepository.SelectByConditions(_mfgDTE.Date, floor);

                _response = _dailyEffList.GroupBy(g => g.ProcessId).Select(eff =>
                {
                    var _eff = eff.Where(w => w.LcmProdSn != 0).OrderByDescending(od => od.LcmProdSn).ThenBy(o => o.Shift).ToList();
                    var _ttlSummary = eff.FirstOrDefault(w => w.LcmProdSn == 0 && w.Shift == "");

                    EfficiencyEntity _tmp = new EfficiencyEntity
                    {
                        Floor = floor.ToString(),
                        Process = eff.Key.GetDescription(),
                        TTLcount = eff.Count() + 3,
                        TTLPassQty = _ttlSummary.PassQty,
                        EfficiencyInlineTTL = _ttlSummary.InlineEfficiency,
                        EfficiencyInlineOfflineTTL = _ttlSummary.InOfflineEfficiency,
                        InfoList = _eff.Select(s => new EfficiencyDetailEntity
                        {
                            ProdNo = s.ProdNo,
                            PassQty = s.PassQty,
                            Shift = s.Shift == "A" ? "日" : "夜",
                            StartTime = s.OpenTime?.ToString("yyyy/MM/dd HH:mm:ss") ?? "",
                            EndTime = s.CloseTime?.ToString("yyyy/MM/dd HH:mm:ss") ?? "",
                            EfficiencyInline = s.InlineEfficiency == 0 ? 0 : s.InlineEfficiency,
                            EfficiencyInlineOffline = s.InOfflineEfficiency == 0 ? 0 : s.InOfflineEfficiency,
                            MedianTT = s.MedianTT.ToString("0.0")
                        }).ToList()
                    };

                    //var _tmpAList = _tmp.InfoList.Where(w => w.Shift == "日");
                    //var _tmpBList = _tmp.InfoList.Where(w => w.Shift == "夜");

                    _tmp.InfoList.AddRange(eff.Where(w => w.LcmProdSn == 0 && w.Shift == "A").Select(s => new EfficiencyDetailEntity
                    {
                        ProdNo = "TTL",
                        PassQty = s.PassQty,
                        Shift = "日",
                        StartTime = "",
                        EndTime = "",
                        EfficiencyInline = s.InlineEfficiency == 0 ? 0 : s.InlineEfficiency,
                        EfficiencyInlineOffline = s.InOfflineEfficiency == 0 ? 0 : s.InOfflineEfficiency,
                        MedianTT = ""
                    }));

                    _tmp.InfoList.AddRange(eff.Where(w => w.LcmProdSn == 0 && w.Shift == "B").Select(s => new EfficiencyDetailEntity
                    {
                        ProdNo = "TTL",
                        PassQty = s.PassQty,
                        Shift = "夜",
                        StartTime = "",
                        EndTime = "",
                        EfficiencyInline = s.InlineEfficiency == 0 ? 0 : s.InlineEfficiency,
                        EfficiencyInlineOffline = s.InOfflineEfficiency == 0 ? 0 : s.InOfflineEfficiency,
                        MedianTT = ""
                    }));

                    return _tmp;

                }).ToList();

                return _response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<EfficiencySettingEntity> GetEfficiencySetting(int floor)
        {
            var _effSettings = _efficiencySettingRepository.SelectByConditions(floor: floor);

            var _defProdList = _lcmProductRepository.SelectByConditions();

            var _effSettingEntity = (from setting in _effSettings
                                     join def in _defProdList
                                     on setting.LcmProdSn equals def.sn
                                     select new EfficiencySettingEntity
                                     {
                                         LcmProdSn = setting.LcmProdSn,
                                         ProdNo = def.ProdNo,
                                         ProcessId = setting.ProcessId,
                                         Shift = setting.Shift,
                                         Process = setting.ProcessId.GetDescription(),
                                         Node = setting.Node,
                                         WT = setting.WT,
                                         InlineEmps = setting.InlineEmps,
                                         OfflineEmps = setting.OfflineEmps,
                                         Floor = setting.Floor
                                     }).ToList();

            var _prodSnList = _effSettingEntity.GroupBy(g => g.LcmProdSn).Select(s => s.Key);

            var _nonSettingProd = _defProdList.Where(w => !_prodSnList.Contains(w.sn));

            foreach (var prodDef in _nonSettingProd)
                for (int i = 1; i < 6; i++)
                    for (int j = 0; j < 2; j++)
                    {
                        _effSettingEntity.Add(new EfficiencySettingEntity
                        {
                            LcmProdSn = prodDef.sn,
                            ProdNo = prodDef.ProdNo,
                            ProcessId = (ProcessEnum)i,
                            Shift = j == 0 ? "A" : "B",
                            Process = ((ProcessEnum)i).GetDescription(),
                            Node = 0,
                            WT = 0,
                            InlineEmps = 0,
                            OfflineEmps = 0,
                            Floor = floor
                        });
                    }

            return _effSettingEntity;
        }

        public string UpdateEfficiencySetting(List<EfficiencySettingEntity> updEntity, UserEntity userEntity)
        {
            try
            {
                string _updResult = "";
                DateTime _nowTime = DateTime.Now;


                if (updEntity.Any(a => a.LcmProdSn == 0 || a.Node == 0))
                    return "";

                List<EfficiencySettingDao> _updEffList = updEntity.Select(eff => new EfficiencySettingDao
                {
                    ProcessId = eff.ProcessId,
                    Shift = eff.Shift,
                    Floor = eff.Floor,
                    Node = eff.Node,
                    LcmProdSn = eff.LcmProdSn,
                    WT = eff.WT,
                    InlineEmps = eff.InlineEmps,
                    OfflineEmps = eff.OfflineEmps,
                    UpdateTime = _nowTime,
                    UpdateUser = userEntity.Name
                }).ToList();

                using (TransactionScope _scope = new TransactionScope())
                {
                    var _res = false;

                    _res = _efficiencySettingRepository.UpdateSetting(_updEffList) == _updEffList.Count;

                    if (_res)
                        _scope.Complete();
                    else
                        _updResult = "更新異常";
                }

                return _updResult;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void BatchInsert()
        {
            DateTime _now = DateTime.Now;

            var _defProdList = _lcmProductRepository.SelectByConditions();

            List<EfficiencySettingDao> _insList = new List<EfficiencySettingDao>();

            int[] _floor = new int[] { 2, 3 };

            foreach (LcmProductDao dao in _defProdList)
            {
                _insList.Add(new EfficiencySettingDao
                {
                    LcmProdSn = dao.sn,
                    Node = 1100,
                    ProcessId = ProcessEnum.BOND,
                    WT = 200,
                    InlineEmps = 5,
                    OfflineEmps = 2,
                    UpdateTime = _now,
                    UpdateUser = "admin"
                });
                _insList.Add(new EfficiencySettingDao
                {
                    LcmProdSn = dao.sn,
                    Node = 1330,
                    ProcessId = ProcessEnum.FOG,
                    WT = 200,
                    InlineEmps = 5,
                    OfflineEmps = 2,
                    UpdateTime = _now,
                    UpdateUser = "admin"
                });
                _insList.Add(new EfficiencySettingDao
                {
                    LcmProdSn = dao.sn,
                    Node = 1355,
                    ProcessId = ProcessEnum.LAM,
                    WT = 200,
                    InlineEmps = 5,
                    OfflineEmps = 2,
                    UpdateTime = _now,
                    UpdateUser = "admin"
                });
                _insList.Add(new EfficiencySettingDao
                {
                    LcmProdSn = dao.sn,
                    Node = 1415,
                    ProcessId = ProcessEnum.ASSY,
                    WT = 200,
                    InlineEmps = 5,
                    OfflineEmps = 2,
                    UpdateTime = _now,
                    UpdateUser = "admin"
                });
                _insList.Add(new EfficiencySettingDao
                {
                    LcmProdSn = dao.sn,
                    Node = 1600,
                    ProcessId = ProcessEnum.CDP,
                    WT = 200,
                    InlineEmps = 5,
                    OfflineEmps = 2,
                    UpdateTime = _now,
                    UpdateUser = "admin"
                });
            }


            using (TransactionScope _scope = new TransactionScope())
            {
                _efficiencySettingRepository.Insert(_insList);
                _scope.Complete();
            }


        }

        #endregion

        #region take back work time

        public List<TakeBackWTEntity> GetTBWTList(DateTime? searchDate, WTCategoryEnum wtCategoryId)
        {
            try
            {
                DateTime _processDate = DateTime.Now.AddDays(-1);

                if (searchDate.HasValue)
                {
                    _processDate = (DateTime)searchDate;
                }

                var _tbMainList = _takeBackWTRepository.SelectByConditions(_processDate.Date, wtCatgIdList: (wtCategoryId == 0 ? null : new List<WTCategoryEnum> { wtCategoryId }));
                var _tbDetailList = _takeBackWTRepository.SelectDetailByConditions(_tbMainList.Select(t => t.Sn).ToList());
                var _tbAttendanceList = _takeBackWTRepository.SelectAttendanceByConditions(_tbMainList.Select(t => t.Sn).ToList());

                var _res = _tbMainList.Select(s => new TakeBackWTEntity
                {
                    Sn = s.Sn,
                    Date = s.ProcessDate.ToString("yyyy-MM-dd"),
                    WTCategoryId = s.WTCategoryId,
                    TakeBackBonding = s.BondTakeBack.ToString("0.00"),
                    TakeBackFOG = s.FogTakeBack.ToString("0.00"),
                    TakeBackLAM = s.LamTakeBack.ToString("0.00"),
                    TakeBackASSY = s.AssyTakeBack.ToString("0.00"),
                    TakeBackCDP = s.CDPTakeBack.ToString("0.00"),
                    TakeBackPercent = s.TakeBackPercent.ToString("0.00") + "%",
                    TotalTakeBack = s.TTLTakeBack.ToString("0.00"),
                    DetailList = _tbDetailList.Where(w => w.TakeBackWtSn == s.Sn).Select(detail => new TakeBackWTProdEntity
                    {
                        TakeBackWTSn = detail.TakeBackWtSn,
                        ProcessId = detail.ProcessId,
                        EqId = detail.EqId,
                        ProdId = detail.Prod,
                        Prod = "",
                        IEStandard = detail.IEStandard.ToString("0.00"),
                        IETT = detail.IETT.ToString("0.00"),
                        IEWT = detail.IEWT.ToString("0.00"),
                        PassQty = detail.PassQty.ToString("0"),
                        TakeBackTime = detail.TakeBackWT.ToString("0.00")
                    }).ToList(),
                    AttendanceList = _tbAttendanceList.Where(w => w.TakeBackWtSn == s.Sn).Select(atten => new TakeBackAttendanceEntity
                    {
                        TakeBackWTSn = atten.TakeBackWtSn,
                        Country = atten.CountryId.GetDescription(),
                        CountryId = atten.CountryId,
                        ShouldPresentCnt = atten.ShouldPresentCnt,
                        OverTimeCnt = atten.OverTimeCnt,
                        AcceptSupCnt = atten.AcceptSupCnt,
                        HaveDayOffCnt = atten.HaveDayOffCnt,
                        OffCnt = atten.OffCnt,
                        Support = atten.Support,
                        PresentCnt = atten.PresentCnt,
                        TotalWorkTime = atten.TotalWorkTime
                    }).ToList()
                }).ToList();

                return _res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public (string, TakeBackWTEntity) UpdateTakeBackWT(TakeBackWTEntity editEntity, UserEntity userInfo)
        {
            try
            {
                string _updResult = "";
                DateTime _nowTime = DateTime.Now;
                DateTime _proccessDate = DateTime.Now;
                List<INXRpt106SubEntity> _rpt106List = new List<INXRpt106SubEntity>();
                TakeBackWTDao _takeBackWT = new TakeBackWTDao();
                List<TakeBackWTDetailDao> _takeBackWTDetail = new List<TakeBackWTDetailDao>();
                List<TakeBackWTAttendanceDao> _takeBackWTAttendance = new List<TakeBackWTAttendanceDao>();
                string _shift = editEntity.WTCategoryId.ToString().Substring(editEntity.WTCategoryId.ToString().Length - 1, 1);

                if (DateTime.TryParseExact(editEntity.Date, "yyyy-MM-dd", null, DateTimeStyles.None, out _))
                    _proccessDate = DateTime.Parse(editEntity.Date);
                else
                    throw new Exception("日期異常");

                var _prodInfoList = _lcmProductRepository.SelectByConditions(snList: editEntity.DetailList.Select(s => s.ProdId).ToList());
                var _reclaimRpt = _reclaimWTRptRepository.SelectByConditions(editEntity.DetailList.Select(s => s.EqId).ToList(), _prodInfoList.Select(p => p.ProdNo).ToList());
                editEntity.DetailList.ForEach(f => f.Prod = _prodInfoList.FirstOrDefault(w => w.sn == f.ProdId).ProdNo);

                Parallel.ForEach(
                    _processNodeDic,
                    new ParallelOptions { MaxDegreeOfParallelism = 5 },
                    (dic) =>
                    {
                        var _tmp = (from detail in editEntity.DetailList.Where(w => w.ProcessId == dic.Key).ToList()
                                    join defProd in _prodInfoList
                                    on detail.ProdId equals defProd.sn
                                    select defProd.ProdNo).ToList();

                        foreach (int node in dic.Value)
                        {
                            _rpt106List.AddRange(_inxReportService.Get106NewReportSubAsync<INXRpt106SubEntity>(_proccessDate, _proccessDate, _shift, node, "2", _tmp).Result.Date.Data.Table);
                        }
                    }
                );

                foreach (var detail in editEntity.DetailList)
                {
                    var _tmpCurrReclaim = _reclaimRpt.Where(w => w.equip_id == detail.EqId && w.mes_prod_id == detail.Prod).OrderByDescending(od => od.mfg_dt).ToList();
                    var _tmpRpt106 = _rpt106List.Where(w => w.PROD_NBR == detail.Prod && w.EQUIP_NBR == detail.EqId).ToList();
                    var _tmpTakeBack = new TakeBackWTDetailDao
                    {
                        TakeBackWtSn = detail.TakeBackWTSn,
                        EqId = detail.EqId,
                        ProcessId = detail.ProcessId,
                        Prod = detail.ProdId,
                        IEStandard = _tmpCurrReclaim.FirstOrDefault()?.base_man_eq_ratio ?? 0,
                        IETT = _tmpCurrReclaim.FirstOrDefault()?.tct_tm_tgt_meas ?? 0,
                        PassQty = Convert.ToInt32(_tmpRpt106.FirstOrDefault()?.PASS_QTY ?? 0),
                        CreateUser = userInfo.JobId,
                        CreateTime = _nowTime
                    };

                    _tmpTakeBack.IEWT = _tmpTakeBack.IEStandard * _tmpTakeBack.IETT;
                    _tmpTakeBack.TakeBackWT = _tmpTakeBack.IEStandard * _tmpTakeBack.IETT * _tmpTakeBack.PassQty;

                    _takeBackWTDetail.Add(_tmpTakeBack);
                }

                _takeBackWTAttendance = editEntity.AttendanceList.Select(atten => new TakeBackWTAttendanceDao
                {
                    TakeBackWtSn = atten.TakeBackWTSn,
                    CountryId = atten.CountryId,
                    AcceptSupCnt = atten.AcceptSupCnt,
                    ShouldPresentCnt = atten.ShouldPresentCnt,
                    OverTimeCnt = atten.OverTimeCnt,
                    Support = atten.Support,
                    HaveDayOffCnt = atten.HaveDayOffCnt,
                    OffCnt = atten.OffCnt,
                    PresentCnt = atten.PresentCnt,
                    TotalWorkTime = Convert.ToInt32(atten.PresentCnt * (atten.CountryId == CountryEnum.TW ? 10 : 9.5) * 3600),
                    CreateUser = userInfo.JobId,
                    CreateTime = _nowTime
                }).ToList();

                _takeBackWT = new TakeBackWTDao
                {
                    Sn = editEntity.Sn,
                    ProcessDate = _proccessDate,
                    WTCategoryId = (WTCategoryEnum)editEntity.WTCategoryId,
                    BondTakeBack = _takeBackWTDetail.Where(w => w.ProcessId == ProcessEnum.BOND).Sum(sum => sum.TakeBackWT),
                    FogTakeBack = _takeBackWTDetail.Where(w => w.ProcessId == ProcessEnum.FOG).Sum(sum => sum.TakeBackWT),
                    LamTakeBack = _takeBackWTDetail.Where(w => w.ProcessId == ProcessEnum.LAM).Sum(sum => sum.TakeBackWT),
                    AssyTakeBack = _takeBackWTDetail.Where(w => w.ProcessId == ProcessEnum.ASSY).Sum(sum => sum.TakeBackWT),
                    CDPTakeBack = _takeBackWTDetail.Where(w => w.ProcessId == ProcessEnum.CDP).Sum(sum => sum.TakeBackWT),
                };

                if (editEntity.Sn == 0)
                {
                    _takeBackWT.CreateUser = userInfo.JobId;
                    _takeBackWT.CreateTime = _nowTime;
                }
                else
                {
                    _takeBackWT.UpdateUser = userInfo.JobId;
                    _takeBackWT.UpdateTime = _nowTime;
                }

                _takeBackWT.TTLTakeBack = _takeBackWT.BondTakeBack + _takeBackWT.FogTakeBack + _takeBackWT.LamTakeBack + _takeBackWT.AssyTakeBack + _takeBackWT.CDPTakeBack;
                _takeBackWT.TakeBackPercent = _takeBackWT.TTLTakeBack / _takeBackWTAttendance.Sum(s => s.TotalWorkTime) * 100;

                using (TransactionScope scope = new TransactionScope())
                {
                    bool _tbRes = false;
                    bool _tbDetailRes = false;
                    bool _tbAttenRes = false;
                    int _tbWTSn = 0;

                    if (_takeBackWT.Sn == 0)
                        _tbRes = _takeBackWTRepository.InsertTakeBackWT(_takeBackWT) == 1;
                    else
                        _tbRes = _takeBackWTRepository.UpdateTakeBackWT(_takeBackWT) == 1;

                    if (!_tbRes)
                        throw new Exception("新增回收工時主檔異常");

                    if (_takeBackWT.Sn == 0)
                    {
                        _tbWTSn = _takeBackWTRepository.SelectByConditions(_takeBackWT.ProcessDate, new List<WTCategoryEnum> { _takeBackWT.WTCategoryId }).FirstOrDefault()?.Sn ?? 0;

                        if (_tbWTSn == 0)
                            throw new Exception("撈取回收工時主檔編號異常");
                    }
                    else
                        _tbWTSn = _takeBackWT.Sn;

                    _takeBackWTRepository.DeleteTakeBackWTInfo(_tbWTSn);

                    _takeBackWTDetail.ForEach(f => f.TakeBackWtSn = _tbWTSn);
                    _takeBackWTAttendance.ForEach(f => f.TakeBackWtSn = _tbWTSn);

                    _tbDetailRes = _takeBackWTRepository.InsertTakeBackWTDetail(_takeBackWTDetail) == _takeBackWTDetail.Count;
                    _tbAttenRes = _takeBackWTRepository.InsertTakeBackWTAttendance(_takeBackWTAttendance) == _takeBackWTAttendance.Count;

                    if (_tbDetailRes && _tbAttenRes)
                        scope.Complete();
                    else
                        _updResult = "新增異常";
                }

                return (_updResult, GetTBWTList(_proccessDate, (WTCategoryEnum)editEntity.WTCategoryId).First());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TakeBackWTKanBanEntity GetWTKanBan(DateTime processDate, int catgId)
        {
            try
            {
                var _wtCatgGrop = Convert.ToBoolean(catgId & (int)WTCategoryEnum.FrontA) && Convert.ToBoolean(catgId & (int)WTCategoryEnum.FrontB)
                        ? new List<WTCategoryEnum> { WTCategoryEnum.FrontA, WTCategoryEnum.FrontB }
                        : new List<WTCategoryEnum> { WTCategoryEnum.BackendA, WTCategoryEnum.BackendB };

                var _takeBackWTList = _takeBackWTRepository.SelectByConditions(processDate.Date, wtCatgIdList: _wtCatgGrop);

                var _takeBackWTMonthList = _takeBackWTRepository.SelectMonthlyData(processDate.Year, processDate.Month, _wtCatgGrop);

                var _takeBackAtten = _takeBackWTRepository.SelectAttendanceByConditions(_takeBackWTMonthList.Select(s => s.Sn).ToList());

                TakeBackWTKanBanEntity _response = new TakeBackWTKanBanEntity()
                {
                    Date = _takeBackWTList.FirstOrDefault()?.ProcessDate.ToString("yyyy-MM-dd") ?? processDate.ToString("yyyy-MM-dd"),
                    TotalTakeBack = _takeBackWTList?.Sum(s => s.BondTakeBack + s.FogTakeBack + s.LamTakeBack + s.AssyTakeBack + s.CDPTakeBack) ?? 0
                };

                _response.TakeBackPercent = _takeBackAtten?.Where(w => _takeBackWTList.Select(s => s.Sn).Contains(w.TakeBackWtSn)).Sum(s => s.TotalWorkTime) == 0
                    ? 0
                    : _response.TotalTakeBack / (_takeBackAtten?.Where(w => _takeBackWTList.Select(s => s.Sn).Contains(w.TakeBackWtSn)).Sum(s => s.TotalWorkTime) ?? 1) * 100;

                _response.DetailList = _takeBackWTMonthList.GroupBy(g => g.ProcessDate).Select(wt => 
                {
                    var _tmpAtten = _takeBackAtten.Where(at => wt.Select(s => s.Sn).Contains(at.TakeBackWtSn)).ToList();

                    return new TakeBackWTKanBanDetailEntity
                    {
                        Date = wt.Key.ToString("yyyy/MM/dd"),
                        ShortDateStr = wt.Key.ToString("MM/dd"),
                        TakeBackPercent = Math.Round(wt.Sum(s => s.BondTakeBack + s.FogTakeBack + s.LamTakeBack + s.AssyTakeBack + s.CDPTakeBack) / _tmpAtten.Sum(s => s.TotalWorkTime) * 100, 2)
                    };
                }).ToList();

                return _response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}