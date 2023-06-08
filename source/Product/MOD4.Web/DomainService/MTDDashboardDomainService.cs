using Microsoft.AspNetCore.Http;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Repostory;
using MOD4.Web.Repostory.Dao;
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

namespace MOD4.Web.DomainService
{
    public class MTDDashboardDomainService : IMTDDashboardDomainService
    {
        private readonly IMTDProductionScheduleRepository _mtdProductionScheduleRepository;
        private readonly IUploadDomainService _uploadDomainService;
        private readonly IAlarmXmlRepository _alarmXmlRepository;
        private readonly IEqpInfoRepository _eqpInfoRepository;
        private readonly IEquipMappingRepository _equipMappingRepository;
        private readonly ITargetSettingDomainService _targetSettingDomainService;
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
            {1, "'QTAP','LCME','PRDG','PROD','RES0'"},
            {2, "'INT0'"}
        };
        private readonly string _zipsum106Url = "http://zipsum/modreport/Report/SHOPMOD/OperPerfDataSet.asp?";
        private readonly string _zipsum108Url = "http://zipsum/modreport/Report/SHOPMOD/EquUtilizationDataSet.asp";
        private readonly string _zipsum109Url = "http://zipsum/modreport/Report/SHOPMOD/EntityStaSumDataSet.asp";

        public MTDDashboardDomainService(IMTDProductionScheduleRepository mtdProductionScheduleRepository,
            IUploadDomainService uploadDomainService,
            IAlarmXmlRepository alarmXmlRepository,
            IEqpInfoRepository eqpInfoRepository,
            IEquipMappingRepository equipMappingRepository,
            ITargetSettingDomainService targetSettingDomainService)
        {
            _mtdProductionScheduleRepository = mtdProductionScheduleRepository;
            _uploadDomainService = uploadDomainService;
            _alarmXmlRepository = alarmXmlRepository;
            _eqpInfoRepository = eqpInfoRepository;
            _equipMappingRepository = equipMappingRepository;
            _targetSettingDomainService = targetSettingDomainService;
        }

        /// <summary>
        /// MTD 儀表板查詢
        /// </summary>
        /// <param name="floor">default 2F</param>
        /// <param name="date">default 昨日</param>
        /// <param name="time">default 24h</param>
        /// <returns></returns>
        public List<MTDDashboardMainEntity> DashboardSearch(int floor = 2, string date = "", decimal time = 24, int owner = 1)
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
            Dictionary<string, decimal> _processDownDic = new Dictionary<string, decimal>();
            Dictionary<string, string> _process109Dic = new Dictionary<string, string>();

            // 取得MTD排程所有機種
            List<MTDProductionScheduleDao> _mtdScheduleDataList = _mtdProductionScheduleRepository.SelectByConditions(floor, owner, _srchDate, _srchDate).ToList();

            var _mtdProdProcessSettings = _targetSettingDomainService.GetSettingForMTD(_mtdScheduleDataList.Select(s => s.ProductName).Distinct().ToList());

            _mtdProdProcessSettings = (from setting in _mtdProdProcessSettings
                                       join schedule in _mtdScheduleDataList
                                       on new { setting.Process, setting.ProdNo } equals new { schedule.Process, ProdNo = schedule.ProductName }
                                       select setting).ToList();

            // gorup MTD排程所有機種 for report 查詢
            List<MTDProductionScheduleDao> _mtdScheduleGroupByProd = _mtdScheduleDataList.GroupBy(gb => gb.ProductName).Select(s => new MTDProductionScheduleDao
            {
                ProductName = s.Key,
                Node = string.Join(",", s.Select(node => node.Node))
            }).ToList();

            // 取得MTD排程所有機種月計畫
            List<MTDProductionScheduleDao> _mtdMonthPlanList = _mtdProductionScheduleRepository.SelectMonthPlanQty(_nowTime.Year.ToString(), _nowTime.Month.ToString(), floor, owner);

