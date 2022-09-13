using MOD4.Web.DomainService.Entity;
using MOD4.Web.Repostory;
//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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

        public PerformanceDomainService(IDailyEquipmentRepository dailyEquipmentRepository,
            ITargetSettingDomainService targetSettingDomainService,
            IEquipmentDomainService equipmentDomainService,
            ILineTTRepository lineTTRepository)
        {
            _dailyEquipmentRepository = dailyEquipmentRepository;
            _targetSettingDomainService = targetSettingDomainService;
            _equipmentDomainService = equipmentDomainService;
            _lineTTRepository = lineTTRepository;
        }


        public List<PassQtyEntity> GetList(string _mfgDTE = "", string _shift = "", string _nodeAryStr = "")
        {
            DateTime _nowTime = DateTime.Now;
            _mfgDTE = string.IsNullOrEmpty(_mfgDTE) ? _nowTime.ToString("yyyy-MM-dd") : _mfgDTE;
            _shift = string.IsNullOrEmpty(_shift)
                    ? _nowTime.TimeOfDay >= _time0730 && _nowTime.TimeOfDay < _time1930 ? "A" : "B"
                    : _shift;
            var _nodeList = string.IsNullOrEmpty(_nodeAryStr)
                    ? new List<string> { "1415", "1420", "1460", "1600", "1700", "1720" }
                    : _nodeAryStr.Split(",").ToList();

            var _mfgDteEnd = DateTime.Parse(_mfgDTE).AddDays(1).ToString("yyyy-MM-dd");

            var _dailyEqpList = _dailyEquipmentRepository.SelectByConditions(_mfgDTE, _shift, _nodeList);

            var _allNodeList = _dailyEqpList.SelectMany(dailyEq => dailyEq.Equipments.Split(",")).ToList();

            var _lineTTList = _lineTTRepository.SelectByConditions(_mfgDTE, _allNodeList, _shift);

            var _settingList = _targetSettingDomainService.GetList(_nodeList);

            var _eqpHistoryList = _equipmentDomainService.GetEntityHistoryDetail(_mfgDTE, _settingList.Where(we => we.DownEquipment != "").Select(s => s.DownEquipment).ToList());

            List<PassQtyEntity> _response = new List<PassQtyEntity>();
            var url = "http://zipsum/modreport/Report/SHOPMOD/OperPerfDetail8.asp?";

            Parallel.ForEach(_dailyEqpList, (_dailyEqp) =>
            {
                List<PerformanceDetailEntity> _performanceDetail = new List<PerformanceDetailEntity>();

                foreach (var Equipment in _dailyEqp.Equipments.Split(","))
                {
                    string _qStr = $"Shop=MOD4&G_FAC=6&StrSql_w=+and+prod_nbr+in+%28%27GDD340IA0090S%27+%29++and+lcm_owner+in+%28%27RES0%27%29&StrSql_w4=&col0=&col1=&row0=GDD340IA0090S&" +
                        $"row2={Equipment}&row3={_shift}&row1={_dailyEqp.Node}&sql_m=+and+acct_date+%3E%3D+%27{_mfgDTE}%27+and+acct_date+%3C%3D+%27{_mfgDTE}%27&" +
                        $"sql_m1=+and+trans_date+%3E%3D+%27{_mfgDTE}+07%3A30%3A00.000000%27+and+trans_date+%3C%3D+%27{_mfgDteEnd}+07%3A30%3A00.000000%27&" +
                        $"sqlbu2=+and+shift_id%3D%27{_shift}%27&vdate_s=&vdate_e=";

                    var data = new StringContent(_qStr, Encoding.UTF8, "application/x-www-form-urlencodedn");

                    string[] array;

                    using (var client = new HttpClient())
                    {
                        //var response = client.PostAsync(url, data);
                        var response = client.PostAsync(url + _qStr, data).Result;
                        //var response = client.GetAsync(url);
                        response.Content.Headers.ContentType.CharSet = "Big5";

                        string result = response.Content.ReadAsStringAsync().Result;
                        //result = Encoding.GetEncoding("big5").GetString(Convert.FromBase64String(result));

                        result = result.Remove(0, 12200);

                        array = result.Split("<SCRIPT LANGUAGE=vbscript >");

                    }

                    //string text = File.ReadAllText("D:\\response.txt");

                    //array = text.Split("<SCRIPT LANGUAGE=vbscript >");

                    var cnt = (array.Count() - 3) / 14;

                    for (int i = 0; i < cnt; i++)
                    {
                        _performanceDetail.Add(Process(array.Skip((i * 14) + 1).Take(14).ToArray(), _dailyEqp.Node, _dailyEqp.NodeNo));
                    }
                }


                PassQtyEntity _tResponse = new PassQtyEntity();

                var _currentLineTT = _lineTTList.Where(w => _dailyEqpList.FirstOrDefault(daily => daily.Node == _dailyEqp.Node).Equipments.Split(",").Contains(w.line))
                        .Select(s => new LineTTEntity
                        {
                            sn = s.sn,
                            line = s.line,
                            MFG_Day = s.MFG_Day,
                            MFG_HR = s.MFG_HR,
                            Line_TT = s.Line_TT
                        }).ToList();

                var _lineSetting = _settingList.First(f => f.Node == _dailyEqp.Node);
                var _eqpHistory = _eqpHistoryList.Where(w => w.ToolId.Contains(_lineSetting.DownEquipment)).ToList();

                _tResponse.NodeNo = _dailyEqp.NodeNo;
                _tResponse.Node = _dailyEqp.Node;
                _tResponse.NodeName = _lineSetting.Node_Name;
                _tResponse.DetailList = new List<PassQtyDetailEntity>();

                if (_shift == "A")
                {
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time0730 && w.Time.TimeOfDay < _time0830)?.ToList(), "0730",
                            _lineSetting.Time0730, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time0730 && w.LmTime.TimeOfDay < _time0830).ToList(),
                            _currentLineTT.Where(tt => tt.MFG_HR == 0).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time0830 && w.Time.TimeOfDay < _time0930)?.ToList(), "0830",
                            _lineSetting.Time0830, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time0830 && w.LmTime.TimeOfDay < _time0930).ToList(),
                            _currentLineTT.Where(tt => tt.MFG_HR == 1).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time0930 && w.Time.TimeOfDay < _time1030)?.ToList(), "0930",
                            _lineSetting.Time0930, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time0930 && w.LmTime.TimeOfDay < _time1030).ToList(),
                            _currentLineTT.Where(tt => tt.MFG_HR == 2).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time1030 && w.Time.TimeOfDay < _time1130)?.ToList(), "1030",
                            _lineSetting.Time1030, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time1030 && w.LmTime.TimeOfDay < _time1130).ToList(),
                            _currentLineTT.Where(tt => tt.MFG_HR == 3).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time1130 && w.Time.TimeOfDay < _time1230)?.ToList(), "1130",
                            _lineSetting.Time1130, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time1130 && w.LmTime.TimeOfDay < _time1230).ToList(),
                            _currentLineTT.Where(tt => tt.MFG_HR == 4).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time1230 && w.Time.TimeOfDay < _time1330)?.ToList(), "1230",
                            _lineSetting.Time1230, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time1230 && w.LmTime.TimeOfDay < _time1330).ToList(),
                            _currentLineTT.Where(tt => tt.MFG_HR == 5).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time1330 && w.Time.TimeOfDay < _time1430)?.ToList(), "1330",
                            _lineSetting.Time1330, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time1330 && w.LmTime.TimeOfDay < _time1430).ToList(),
                            _currentLineTT.Where(tt => tt.MFG_HR == 6).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time1430 && w.Time.TimeOfDay < _time1530)?.ToList(), "1430",
                            _lineSetting.Time1430, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time1430 && w.LmTime.TimeOfDay < _time1530).ToList(),
                            _currentLineTT.Where(tt => tt.MFG_HR == 7).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time1530 && w.Time.TimeOfDay < _time1630)?.ToList(), "1530",
                            _lineSetting.Time1530, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time1530 && w.LmTime.TimeOfDay < _time1630).ToList(),
                            _currentLineTT.Where(tt => tt.MFG_HR == 8).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time1630 && w.Time.TimeOfDay < _time1730)?.ToList(), "1630",
                            _lineSetting.Time1630, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time1630 && w.LmTime.TimeOfDay < _time1730).ToList(),
                            _currentLineTT.Where(tt => tt.MFG_HR == 9).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time1730 && w.Time.TimeOfDay < _time1830)?.ToList(), "1730",
                            _lineSetting.Time1730, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time1730 && w.LmTime.TimeOfDay < _time1830).ToList(),
                            _currentLineTT.Where(tt => tt.MFG_HR == 10).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time1830 && w.Time.TimeOfDay < _time1930)?.ToList(), "1830",
                            _lineSetting.Time1830, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time1830 && w.LmTime.TimeOfDay < _time1930).ToList(),
                            _currentLineTT.Where(tt => tt.MFG_HR == 11).ToList()));
                }
                else
                {
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time1930 && w.Time.TimeOfDay < _time2030)?.ToList(), "1930",
                            _lineSetting.Time1930, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time1930 && w.LmTime.TimeOfDay < _time2030).ToList(),
                            _currentLineTT.Where(tt => tt.MFG_HR == 12).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time2030 && w.Time.TimeOfDay < _time2130)?.ToList(), "2030",
                            _lineSetting.Time2030, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time2030 && w.LmTime.TimeOfDay < _time2130).ToList(),
                            _currentLineTT.Where(tt => tt.MFG_HR == 13).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time2130 && w.Time.TimeOfDay < _time2230)?.ToList(), "2130",
                            _lineSetting.Time2130, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time2130 && w.LmTime.TimeOfDay < _time2230).ToList(),
                            _currentLineTT.Where(tt => tt.MFG_HR == 14).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time2230 && w.Time.TimeOfDay < _time2330)?.ToList(), "2230",
                            _lineSetting.Time2230, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time2230 && w.LmTime.TimeOfDay < _time2330).ToList(),
                            _currentLineTT.Where(tt => tt.MFG_HR == 15).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time2330 && w.Time.TimeOfDay < _time0030)?.ToList(), "2330",
                            _lineSetting.Time2330, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time2330 && w.LmTime.TimeOfDay < _time0030).ToList(),
                            _currentLineTT.Where(tt => tt.MFG_HR == 16).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time0030 && w.Time.TimeOfDay < _time0130)?.ToList(), "0030",
                            _lineSetting.Time0030, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time0030 && w.LmTime.TimeOfDay < _time0130).ToList(),
                            _currentLineTT.Where(tt => tt.MFG_HR == 17).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time0130 && w.Time.TimeOfDay < _time0230)?.ToList(), "0130",
                            _lineSetting.Time0130, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time0130 && w.LmTime.TimeOfDay < _time0230).ToList(),
                            _currentLineTT.Where(tt => tt.MFG_HR == 18).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time0230 && w.Time.TimeOfDay < _time0330)?.ToList(), "0230",
                            _lineSetting.Time0230, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time0230 && w.LmTime.TimeOfDay < _time0330).ToList(),
                            _currentLineTT.Where(tt => tt.MFG_HR == 19).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time0330 && w.Time.TimeOfDay < _time0430)?.ToList(), "0330",
                            _lineSetting.Time0330, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time0330 && w.LmTime.TimeOfDay < _time0430).ToList(),
                            _currentLineTT.Where(tt => tt.MFG_HR == 20).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time0430 && w.Time.TimeOfDay < _time0530)?.ToList(), "0430",
                            _lineSetting.Time0430, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time0430 && w.LmTime.TimeOfDay < _time0530).ToList(),
                            _currentLineTT.Where(tt => tt.MFG_HR == 21).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time0530 && w.Time.TimeOfDay < _time0630)?.ToList(), "0530",
                            _lineSetting.Time0530, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time0530 && w.LmTime.TimeOfDay < _time0630).ToList(),
                            _currentLineTT.Where(tt => tt.MFG_HR == 22).ToList()));
                    _tResponse.DetailList.Add(GroupDetail(_performanceDetail.Where(w => w.Time.TimeOfDay > _time0630 && w.Time.TimeOfDay < _time0730)?.ToList(), "0630",
                            _lineSetting.Time0630, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time0630 && w.LmTime.TimeOfDay < _time0730).ToList(),
                            _currentLineTT.Where(tt => tt.MFG_HR == 23).ToList()));
                }

                _response.Add(_tResponse);

                //_response.AddRange(_performanceDetail.GroupBy(gb => new { gb.Node, gb.NodeNo }).Select(s =>
                //{
                //    var _temp = new PassQtyEntity();

                //    var _currentLineTT = _lineTTList.Where(w =>
                //        _dailyEqpList.FirstOrDefault(daily => daily.Node == s.Key.Node).Equipments.Split(",").Contains(w.line))
                //        .Select(s => new LineTTEntity
                //        {
                //            sn = s.sn,
                //            line = s.line,
                //            MFG_Day = s.MFG_Day,
                //            MFG_HR = s.MFG_HR,
                //            Line_TT = s.Line_TT
                //        }).ToList();

                //    var _lineSetting = _settingList.First(f => f.Node == s.Key.Node);
                //    var _eqpHistory = _eqpHistoryList.Where(w => w.ToolId.Contains(_lineSetting.DownEquipment)).ToList();
                //    //_downEqDic.ContainsKey(s.Key) ? _eqpHistoryList.Where(w => _downEqDic[s.Key].Contains(w.ToolId)).ToList() : new List<EquipmentEntity>();

                //    _temp.NodeNo = s.Key.NodeNo;
                //    _temp.Node = s.Key.Node;
                //    _temp.NodeName = _lineSetting.Node_Name;
                //    _temp.DetailList = new List<PassQtyDetailEntity>();

                //    if (_shift == "A")
                //    {
                //        _temp.DetailList.Add(GroupDetail(s.Where(w => w.Time.TimeOfDay > _time0730 && w.Time.TimeOfDay < _time0830)?.ToList(), "0730",
                //                _lineSetting.Time0730, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time0730 && w.LmTime.TimeOfDay < _time0830).ToList(),
                //                _currentLineTT.Where(tt => tt.MFG_HR == 0).ToList()));
                //        _temp.DetailList.Add(GroupDetail(s.Where(w => w.Time.TimeOfDay > _time0830 && w.Time.TimeOfDay < _time0930)?.ToList(), "0830",
                //                _lineSetting.Time0830, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time0830 && w.LmTime.TimeOfDay < _time0930).ToList(),
                //                _currentLineTT.Where(tt => tt.MFG_HR == 1).ToList()));
                //        _temp.DetailList.Add(GroupDetail(s.Where(w => w.Time.TimeOfDay > _time0930 && w.Time.TimeOfDay < _time1030)?.ToList(), "0930",
                //                _lineSetting.Time0930, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time0930 && w.LmTime.TimeOfDay < _time1030).ToList(),
                //                _currentLineTT.Where(tt => tt.MFG_HR == 2).ToList()));
                //        _temp.DetailList.Add(GroupDetail(s.Where(w => w.Time.TimeOfDay > _time1030 && w.Time.TimeOfDay < _time1130)?.ToList(), "1030",
                //                _lineSetting.Time1030, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time1030 && w.LmTime.TimeOfDay < _time1130).ToList(),
                //                _currentLineTT.Where(tt => tt.MFG_HR == 3).ToList()));
                //        _temp.DetailList.Add(GroupDetail(s.Where(w => w.Time.TimeOfDay > _time1130 && w.Time.TimeOfDay < _time1230)?.ToList(), "1130",
                //                _lineSetting.Time1130, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time1130 && w.LmTime.TimeOfDay < _time1230).ToList(),
                //                _currentLineTT.Where(tt => tt.MFG_HR == 4).ToList()));
                //        _temp.DetailList.Add(GroupDetail(s.Where(w => w.Time.TimeOfDay > _time1230 && w.Time.TimeOfDay < _time1330)?.ToList(), "1230",
                //                _lineSetting.Time1230, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time1230 && w.LmTime.TimeOfDay < _time1330).ToList(),
                //                _currentLineTT.Where(tt => tt.MFG_HR == 5).ToList()));
                //        _temp.DetailList.Add(GroupDetail(s.Where(w => w.Time.TimeOfDay > _time1330 && w.Time.TimeOfDay < _time1430)?.ToList(), "1330",
                //                _lineSetting.Time1330, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time1330 && w.LmTime.TimeOfDay < _time1430).ToList(),
                //                _currentLineTT.Where(tt => tt.MFG_HR == 6).ToList()));
                //        _temp.DetailList.Add(GroupDetail(s.Where(w => w.Time.TimeOfDay > _time1430 && w.Time.TimeOfDay < _time1530)?.ToList(), "1430",
                //                _lineSetting.Time1430, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time1430 && w.LmTime.TimeOfDay < _time1530).ToList(),
                //                _currentLineTT.Where(tt => tt.MFG_HR == 7).ToList()));
                //        _temp.DetailList.Add(GroupDetail(s.Where(w => w.Time.TimeOfDay > _time1530 && w.Time.TimeOfDay < _time1630)?.ToList(), "1530",
                //                _lineSetting.Time1530, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time1530 && w.LmTime.TimeOfDay < _time1630).ToList(),
                //                _currentLineTT.Where(tt => tt.MFG_HR == 8).ToList()));
                //        _temp.DetailList.Add(GroupDetail(s.Where(w => w.Time.TimeOfDay > _time1630 && w.Time.TimeOfDay < _time1730)?.ToList(), "1630",
                //                _lineSetting.Time1630, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time1630 && w.LmTime.TimeOfDay < _time1730).ToList(),
                //                _currentLineTT.Where(tt => tt.MFG_HR == 9).ToList()));
                //        _temp.DetailList.Add(GroupDetail(s.Where(w => w.Time.TimeOfDay > _time1730 && w.Time.TimeOfDay < _time1830)?.ToList(), "1730",
                //                _lineSetting.Time1730, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time1730 && w.LmTime.TimeOfDay < _time1830).ToList(),
                //                _currentLineTT.Where(tt => tt.MFG_HR == 10).ToList()));
                //        _temp.DetailList.Add(GroupDetail(s.Where(w => w.Time.TimeOfDay > _time1830 && w.Time.TimeOfDay < _time1930)?.ToList(), "1830",
                //                _lineSetting.Time1830, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time1830 && w.LmTime.TimeOfDay < _time1930).ToList(),
                //                _currentLineTT.Where(tt => tt.MFG_HR == 11).ToList()));
                //    }
                //    else
                //    {
                //        _temp.DetailList.Add(GroupDetail(s.Where(w => w.Time.TimeOfDay > _time1930 && w.Time.TimeOfDay < _time2030)?.ToList(), "1930",
                //                _lineSetting.Time1930, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time1930 && w.LmTime.TimeOfDay < _time2030).ToList(),
                //                _currentLineTT.Where(tt => tt.MFG_HR == 12).ToList()));
                //        _temp.DetailList.Add(GroupDetail(s.Where(w => w.Time.TimeOfDay > _time2030 && w.Time.TimeOfDay < _time2130)?.ToList(), "2030",
                //                _lineSetting.Time2030, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time2030 && w.LmTime.TimeOfDay < _time2130).ToList(),
                //                _currentLineTT.Where(tt => tt.MFG_HR == 13).ToList()));
                //        _temp.DetailList.Add(GroupDetail(s.Where(w => w.Time.TimeOfDay > _time2130 && w.Time.TimeOfDay < _time2230)?.ToList(), "2130",
                //                _lineSetting.Time2130, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time2130 && w.LmTime.TimeOfDay < _time2230).ToList(),
                //                _currentLineTT.Where(tt => tt.MFG_HR == 14).ToList()));
                //        _temp.DetailList.Add(GroupDetail(s.Where(w => w.Time.TimeOfDay > _time2230 && w.Time.TimeOfDay < _time2330)?.ToList(), "2230",
                //                _lineSetting.Time2230, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time2230 && w.LmTime.TimeOfDay < _time2330).ToList(),
                //                _currentLineTT.Where(tt => tt.MFG_HR == 15).ToList()));
                //        _temp.DetailList.Add(GroupDetail(s.Where(w => w.Time.TimeOfDay > _time2330 && w.Time.TimeOfDay < _time0030)?.ToList(), "2330",
                //                _lineSetting.Time2330, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time2330 && w.LmTime.TimeOfDay < _time0030).ToList(),
                //                _currentLineTT.Where(tt => tt.MFG_HR == 16).ToList()));
                //        _temp.DetailList.Add(GroupDetail(s.Where(w => w.Time.TimeOfDay > _time0030 && w.Time.TimeOfDay < _time0130)?.ToList(), "0030",
                //                _lineSetting.Time0030, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time0030 && w.LmTime.TimeOfDay < _time0130).ToList(),
                //                _currentLineTT.Where(tt => tt.MFG_HR == 17).ToList()));
                //        _temp.DetailList.Add(GroupDetail(s.Where(w => w.Time.TimeOfDay > _time0130 && w.Time.TimeOfDay < _time0230)?.ToList(), "0130",
                //                _lineSetting.Time0130, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time0130 && w.LmTime.TimeOfDay < _time0230).ToList(),
                //                _currentLineTT.Where(tt => tt.MFG_HR == 18).ToList()));
                //        _temp.DetailList.Add(GroupDetail(s.Where(w => w.Time.TimeOfDay > _time0230 && w.Time.TimeOfDay < _time0330)?.ToList(), "0230",
                //                _lineSetting.Time0230, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time0230 && w.LmTime.TimeOfDay < _time0330).ToList(),
                //                _currentLineTT.Where(tt => tt.MFG_HR == 19).ToList()));
                //        _temp.DetailList.Add(GroupDetail(s.Where(w => w.Time.TimeOfDay > _time0330 && w.Time.TimeOfDay < _time0430)?.ToList(), "0330",
                //                _lineSetting.Time0330, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time0330 && w.LmTime.TimeOfDay < _time0430).ToList(),
                //                _currentLineTT.Where(tt => tt.MFG_HR == 20).ToList()));
                //        _temp.DetailList.Add(GroupDetail(s.Where(w => w.Time.TimeOfDay > _time0430 && w.Time.TimeOfDay < _time0530)?.ToList(), "0430",
                //                _lineSetting.Time0430, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time0430 && w.LmTime.TimeOfDay < _time0530).ToList(),
                //                _currentLineTT.Where(tt => tt.MFG_HR == 21).ToList()));
                //        _temp.DetailList.Add(GroupDetail(s.Where(w => w.Time.TimeOfDay > _time0530 && w.Time.TimeOfDay < _time0630)?.ToList(), "0530",
                //                _lineSetting.Time0530, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time0530 && w.LmTime.TimeOfDay < _time0630).ToList(),
                //                _currentLineTT.Where(tt => tt.MFG_HR == 22).ToList()));
                //        _temp.DetailList.Add(GroupDetail(s.Where(w => w.Time.TimeOfDay > _time0630 && w.Time.TimeOfDay < _time0730)?.ToList(), "0630",
                //                _lineSetting.Time0630, _eqpHistory.Where(w => w.LmTime.TimeOfDay > _time0630 && w.LmTime.TimeOfDay < _time0730).ToList(),
                //                _currentLineTT.Where(tt => tt.MFG_HR == 23).ToList()));
                //    }

                //    return _temp;
                //}));

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
            List<EquipmentEntity> eqpEntities,
            List<LineTTEntity> lineTTList)
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

            eqpEntities = eqpEntities.OrderByDescending(od => Convert.ToInt16(od.RepairTime)).ToList();

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
            temp.LineTTTOP1 = lineTTList != null && lineTTList.Any()
                ? lineTTList.GroupBy(b => b.Line_TT).OrderBy(ob => ob.Key).Take(1).FirstOrDefault()?.Key.ToString() ?? ""
                : "";
            temp.LineTTTOP2 = lineTTList != null && lineTTList.Any()
                ? lineTTList.GroupBy(b => b.Line_TT).OrderBy(ob => ob.Key).Skip(1).Take(1).FirstOrDefault()?.Key.ToString() ?? ""
                : "";

            return temp;
        }
    }
}