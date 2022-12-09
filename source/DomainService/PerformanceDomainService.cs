using MOD4.Web.DomainService.Entity;
using MOD4.Web.Repostory;
//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace MOD4.Web.DomainService
{
    public class PerformanceDomainService : IPerformanceDomainService
    {
        private readonly IDailyEquipmentRepository _dailyEquipmentRepository;
        private readonly ITargetSettingDomainService _targetSettingDomainService;
        private readonly IEquipmentDomainService _equipmentDomainService;
        private readonly ILineTTRepository _lineTTRepository;
        private readonly IOptionDomainService _optionDomainService;
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
            {"1700",("D KEN",10) }
        };
        #region time block
        private TimeSpan _time0730 = new TimeSpan(7, 30, 1);
        private TimeSpan _time0830 = new TimeSpan(8, 30, 1);
        private TimeSpan _time0930 = new TimeSpan(09, 30, 1);
        private TimeSpan _time1030 = new TimeSpan(10, 30, 1);
        private TimeSpan _time1130 = new TimeSpan(11, 30, 1);
        private TimeSpan _time1230 = new TimeSpan(12, 30, 1);
        private TimeSpan _time1330 = new TimeSpan(13, 30, 1);
        private TimeSpan _time1430 = new TimeSpan(14, 30, 1);
        private TimeSpan _time1530 = new TimeSpan(15, 30, 1);
        private TimeSpan _time1630 = new TimeSpan(16, 30, 1);
        private TimeSpan _time1730 = new TimeSpan(17, 30, 1);
        private TimeSpan _time1830 = new TimeSpan(18, 30, 1);
        private TimeSpan _time1930 = new TimeSpan(19, 30, 1);
        private TimeSpan _time2030 = new TimeSpan(20, 30, 1);
        private TimeSpan _time2130 = new TimeSpan(21, 30, 1);
        private TimeSpan _time2230 = new TimeSpan(22, 30, 1);
        private TimeSpan _time2330 = new TimeSpan(23, 30, 1);
        private TimeSpan _time0030 = new TimeSpan(00, 30, 1);
        private TimeSpan _time0130 = new TimeSpan(01, 30, 1);
        private TimeSpan _time0230 = new TimeSpan(02, 30, 1);
        private TimeSpan _time0330 = new TimeSpan(03, 30, 1);
        private TimeSpan _time0430 = new TimeSpan(04, 30, 1);
        private TimeSpan _time0530 = new TimeSpan(05, 30, 1);
        private TimeSpan _time0630 = new TimeSpan(06, 30, 1);
        #endregion

        public PerformanceDomainService(IDailyEquipmentRepository dailyEquipmentRepository,
            ITargetSettingDomainService targetSettingDomainService,
            IEquipmentDomainService equipmentDomainService,
            ILineTTRepository lineTTRepository,
            IOptionDomainService optionDomainService)
        {
            _dailyEquipmentRepository = dailyEquipmentRepository;
            _targetSettingDomainService = targetSettingDomainService;
            _equipmentDomainService = equipmentDomainService;
            _lineTTRepository = lineTTRepository;
            _optionDomainService = optionDomainService;
        }


        public List<PassQtyEntity> GetList(string mfgDTE = "", string prodList = "1206", string shift = "", string nodeAryStr = "")
        {

            DateTime _nowTime = DateTime.Now;
            Dictionary<string, (string, int)> _nodeDic = new Dictionary<string, (string, int)>();
            mfgDTE = string.IsNullOrEmpty(mfgDTE) ? _nowTime.ToString("yyyy-MM-dd") : mfgDTE;
            var _mfgDteEnd = DateTime.Parse(mfgDTE).AddDays(1).ToString("yyyy-MM-dd");
            shift = string.IsNullOrEmpty(shift)
                    ? _nowTime.TimeOfDay >= _time0730 && _nowTime.TimeOfDay < _time1930 ? "A" : "B"
                    : shift;

            if (string.IsNullOrEmpty(nodeAryStr))
                _nodeDic = _allNodeDictionary;
            else
                _allNodeDictionary.Where(w => nodeAryStr.Contains(w.Key)).ToList().ForEach(f => _nodeDic.Add(f.Key, f.Value));

            List<int> _prodOptionList = (prodList ?? "1206").Split(",").Select(s => Convert.ToInt32(s)).ToList();

            List<(int, string)> _allLcmProdOptions = _optionDomainService.GetLcmProdOptions().SelectMany(sm => sm.Item2).ToList();

            _allLcmProdOptions = _allLcmProdOptions.Where(option => _prodOptionList.Contains(option.Item1)).ToList();


            //var _dailyEqpList = _dailyEquipmentRepository.SelectByConditions(mfgDTE, shift, _nodeList);

            //var _allNodeList = _dailyEqpList.SelectMany(dailyEq => dailyEq.Equipments.Split(",")).ToList();

            //var _lineTTList = _lineTTRepository.SelectByConditions(mfgDTE, _allNodeList, shift);

            var _targetSettingList = _targetSettingDomainService.GetList(prodSn: _prodOptionList, nodeList: _nodeDic.Select(s => s.Key).ToList());

            var _eqpHistoryList =
                _equipmentDomainService.GetEntityHistoryDetail(mfgDTE, _targetSettingList.Where(we => we.DownEquipment != "").Select(s => s.DownEquipment).ToList(), _prodOptionList);

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
                    _prodEqDic.Add(prod, GetNodeAllEquiomentNo($"{node.Key}-{node.Value.Item1}", prod, shift, mfgDTE, _mfgDteEnd));
                }

                //foreach (var prodEq in _prodEqDic)
                //{

                Parallel.ForEach(_prodEqDic, (prodEq) =>
                {

                    foreach (var eq in prodEq.Value)
                    {
                        string _qStr = $"Shop=MOD4&G_FAC=6&StrSql_w=+and+prod_nbr+in+('{prodEq.Key}')+and+lcm_owner+in+('QTAP','LCME','PRDG','PROD','RES0')&StrSql_w4=&col0=&col1=&row0={prodEq.Key}" +
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
                            _performanceDetail.Add(Process(array.Skip((i * 14) + 1).Take(14).ToArray(), node.Key, node.Value.Item2));
                        }
                    }
                });
                //}

                //foreach (var prodEq in _prodEqDic)
                //{
                //    foreach (var eq in prodEq.Value)
                //    {
                //        string _qStr = $"Shop=MOD4&G_FAC=6&StrSql_w=+and+prod_nbr+in+('{prodEq.Key}')+and+lcm_owner+in+('QTAP','LCME','PRDG','PROD','RES0')&StrSql_w4=&col0=&col1=&row0={prodEq.Key}" +
                //                    $"&row2={eq}&row3={shift}&row1={node.Key}&sql_m=+and+acct_date+>=+'{mfgDTE}'+and+acct_date+<=+'{mfgDTE}'" +
                //                    $"&sql_m1=+and+trans_date+>=+'{mfgDTE}+07:30:00.000000'+and+trans_date+<=+'{_mfgDteEnd}+07:30:00.000000'" +
                //                    $"&sqlbu2=+and+shift_id='{shift}'&vdate_s=&vdate_e=";

                //        var data = new StringContent(_qStr, Encoding.UTF8, "application/x-www-form-urlencodedn");

                //        string[] array;

                //        using (var client = new HttpClient())
                //        {
                //            //var response = client.PostAsync(url, data);
                //            var response = client.PostAsync(url + _qStr, data).Result;
                //            //var response = client.GetAsync(url);
                //            response.Content.Headers.ContentType.CharSet = "Big5";

                //            string result = response.Content.ReadAsStringAsync().Result;

                //            result = result.Remove(0, 12200);

                //            array = result.Split("<SCRIPT LANGUAGE=vbscript >");

                //        }

                //        //string text = File.ReadAllText("D:\\response.txt");

                //        //array = text.Split("<SCRIPT LANGUAGE=vbscript >");

                //        var cnt = (array.Count() - 3) / 14;

                //        for (int i = 0; i < cnt; i++)
                //        {
                //            _performanceDetail.Add(Process(array.Skip((i * 14) + 1).Take(14).ToArray(), node.Key, node.Value.Item2));
                //        }
                //    }
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
                    Node_Name = string.Join("/", _currentProdTarget.Select(s => s.Node_Name)),
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
                    Time0630 = _currentProdTarget.Sum(s => s.Time0630)
                };

                var _eqpHistory = _eqpHistoryList.Where(w => _lineSetting.DownEquipment.Contains(w.ToolId)).ToList();

                _tResponse.NodeNo = node.Value.Item2;
                _tResponse.Node = node.Key;
                _tResponse.NodeName = _lineSetting.Node_Name;
                _tResponse.DetailList = new List<PassQtyDetailEntity>();

                if (shift == "A")
                {
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time0730 && w.Time.TimeOfDay < _time0830)?.ToList(), "0730",
                                    _lineSetting.Time0730, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time0730 && w.LmTime.TimeOfDay < _time0830).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time0830 && w.Time.TimeOfDay < _time0930)?.ToList(), "0830",
                                    _lineSetting.Time0830, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time0830 && w.LmTime.TimeOfDay < _time0930).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time0930 && w.Time.TimeOfDay < _time1030)?.ToList(), "0930",
                                    _lineSetting.Time0930, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time0930 && w.LmTime.TimeOfDay < _time1030).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time1030 && w.Time.TimeOfDay < _time1130)?.ToList(), "1030",
                                    _lineSetting.Time1030, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time1030 && w.LmTime.TimeOfDay < _time1130).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time1130 && w.Time.TimeOfDay < _time1230)?.ToList(), "1130",
                                    _lineSetting.Time1130, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time1130 && w.LmTime.TimeOfDay < _time1230).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time1230 && w.Time.TimeOfDay < _time1330)?.ToList(), "1230",
                                    _lineSetting.Time1230, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time1230 && w.LmTime.TimeOfDay < _time1330).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time1330 && w.Time.TimeOfDay < _time1430)?.ToList(), "1330",
                                    _lineSetting.Time1330, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time1330 && w.LmTime.TimeOfDay < _time1430).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time1430 && w.Time.TimeOfDay < _time1530)?.ToList(), "1430",
                                    _lineSetting.Time1430, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time1430 && w.LmTime.TimeOfDay < _time1530).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time1530 && w.Time.TimeOfDay < _time1630)?.ToList(), "1530",
                                    _lineSetting.Time1530, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time1530 && w.LmTime.TimeOfDay < _time1630).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time1630 && w.Time.TimeOfDay < _time1730)?.ToList(), "1630",
                                    _lineSetting.Time1630, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time1630 && w.LmTime.TimeOfDay < _time1730).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time1730 && w.Time.TimeOfDay < _time1830)?.ToList(), "1730",
                                    _lineSetting.Time1730, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time1730 && w.LmTime.TimeOfDay < _time1830).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time1830 && w.Time.TimeOfDay < _time1930)?.ToList(), "1830",
                                    _lineSetting.Time1830, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time1830 && w.LmTime.TimeOfDay < _time1930).ToList()));
                }
                else
                {
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time1930 && w.Time.TimeOfDay < _time2030)?.ToList(), "1930",
                                    _lineSetting.Time1930, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time1930 && w.LmTime.TimeOfDay < _time2030).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time2030 && w.Time.TimeOfDay < _time2130)?.ToList(), "2030",
                                    _lineSetting.Time2030, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time2030 && w.LmTime.TimeOfDay < _time2130).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time2130 && w.Time.TimeOfDay < _time2230)?.ToList(), "2130",
                                    _lineSetting.Time2130, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time2130 && w.LmTime.TimeOfDay < _time2230).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time2230 && w.Time.TimeOfDay < _time2330)?.ToList(), "2230",
                                    _lineSetting.Time2230, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time2230 && w.LmTime.TimeOfDay < _time2330).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time2330 && w.Time.TimeOfDay < _time0030)?.ToList(), "2330",
                                    _lineSetting.Time2330, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time2330 && w.LmTime.TimeOfDay < _time0030).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time0030 && w.Time.TimeOfDay < _time0130)?.ToList(), "0030",
                                    _lineSetting.Time0030, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time0030 && w.LmTime.TimeOfDay < _time0130).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time0130 && w.Time.TimeOfDay < _time0230)?.ToList(), "0130",
                                    _lineSetting.Time0130, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time0130 && w.LmTime.TimeOfDay < _time0230).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time0230 && w.Time.TimeOfDay < _time0330)?.ToList(), "0230",
                                    _lineSetting.Time0230, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time0230 && w.LmTime.TimeOfDay < _time0330).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time0330 && w.Time.TimeOfDay < _time0430)?.ToList(), "0330",
                                    _lineSetting.Time0330, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time0330 && w.LmTime.TimeOfDay < _time0430).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time0430 && w.Time.TimeOfDay < _time0530)?.ToList(), "0430",
                                    _lineSetting.Time0430, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time0430 && w.LmTime.TimeOfDay < _time0530).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time0530 && w.Time.TimeOfDay < _time0630)?.ToList(), "0530",
                                    _lineSetting.Time0530, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time0530 && w.LmTime.TimeOfDay < _time0630).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time0630 && w.Time.TimeOfDay < _time0730)?.ToList(), "0630",
                                    _lineSetting.Time0630, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time0630 && w.LmTime.TimeOfDay < _time0730).ToList()));
                }

                _response.Add(_tResponse);
                //};
            });

            return _response.OrderBy(ob => ob.NodeNo).ToList();
        }

        private PerformanceDetailEntity Process(string[] detailStr, string nodeStr, int nodeNo)
        {
            PerformanceDetailEntity _entity = new PerformanceDetailEntity();

            string[] _tempArray;
            string _tempStr;

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
            _entity.Node = nodeStr;
            _entity.NodeNo = nodeNo;

            return _entity;
        }


        private PassQtyDetailEntity GroupDetail(
            List<PerformanceDetailEntity> entityList,
            string _time,
            int targetQty,
            List<EquipmentEntity> eqpEntities)
        //,List<LineTTEntity> lineTTList)
        {
            if (entityList == null || !entityList.Any())
                return new PassQtyDetailEntity
                {
                    Time = _time,
                    Target = targetQty,
                    InQty = 0,
                    NGQty = 0,
                    OKQty = 0,
                    Yield = "0.00%",
                    DefectRate = "0.00%",
                    Diff = targetQty * -1
                };

            eqpEntities = eqpEntities.OrderByDescending(od => Convert.ToDouble(od.RepairTime)).ToList();

            var _top1 = eqpEntities.Take(1).FirstOrDefault();
            var _top2 = eqpEntities.Skip(1).Take(1).FirstOrDefault();
            var _top3 = eqpEntities.Skip(2).Take(1).FirstOrDefault();

            var temp = new PassQtyDetailEntity
            {
                Time = _time,
                Target = targetQty,
                InQty = entityList.Count,
                NGQty = entityList.Sum(sum => sum.NG),
                OKQty = entityList.Count - entityList.Sum(sum => sum.NG)
            };

            temp.Yield = temp.InQty == 0 ? "0.00%" : (((float)temp.OKQty / (float)temp.InQty) * 100).ToString("0.00") + "%";
            temp.DefectRate = temp.InQty == 0 ? "0.00%" : (((float)temp.NGQty / (float)temp.InQty) * 100).ToString("0.00") + "%";
            temp.Diff = temp.InQty - temp.Target;
            temp.eqpInfoTOP1 = _top1 == null ? "" : $"{_top1.StatusCdsc} {_top1.Comment} ({_top1.RepairTime} min.)";
            temp.eqpInfoTOP2 = _top2 == null ? "" : $"{_top1.StatusCdsc} {_top2.Comment} ({_top2.RepairTime} min.)";
            temp.eqpInfoTOP3 = _top3 == null ? "" : $"{_top1.StatusCdsc} {_top3.Comment} ({_top3.RepairTime} min.)";
            //temp.LineTTTOP1 = lineTTList != null && lineTTList.Any()
            //    ? lineTTList.GroupBy(b => b.Line_TT).OrderBy(ob => ob.Key).Take(1).FirstOrDefault()?.Key.ToString() ?? ""
            //    : "";
            //temp.LineTTTOP2 = lineTTList != null && lineTTList.Any()
            //    ? lineTTList.GroupBy(b => b.Line_TT).OrderBy(ob => ob.Key).Skip(1).Take(1).FirstOrDefault()?.Key.ToString() ?? ""
            //    : "";

            return temp;
        }


        private List<string> GetNodeAllEquiomentNo(string node, string prodList, string shift, string startDTE, string endDTE)
        {
            List<string> _nodeEqList = new List<string>();

            string _qStr = $"Shop=MOD4&G_FAC=6&StrSql_w2={(string.IsNullOrEmpty(prodList) ? "" : $"+And+Prod_Nbr+in+('{prodList}')")}+and+lcm_owner+in+('QTAP','LCME','PRDG','PROD','RES0')+and++work_ctr+>+1000++" +
                $"&StrSql_w1={(string.IsNullOrEmpty(prodList) ? "" : $"+And+Prod_Nbr+in+('{prodList}')")}+and+lcm_owner+in+('QTAP','LCME','PRDG','PROD','RES0')+and++to_wc+>+1000+++and+shift_id='A'" +
                $"&StrSql_w={(string.IsNullOrEmpty(prodList) ? "" : $"+And+Prod_Nbr+in+('{prodList}')")}+and+lcm_owner+in+('QTAP','LCME','PRDG','PROD','RES0')&StrSql_tvset=&col0=Pass+Q%27ty&col1=&row0=" +
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
    }
}