            Task _106Task = new Task(() =>
            {
                // call zipsum 1.06 report 查詢 & 解析
                Parallel.ForEach(_mtdProdProcessSettings.GroupBy(gb => gb.ProdNo).Select(s => new MTDProductionScheduleDao
                {
                    ProductName = s.Key,
                    Node = string.Join(",", s.Select(node => node.Node))
                }), (prod) =>
                {
                    var _temp106Today = GetZipsum106Today(_srchDate.ToString("yyyy-MM-dd"), _srchDate.ToString("yyyy-MM-dd"), prod.ProductName, prod.Node);
                    var _temp106Month = GetZipsum106Month($"{_srchDate:yyyy-MM}-01", _srchDate.ToString("yyyy-MM-dd"), prod.ProductName, prod.Node);
                    lock (this)
                    {
                        _mtdPerformanceDay.AddRange(_temp106Today);
                        _mtdPerformanceMonth.AddRange(_temp106Month);
                    }
                });

                //_mtdProdProcessSettings.GroupBy(gb => gb.ProdNo).Select(s => new MTDProductionScheduleDao
                //{
                //    ProductName = s.Key,
                //    Node = string.Join(",", s.Select(node => node.Node))
                //}).ToList().ForEach(prod =>
                //{
                //    _mtdPerformanceDay.AddRange(GetZipsum106Today(_srchDate.ToString("yyyy-MM-dd"), _srchDate.ToString("yyyy-MM-dd"), prod.ProductName, prod.Node));
                //    _mtdPerformanceMonth.AddRange(GetZipsum106Month($"{_srchDate:yyyy-MM}-01", _srchDate.ToString("yyyy-MM-dd"), prod.ProductName, prod.Node));
                //});
            });

            Task _108109Task = new Task(() =>
            {
                // call zipsum 1.08 1.09 report 查詢 & 解析
                Parallel.ForEach(_mtdProdProcessSettings.GroupBy(gb => new { gb.Node, gb.DownEq }), (node) =>
                {
                    var _temp108List = GetZipsum108Today(_srchDate.ToString("yyyy-MM-dd"), node.Key.DownEq, node.Key.Node.ToString());
                    foreach (var zip108Data in _temp108List)
                    {
                        lock (this)
                        {
                            _processDownDic.Add(zip108Data.Item1, zip108Data.Item2);
                        }
                    }

                    var _temp109List = GetZipsum109Today(_srchDate.ToString("yyyy-MM-dd"), node.Key.DownEq, node.Key.Node.ToString());
                    foreach (var zip109Data in _temp109List)
                    {
                        lock (this)
                        {
                            _process109Dic.Add(zip109Data.Item1, zip109Data.Item2);
                        }
                    }
                });
            });

            _106Task.Start();
            _108109Task.Start();

            //_mtdProdProcessSettings.GroupBy(gb => new { gb.Node, gb.DownEq }).ToList().ForEach(node =>
            //{
            //    GetZipsum108Today(_srchDate.ToString("yyyy-MM-dd"), node.Key.DownEq, node.Key.Node.ToString(), ref _processDownDic);
            //    GetZipsum109Today(_srchDate.ToString("yyyy-MM-dd"), node.Key.DownEq, node.Key.Node.ToString(), ref _process109Dic);
            //});

            //_mtdScheduleDataList.GroupBy(gb => gb.Node).ToList().ForEach(s =>
            //{
            //    GetZipsum108Today(_srchDate.ToString("yyyy-MM-dd"), _processEqDic[s.Key], s.Key, ref _processDownDic);
            //    GetZipsum109Today(_srchDate.ToString("yyyy-MM-dd"), _processEqDic[s.Key], s.Key, ref _process109Dic);
            //});

            // 取得機況 (未處理、已處理)
            List<AlarmXmlDao> _alarmOverList = _alarmXmlRepository.SelectForMTD(_srchDate, _mtdProdProcessSettings.Select(s => s.DownEq).Distinct().ToList(), _mtdProdProcessSettings.Select(s => s.ProdNo).Distinct().ToList());

            _106Task.Wait();
            _108109Task.Wait();

