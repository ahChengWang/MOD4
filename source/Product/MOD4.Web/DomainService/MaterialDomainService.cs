using Ionic.Zip;
using Microsoft.AspNetCore.Http;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using MOD4.Web.Repostory;
using MOD4.Web.Repostory.Dao;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Transactions;
using Utility.Helper;

namespace MOD4.Web.DomainService
{
    public class MaterialDomainService : BaseDomainService, IMaterialDomainService
    {
        private readonly IUploadDomainService _uploadDomainService;
        private readonly IMPSUploadHistoryRepository _mpsUploadHistoryRepository;

        public MaterialDomainService(IUploadDomainService uploadDomainService,
            IMPSUploadHistoryRepository mpsUploadHistoryRepository)
        {
            _uploadDomainService = uploadDomainService;
            _mpsUploadHistoryRepository = mpsUploadHistoryRepository;
        }

        /// <summary>
        /// SAP 撥料檔上傳流程：本地新增計算檔 -> 上傳檔計算 -> 寫入計算檔 -> 上傳 FTP -> 觸發下載計算檔
        /// </summary>
        /// <param name="uploadFile">上傳檔案</param>
        /// <param name="userEntity">使用者資訊</param>
        /// <returns></returns>
        public (bool, string, string) Upload(IFormFile uploadFile, UserEntity userEntity)
        {
            try
            {
                if (uploadFile.Length > 0)   // 檢查 <input type="file"> 是否輸入檔案？
                {
                    XSSFWorkbook workbook;
                    List<SAPWorkOrderEntity> _insSAPWorkOrderList = new List<SAPWorkOrderEntity>();
                    MTDScheduleUpdateHistoryDao _mtdScheduleUpdateHistoryDao = new MTDScheduleUpdateHistoryDao();
                    DateTime _nowTime = DateTime.Now;

                    // 刪除暫存計算檔
                    string[] _dirAllFiles = Directory.GetFiles("..\\tempFileProcess\\");
                    if (_dirAllFiles.Length != 0)
                        foreach (string filePath in _dirAllFiles)
                            File.Delete(filePath);

                    string[] _fileNameAry = uploadFile.FileName.Split(".");
                    var fileName = Path.GetFileName($"{_fileNameAry[0]}_{_nowTime.ToString("yyMMddHHmmss")}.{_fileNameAry[1]}");

                    // 新增暫存計算檔
                    using (FileStream fs = new FileStream($"..\\tempFileProcess\\{fileName}", FileMode.Create))
                    {
                    }                    

                    using (var ms = new MemoryStream())
                    {
                        uploadFile.CopyTo(ms);

                        // NOPI 讀取檔案內容（Stream串流）
                        Stream stream = new MemoryStream(ms.ToArray());

                        workbook = new XSSFWorkbook(stream); // 將剛剛的Excel (Stream）讀取到工作表裡面
                        // XSSFWorkbook() 只能讀取 System.IO.Stream
                    }
                    
                    #region
                    XSSFSheet _scheduleSheet = (XSSFSheet)workbook.GetSheetAt(0);

                    // 表頭列新增"理論撥料"、"修正數"
                    XSSFRow row = (XSSFRow)_scheduleSheet.GetRow(0);

                    // font red
                    ICellStyle _cellStyle = workbook.CreateCellStyle();
                    IFont _font = workbook.CreateFont();
                    _font.Color = IndexedColors.Red.Index;
                    _cellStyle.SetFont(_font);

                    ICell _theortOut = row.CreateCell(19);
                    ICell _diffOut = row.CreateCell(20);
                    _theortOut.SetCellValue("理論撥料");
                    _diffOut.SetCellValue("修正數");

                    // for迴圈的「啟始值」要加一，表示不包含 Excel表頭列
                    for (int i = (_scheduleSheet.FirstRowNum + 1); i <= _scheduleSheet.LastRowNum; i++)
                    {
                        // 每一列做迴圈
                        row = (XSSFRow)_scheduleSheet.GetRow(i); // 不包含 Excel表頭列

                        SAPWorkOrderEntity _tmpEntity = new SAPWorkOrderEntity()
                        {
                            Order = row.GetCell(0).StringCellValue,
                            Prod = row.GetCell(1).StringCellValue,
                            MaterialSpec = row.GetCell(2).StringCellValue,
                            MaterialNo = row.GetCell(3).StringCellValue,
                            MaterialName = row.GetCell(4).StringCellValue,
                        };

                        _insSAPWorkOrderList.Add(new SAPWorkOrderEntity
                        {
                            Order = row.GetCell(0).StringCellValue,
                            Prod = row.GetCell(1).StringCellValue,
                            MaterialSpec = row.GetCell(2).StringCellValue,
                            MaterialNo = row.GetCell(3).StringCellValue,
                            MaterialName = row.GetCell(4).StringCellValue,
                            SAPNode = row.GetCell(5)?.StringCellValue ?? "",
                            Dept = row.GetCell(6).StringCellValue,
                            ProdQty = Convert.ToInt32(row.GetCell(7).NumericCellValue),
                            StorageQty = Convert.ToInt32(row.GetCell(8).NumericCellValue),
                            StartDate = row.GetCell(9).DateCellValue,
                            FinishDate = row.GetCell(10).DateCellValue,
                            Unit = Convert.ToInt32(row.GetCell(11).NumericCellValue),
                            ExptQty = Convert.ToInt32(row.GetCell(12).NumericCellValue),
                            DisburseQty = Convert.ToInt32(row.GetCell(13).NumericCellValue),
                            ReturnQty = Convert.ToInt32(row.GetCell(14).NumericCellValue),
                            ActStorageQty = Convert.ToInt32(row.GetCell(15).NumericCellValue),
                            ScrapQty = Convert.ToInt32(row.GetCell(16).NumericCellValue),
                            DiffQty = Convert.ToInt32(row.GetCell(17).NumericCellValue),
                            DiffRate = Convert.ToDecimal(row.GetCell(18).NumericCellValue)
                        });

                        #region 計算"理論撥料"、"修正數"
                        var _culQty = Convert.ToInt32(row.GetCell(12).NumericCellValue) * 1.03;
                        var _diffCulQty = Convert.ToInt32(row.GetCell(13).NumericCellValue) - Math.Round(_culQty);

                        _theortOut = row.CreateCell(19);
                        _theortOut.SetCellType(CellType.Numeric);
                        _theortOut.SetCellValue(_culQty);

                        _diffOut = row.CreateCell(20);
                        _diffOut.SetCellType(CellType.String);
                        _diffOut.SetCellValue(_diffCulQty);

                        // 修正數 < 0 上紅色
                        if (_diffCulQty < 0)
                        {
                            _diffOut.CellStyle = _cellStyle;
                        }
                        #endregion
                    }
                    #endregion

                    // 回寫本地計算檔
                    using (FileStream fs = new FileStream($"..\\tempFileProcess\\{fileName}", FileMode.Open, FileAccess.Write))
                    {
                        workbook.Write(fs);
                        fs.Close();
                    }

                    // FTP 上傳
                    _ftpService.FTP_Upload(uploadFile, "FTP_SAP", fileName, true, $"..\\tempFileProcess\\{fileName}");

                    return (true, $"..\\tempFileProcess\\{fileName}", fileName);
                }
                else
                    return (false, "", "");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public (byte[], string) Download(string jobId, ApplyAreaEnum applyAreaId, int itemId, UserEntity userEntity)
        {
            try
            {
                var _url = "D:\\";
                var _folder = $@"CertifiedUpload\{jobId}\{applyAreaId.GetDescription()}\{itemId}";
                string[] _allFiles = Directory.GetFiles($@"{_url}\{_folder}");
                var bytes = default(byte[]);

                if (!Directory.Exists($@"{_url}\{_folder}") || _allFiles.Length == 0)
                {
                    return (null, "人員無相關上傳檔案");
                }

                using (var zip = new ZipFile())
                {
                    //zip.Password = "P@ssW0rd";

                    foreach (var file in _allFiles)
                    {
                        string[] _pathArray = file.Split("\\");
                        zip.AddEntry(_pathArray[_pathArray.Length - 1], new byte[] { });
                    }

                    using (var ms = new MemoryStream())
                    {
                        zip.Save(ms);
                        bytes = ms.ToArray();
                    }
                }

                return (bytes, "");
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }

        private string[] GetZipsumReport418(string woOrder, string dateStr)
        {

            string _qStr = $"wo_area=+and+status<>'3'+&wotype=ALL&iswodueday=N&Calendar1={dateStr}&calendar2={dateStr}&iswostartday=N&Calendar2_1={dateStr}&calendar2_2={dateStr}&workorder={woOrder}&prod_type=ALL&wo_status=ALL&mvin_rate=ALL&mvou_rate=ALL&G_FAC=6&Shop=MOD4&calendar_1={dateStr}&calendar_2={dateStr}&calendar_2_1={dateStr}&calendar_2_2={dateStr}";
            var data = new StringContent(_qStr, Encoding.UTF8, "application/x-www-form-urlencodedn");

            string[] array;

            using (var client = new HttpClient())
            {
                var response = client.PostAsync("http://zipsum/modreport/Report/MOD4/NHWoUnClose60DataSet.asp", data).Result;

                response.Content.Headers.ContentType.CharSet = "Big5";

                string result = response.Content.ReadAsStringAsync().Result;

                result = result.Remove(0, 33000);

                array = result.Split("<SCRIPT LANGUAGE=vbscript>");
            }

            return array;
        }
    }
}
