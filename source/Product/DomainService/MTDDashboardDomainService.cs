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
using System.Transactions;

namespace MOD4.Web.DomainService
{
    public class MTDDashboardDomainService : IMTDDashboardDomainService
    {
        private readonly IMTDProductionScheduleRepository _mtdProductionScheduleRepository;
        private readonly IUploadDomainService _uploadDomainService;

        public MTDDashboardDomainService(IMTDProductionScheduleRepository mtdProductionScheduleRepository,
            IUploadDomainService uploadDomainService)
        {
            _mtdProductionScheduleRepository = mtdProductionScheduleRepository;
            _uploadDomainService = uploadDomainService;
        }

        public void DashboardSearch(string date, decimal time)
        {
            DateTime _nowTime = DateTime.Now;
            DateTime _srchDate = _nowTime;
            var url = "http://zipsum/modreport/Report/SHOPMOD/OperPerfDetaSet.asp?";

            if (DateTime.TryParseExact(date, "yyyy-MM-dd", null, DateTimeStyles.None, out _))
                DateTime.TryParseExact(date, "yyyy/MM/dd", null, DateTimeStyles.None, out _srchDate);

            List<MTDDashboardEntity> _mtdDashboardList = new List<MTDDashboardEntity>();
            List<MTDProductionScheduleDao> _mtdScheduleDataList = _mtdProductionScheduleRepository.SelectByConditions(_srchDate, _srchDate);
            List<MTDProductionScheduleDao> _mtdMonthPlanList = _mtdProductionScheduleRepository.SelectMonthPlanQty(_nowTime.Year.ToString(), _nowTime.Month.ToString());

            _mtdDashboardList = _mtdScheduleDataList.GroupBy(g => new { g.Date, g.Node, g.Process, g.Model, g.ProductName })
                .Select(s =>
                {
                    GetZipsum106Today(s.Key.Date.ToString("yyyy-MM-dd"), s.Key.Date.ToString("yyyy-MM-dd"), s.Key.ProductName, url, s.Key.Node);
                    GetZipsum106Month($"{s.Key.Date.ToString("yyyy-MM")}-01", s.Key.Date.ToString("yyyy-MM-dd"), s.Key.ProductName, url, s.Key.Node);
                    MTDDashboardEntity _tmpEntity = new MTDDashboardEntity
                    {
                        Process = s.Key.Process,
                        Detail = s.Select(detail => new MTDDashboardDetailEntity
                        {
                            Date = s.Key.Date.ToString("MM/dd"),

                        }).ToList()
                    };

                    return _tmpEntity;

                }).ToList();

        }

        private void GetZipsum106Today(string startDate, string endDate, string prod, string node, string url)
        {
            string _qStr = $"Calendar1={startDate}&calendar2={endDate}&shift=&rwktype=ALL&floor=&prod_type=ALL&source_fab=1=1&wo_type=1=1&WO_NBR=&psize=&message=" +
                $"&prod_size={prod.Substring(2, 3)}&big_prod={prod.Substring(0, 9)}&Shop=MOD4&G_FAC=6&lcm_owner=('QTAP','LCME','PRDG','PROD','RES0')&calendar_1={startDate}" +
                $"&calendar_2={endDate}&vSelGroup=('{prod}')&LCDGrade=()";

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

            Process(array, node);
        }

        private void GetZipsum106Month(string startDate, string endDate, string prod, string node, string url)
        {
            string _qStr = $"Calendar1={startDate}&calendar2={endDate}&shift=&rwktype=ALL&floor=&prod_type=ALL&source_fab=1=1&wo_type=1=1&WO_NBR=&psize=&message=" +
                $"&prod_size={prod.Substring(2, 3)}&big_prod={prod.Substring(0, 9)}&Shop=MOD4&G_FAC=6&lcm_owner=('QTAP','LCME','PRDG','PROD','RES0')&calendar_1={startDate}" +
                $"&calendar_2={endDate}&vSelGroup=('{prod}')&LCDGrade=()";

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

            Process(array, node);

        }