            // 資料合併
            Parallel.ForEach(_mtdProdProcessSettings.GroupBy(g => new { g.Process, g.DownEq }), (detail) =>
            {
                //_mtdProdProcessSettings.GroupBy(g => new { g.Process, g.DownEq }).ToList().ForEach(detail =>
                // {
                List<MTDDashboardDetailEntity> _tempDetailList = detail.Select(dt =>
                 {
                     var _currSchedule = _mtdScheduleDataList.FirstOrDefault(f => f.Process == dt.Process && f.ProductName == dt.ProdNo) ??
                     new MTDProductionScheduleDao
                     {
                         Date = _nowTime,
                         Value = 0
                     };

                     var _currMonth = _mtdMonthPlanList.Where(w => w.Node == dt.Node.ToString() && w.ProductName == dt.ProdNo);
                     var _currActualMonth = _mtdPerformanceMonth.Where(w => w.Node == dt.Node.ToString() && w.Product == dt.ProdNo);
                     var _currAlarmData = _alarmOverList.FirstOrDefault(f => f.tool_id == dt.DownEq && f.prod_id == dt.ProdNo);

                     MTDDashboardDetailEntity _test = new MTDDashboardDetailEntity
                     {
                         Date = _currSchedule.Date.ToString("MM/dd"),
                         Equipment = dt.DownEq,
                         BigProduct = _currSchedule.Model,
                         PlanProduct = dt.ProdNo,
                         Output = _mtdPerformanceDay.FirstOrDefault(f => f.Node == dt.Node.ToString() && f.Product == dt.ProdNo)?.Qty.ToString("#,0") ?? "0",
                         DayPlan = _currSchedule.Value.ToString("#,0"),
                         RangPlan = (_currSchedule.Value * (time / 24)).ToString("#,0"),
                         RangDiff = ((_mtdPerformanceDay.FirstOrDefault(f => f.Node == dt.Node.ToString() && f.Product == dt.ProdNo)?.Qty ?? 0) - (_currSchedule.Value * (time / 24))).ToString("#,0"),
                         MonthPlan = _currMonth.Sum(sum => sum.Value).ToString("#,0"),
                         MTDPlan = _currMonth.Where(mon => mon.Date <= _srchDate).Sum(sum => sum.Value).ToString("#,0"),
                         MTDActual = _currActualMonth.Sum(sum => sum.Qty).ToString("#,0"),
                         MTDDiff = (_currActualMonth.Sum(sum => sum.Qty) - _currMonth.Where(mon => mon.Date <= _srchDate).Sum(sum => sum.Value)).ToString("#,0"),
                         EqAbnormal = _currAlarmData == null ? "" : _currAlarmData.comment,
                         RepaireTime = _currAlarmData == null ? "" : _currAlarmData.spend_time.ToString(),
                         Status = _currAlarmData == null ? "" : _currAlarmData.end_time == null ? "處理中" : "已排除"
                     };

                     return _test;
                 }).ToList();

                var _currDownTime = _processDownDic[detail.Key.DownEq];

                lock (this)
                {
                    _mtdDashboardList.Add(new MTDDashboardSubEntity
                    {
                        Sn = detail.FirstOrDefault().Sn,
                        EqNo = Convert.ToInt32(_tempDetailList.FirstOrDefault().Equipment.Substring(4, 4)),
                        Process = detail.Key.Process,
                        Plan = _tempDetailList.Sum(sum => Convert.ToInt32(sum.RangPlan.Replace(",", ""))),
                        Actual = _tempDetailList.Sum(sum => Convert.ToInt32(sum.Output.Replace(",", ""))),
                        DownTime = _currDownTime.ToString(),
                        DownPercent = $"{_currDownTime / 1440 * 100:0.00}%",
                        UPPercent = _process109Dic[$"{detail.Key.DownEq}-UP"],
                        RUNPercent = _process109Dic[$"{detail.Key.DownEq}-RUN"],
                        UPHPercent = _process109Dic[$"{detail.Key.DownEq}-UPH"],
                        OEEPercent = _process109Dic[$"{detail.Key.DownEq}-OEE"],
                        MTDDetail = _tempDetailList.OrderByDescending(o => o.BigProduct).ToList()
                    });
                }
            });

            _mtdDashboardList = _mtdDashboardList.Select(data =>
            {
                data.Diff = data.Actual - data.Plan;                
                //data.MTDDetail = data.MTDDetail.Where(detail => detail.MTDPlan != "0").ToList();

                return data;
            }).OrderBy(ob => ob.Sn).ThenBy(tb => tb.EqNo).ToList();

            return _mtdDashboardList.GroupBy(g => g.Process).Select(data => new MTDDashboardMainEntity
            {
                Process = data.Key,
                MTDSubList = data.ToList()
            }).ToList();
        }

        private List<MTDPerformanceEntity> GetZipsum106Today(string startDate, string endDate, string prod, string node)
        {
            List<MTDPerformanceEntity> _tempZipsunEntity = new List<MTDPerformanceEntity>();

            string _qStr = $"Calendar1={startDate}&calendar2={endDate}&shift=&rwktype=ALL&floor=&prod_type=ALL&source_fab=1=1&wo_type=1=1&WO_NBR=&psize=&message=" +
                $"&prod_size={prod.Substring(2, 3)}&big_prod={prod.Substring(0, 9)}&Shop=MOD4&G_FAC=6&lcm_owner=('QTAP','LCME','PRDG','PROD','RES0')&calendar_1={startDate}" +
                $"&calendar_2={endDate}&vSelGroup=('{prod}')&LCDGrade=()";

            var data = new StringContent(_qStr, Encoding.UTF8, "application/x-www-form-urlencodedn");

            string[] array;

            using (var client = new HttpClient())
            {
                var response = client.PostAsync(_zipsum106Url + _qStr, data).Result;
                response.Content.Headers.ContentType.CharSet = "Big5";
                string result = response.Content.ReadAsStringAsync().Result;

                result = result.Remove(0, 12200);

                array = result.Split("<script language= \"VBScript\">");
            }

            _tempZipsunEntity.AddRange(Process(array, node, prod));

            return _tempZipsunEntity;
        }

        private List<MTDPerformanceEntity> GetZipsum106Month(string startDate, string endDate, string prod, string node)
        {
            List<MTDPerformanceEntity> _tempZipsunEntity = new List<MTDPerformanceEntity>();

            string _qStr = $"Calendar1={startDate}&calendar2={endDate}&shift=&rwktype=ALL&floor=&prod_type=ALL&source_fab=1=1&wo_type=1=1&WO_NBR=&psize=&message=" +
                $"&prod_size={prod.Substring(2, 3)}&big_prod={prod.Substring(0, 9)}&Shop=MOD4&G_FAC=6&lcm_owner=('QTAP','LCME','PRDG','PROD','RES0')&calendar_1={startDate}" +
                $"&calendar_2={endDate}&vSelGroup=('{prod}')&LCDGrade=()";

            var data = new StringContent(_qStr, Encoding.UTF8, "application/x-www-form-urlencodedn");

            string[] array;

            using (var client = new HttpClient())
            {
                var response = client.PostAsync(_zipsum106Url + _qStr, data).Result;
                response.Content.Headers.ContentType.CharSet = "Big5";

                string result = response.Content.ReadAsStringAsync().Result;

                result = result.Remove(0, 12200);

                array = result.Split("<script language= \"VBScript\">");

            }

            _tempZipsunEntity.AddRange(Process(array, node, prod));

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

        private List<(string, decimal)> GetZipsum108Today(string date, string passEq, string process)
        {
            List<(string, decimal)> _respList = new List<(string, decimal)>();

            FormUrlEncodedContent formUrlEncodedContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Calendar1", date),
                new KeyValuePair<string, string>("calendar2", date),
                new KeyValuePair<string, string>("shift", ""),
                new KeyValuePair<string, string>("intercond", "day"),
                new KeyValuePair<string, string>("floor", ""),
                new KeyValuePair<string, string>("eq_stat", "ALL"),
                new KeyValuePair<string, string>("big_eqp", $"{passEq.Substring(0, 4)}"),
                new KeyValuePair<string, string>("Shop", "MOD4"),
                new KeyValuePair<string, string>("G_FAC", "6"),
                new KeyValuePair<string, string>("calendar_1", date),
                new KeyValuePair<string, string>("calendar_2", date),
                new KeyValuePair<string, string>("eq_nbr", $"('{passEq}')"),
                new KeyValuePair<string, string>("pallet_nbr", ""),
            });

            string[] array;

            using (var client = new HttpClient())
            {
                var response = client.PostAsync(_zipsum108Url, formUrlEncodedContent).Result;
                response.Content.Headers.ContentType.CharSet = "Big5";
                string result = response.Content.ReadAsStringAsync().Result;

                result = result.Remove(0, 18758);

                array = result.Split("ReportGrid1.TextMatrix(row,");
            }

            array = array[array.Length - 2].Split("\r\n");

            var _downSum = (array[4].Replace(" ", "").Replace("\"", "").Split("="))[1];

            lock (this)
            {
                _respList.Add((passEq, Convert.ToDecimal(_downSum)));
            }

            return _respList;
        }

        private List<(string, string)> GetZipsum109Today(string date, string passEq, string process)
        {
            List<(string, string)> _respList = new List<(string, string)>();

            FormUrlEncodedContent formUrlEncodedContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Calendar1", date),
                new KeyValuePair<string, string>("calendar2", date),
                new KeyValuePair<string, string>("shift", ""),
                new KeyValuePair<string, string>("big_eqp", $"{passEq.Substring(0, 4)}"),
                new KeyValuePair<string, string>("Shop", "MOD4"),
                new KeyValuePair<string, string>("G_FAC", "6"),
                new KeyValuePair<string, string>("vDate", ""),
                new KeyValuePair<string, string>("vDate_s", date),
                new KeyValuePair<string, string>("vDate_e", date),
                new KeyValuePair<string, string>("vSelGrp", $"{passEq.Substring(0, 4)}"),
                new KeyValuePair<string, string>("vSelMac", $"('{passEq}')"),
                new KeyValuePair<string, string>("vSelR1", "")
            });

            string[] array;

            using (var client = new HttpClient())
            {
                var response = client.PostAsync(_zipsum109Url, formUrlEncodedContent).Result;
                response.Content.Headers.ContentType.CharSet = "Big5";
                string result = response.Content.ReadAsStringAsync().Result;

                result = result.Remove(0, 21580);

                array = result.Split("ReportGrid1.TextMatrix");
            }

            string _upStr = (array[21].Split("formatnumber(")[1]).Split(",")[0];
            string _runStr = (array[23].Split("formatnumber(")[1]).Split(",")[0];
            string _uphStr = (array[27].Split("formatnumber(")[1]).Split(",")[0];
            string _oeeStr = (array[31].Split("formatnumber(")[1]).Split(",")[0];
            _respList.Add(($"{passEq}-UP", $"{(_upStr.Length > 5 ? _upStr.Substring(0, 5) : _upStr)}%"));
            _respList.Add(($"{passEq}-RUN", $"{(_runStr.Length > 5 ? _runStr.Substring(0, 5) : _runStr)}%"));
            _respList.Add(($"{passEq}-UPH", $"{(_uphStr.Length > 5 ? _uphStr.Substring(0, 5) : _uphStr)}%"));
            _respList.Add(($"{passEq}-OEE", $"{(_oeeStr.Length > 5 ? _oeeStr.Substring(0, 5) : _oeeStr)}%"));

            return _respList;
        }

        #region ======== schedule ========

        public List<ManufactureScheduleEntity> Search(string dateRange = "", int floor = 2, int owner = 1)
        {
            try
            {
                DateTime _nowTime = DateTime.Now;
                DateTime _startDate = DateTime.Parse($"{_nowTime.ToString("yyyy/MM")}/01").AddDays(-10);
                DateTime _endDate = DateTime.Parse($"{_nowTime.AddMonths(1).ToString("yyyy/MM")}/01").AddDays(-1);

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
                List<MTDProductionScheduleDao> _mtdScheduleDataList = _mtdProductionScheduleRepository.SelectByConditions(floor, owner, _startDate, _endDate);
                List<MTDProductionScheduleDao> _mtdMonthPlanList = _mtdProductionScheduleRepository.SelectMonthPlanQty(_nowTime.Year.ToString(), _nowTime.Month.ToString(), floor, owner);

                _manufactureSchedules = _mtdScheduleDataList.GroupBy(gb => new { gb.Process, gb.Model, gb.ProductName })
                    .Select(mtd => new ManufactureScheduleEntity
                    {
                        Process = mtd.Key.Process,
                        Category = mtd.Key.Model,
                        MonthPlan = _mtdMonthPlanList.Where(w => w.Process == mtd.Key.Process && w.Model == mtd.Key.Model && w.ProductName == mtd.Key.ProductName).Sum(sum => sum.Value).ToString("#,0"),
                        ProductName = mtd.Key.ProductName,
                        PlanDetail = mtd.Select(s => new ManufactureDetailEntity
                        {
                            Date = s.Date.ToString("MM/dd"),
                            Quantity = s.Value,
                            IsToday = s.Date == _nowTime.Date
                        }).ToList()
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

                if (formFile.Length > 0)   // 檢查 <input type="file"> 是否輸入檔案？
                {
                    XSSFWorkbook workbook;
                    List<MTDProductionScheduleDao> _updMTDScheduleDao = new List<MTDProductionScheduleDao>();
                    MTDScheduleUpdateHistoryDao _mtdScheduleUpdateHistoryDao = new MTDScheduleUpdateHistoryDao();
                    Dictionary<int, DateTime> _dateDictionary = new Dictionary<int, DateTime>();
                    DateTime _nowTime = DateTime.Now;
                    string _newFileName = DoFileCopy(formFile, _nowTime);
                    (int, string) _processInfo = (0, "");

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
                        if (_headerDate.GetCell(k) != null)
                        {
                            _dateDictionary.Add(k, _headerDate.GetCell(k).DateCellValue);
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

                        if (string.IsNullOrEmpty(row.GetCell(0).StringCellValue) || string.IsNullOrEmpty(row.GetCell(1).StringCellValue) ||
                            string.IsNullOrEmpty(row.GetCell(4).StringCellValue) || row.GetCell(4).StringCellValue.Length < 10)
                            continue;

                        for (int j = 5; j < row.LastCellNum; j++)
                        {
                            _processInfo = ConvertProcessToSn(row.GetCell(0).StringCellValue);

                            _updMTDScheduleDao.Add(new MTDProductionScheduleDao
                            {
                                Sn = _processInfo.Item1,
                                Process = row.GetCell(0).StringCellValue.Trim(),
                                Date = _dateDictionary[j],
                                Node = _processInfo.Item2,
                                Model = row.GetCell(1).StringCellValue.Trim(),
                                ProductName = row.GetCell(4).StringCellValue.Trim(),
                                Value = Convert.ToInt32(row.GetCell(j).NumericCellValue),
                                Floor = floor,
                                OwnerId = owner,
                                UpdateUser = userEntity.Name,
                                UpdateTime = _nowTime,
                            });
                        }
                    }
                    #endregion

                    _mtdScheduleUpdateHistoryDao.FileName = _newFileName;
                    _mtdScheduleUpdateHistoryDao.Floor = floor;
                    _mtdScheduleUpdateHistoryDao.OwnerId = owner;
                    _mtdScheduleUpdateHistoryDao.UpdateUser = userEntity.Name;
                    _mtdScheduleUpdateHistoryDao.UpdateTime = _nowTime;

                    using (TransactionScope _scope = new TransactionScope())
                    {
                        bool _updRes = false;
                        bool _insHisRes = false;

                        _updRes = _mtdProductionScheduleRepository.DeleteSchedule(owner) == _updMTDScheduleDao.Count;
                        _updRes = _mtdProductionScheduleRepository.InsertSchedule(_updMTDScheduleDao) == _updMTDScheduleDao.Count;
                        _insHisRes = _mtdProductionScheduleRepository.InsertScheduleHistory(_mtdScheduleUpdateHistoryDao) == 1;

                        if (_updRes && _insHisRes)
                            _scope.Complete();
                        else
                            _uploadResult = "更新異常";
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
                default:
                    return (0, "");
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
    }
}