        private int Process(string[] detailStr, string node)
        {
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
                        break;
                    case 2:
                        _tempArray = detailStr[i].Split("=");
                        _tempArray = _tempArray[2].Split("\r\n");
                        _tempStr = _tempArray[0].Replace("\"", "");
                        DateTime _outTime;
                        DateTime.TryParse(_tempStr, out _outTime);
                        break;
                    case 4:
                        _tempArray = detailStr[i].Split("=");
                        _tempArray = _tempArray[2].Split("\r\n");
                        _tempStr = _tempArray[0].Replace("\"", "");
                        break;
                    case 12:
                        _tempArray = detailStr[i].Split("=");
                        _tempArray = _tempArray[2].Split("\r\n");
                        _tempStr = _tempArray[0].Replace("\"", "");
                        break;
                }
            }

            return 0;
        }

        #region ======== schedule ========

        public List<ManufactureScheduleEntity> Search(string dateRange = "")
        {
            try
            {
                DateTime _nowTime = DateTime.Now;
                DateTime _startDate = DateTime.Parse($"{_nowTime.AddMonths(-1).ToString("yyyy/MM")}/01");
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
                List<MTDProductionScheduleDao> _mtdScheduleDataList = _mtdProductionScheduleRepository.SelectByConditions(_startDate, _endDate);
                List<MTDProductionScheduleDao> _mtdMonthPlanList = _mtdProductionScheduleRepository.SelectMonthPlanQty(_nowTime.Year.ToString(), _nowTime.Month.ToString());

                _manufactureSchedules = _mtdScheduleDataList.GroupBy(gb => new { gb.Process, gb.Model, gb.ProductName })
                    .Select(mtd => new ManufactureScheduleEntity
                    {
                        Process = mtd.Key.Process,
                        Category = mtd.Key.Model,
                        MonthPlan = _mtdMonthPlanList.FirstOrDefault(f => f.Process == mtd.Key.Process && f.Model == mtd.Key.Model && f.ProductName == mtd.Key.ProductName).MonthPlan.ToString(),
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

        public string Upload(IFormFile formFile, UserEntity userEntity)
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
                    XSSFSheet _verifySheet = (XSSFSheet)workbook.GetSheetAt(1); // 驗證 sheet

                    StringBuilder SB = new StringBuilder(); // System.Text命名空間

                    XSSFRow _headerDate = (XSSFRow)_scheduleSheet.GetRow(0); // 取得表頭日期

                    // 表頭列，共有幾個 "欄位"?（取得最後一欄的數字）
                    for (int k = 4; k < _headerDate.LastCellNum; k++)
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
                            string.IsNullOrEmpty(row.GetCell(3).StringCellValue))
                            continue;

                        for (int j = 4; j < row.LastCellNum; j++)
                        {
                            _processInfo = ConvertProcessToSn(row.GetCell(0).StringCellValue);

                            _updMTDScheduleDao.Add(new MTDProductionScheduleDao
                            {
                                Sn = _processInfo.Item1,
                                Process = row.GetCell(0).StringCellValue.Trim(),
                                Node = _processInfo.Item2,
                                Model = row.GetCell(1).StringCellValue.Trim(),
                                ProductName = row.GetCell(3).StringCellValue.Trim(),
                                Value = Convert.ToInt32(row.GetCell(j).NumericCellValue),
                                Date = _dateDictionary[j],
                                UpdateUser = userEntity.Name,
                                UpdateTime = _nowTime,
                            });
                        }
                    }
                    #endregion

                    _mtdScheduleUpdateHistoryDao.FileName = _newFileName;
                    _mtdScheduleUpdateHistoryDao.UpdateUser = userEntity.Name;
                    _mtdScheduleUpdateHistoryDao.UpdateTime = _nowTime;

                    using (TransactionScope _scope = new TransactionScope())
                    {
                        bool _updRes = false;
                        bool _insHisRes = false;

                        _updRes = _mtdProductionScheduleRepository.DeleteSchedule() == _updMTDScheduleDao.Count;
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
                    return (1, "1100");
                case "FOG":
                    return (2, "1330");
                case "LAM":
                    return (3, "1355");
                case "ASSY":
                    return (4, "1415");
                case "CDP":
                    return (5, "1600");
                default:
                    return (0, "");
            }
        }

        #endregion
    }
}