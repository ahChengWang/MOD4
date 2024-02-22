using Microsoft.AspNetCore.Http;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using System;
using System.IO;
using Ionic.Zip;
using Utility.Helper;
using System.Net;
using MOD4.Web.Repostory.Dao;
using MOD4.Web.Repostory;
using System.Transactions;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Data.SqlClient;

namespace MOD4.Web.DomainService
{
    public class ExtensionDomainService : BaseDomainService, IExtensionDomainService
    {
        private readonly IUploadDomainService _uploadDomainService;
        private readonly IAccountDomainService _accountDomainService;
        private readonly IMPSUploadHistoryRepository _mpsUploadHistoryRepository;
        private readonly ILightingLogRepository _lightingLogRepository;
        private readonly IDefinitionRWDefectCodeRepository _definitionRWDefectCodeRepository;

        public ExtensionDomainService(IUploadDomainService uploadDomainService,
            IAccountDomainService accountDomainService,
            IMPSUploadHistoryRepository mpsUploadHistoryRepository,
            ILightingLogRepository lightingLogRepository,
            IDefinitionRWDefectCodeRepository definitionRWDefectCodeRepository)
        {
            _uploadDomainService = uploadDomainService;
            _accountDomainService = accountDomainService;
            _mpsUploadHistoryRepository = mpsUploadHistoryRepository;
            _lightingLogRepository = lightingLogRepository;
            _definitionRWDefectCodeRepository = definitionRWDefectCodeRepository;
        }

        public List<LightingLogMainEntity> GetLightingHisList(string panelId = "")
        {
            try
            {
                DateTime _nowTime = DateTime.Now;
                DateTime _startDate = DateTime.Parse($"{_nowTime.AddMonths(-6).ToString("yyyy-MM-dd")} 00:00:00");
                DateTime _endDate = DateTime.Parse($"{_nowTime.ToString("yyyy-MM-dd")} 23:59:59");
                List<LightingLogDao> _lightingLogList = new List<LightingLogDao>();

                if (string.IsNullOrEmpty(panelId))
                    _lightingLogList = _lightingLogRepository.SelectByConditions(startDate: _startDate, endDate: _endDate);
                else
                    _lightingLogList = _lightingLogRepository.SelectByConditions(panelId: panelId);

                List<LightingLogMainEntity> _res = _lightingLogList.GroupBy(g => g.PanelDate.Date)
                    .Select(panel => new LightingLogMainEntity
                    {
                        LogDate = panel.Key,
                        ProcessList = panel.GroupBy(gb => gb.CategoryId)
                        .Select(detail => new LightingLogSubEntity
                        {
                            CategoryId = detail.Key,
                            Category = detail.Key.GetDescription(),
                            ProcessCnt = detail.Count()
                        }).ToList()
                    }).ToList();

                return _res.OrderBy(o => o.LogDate).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<LightingLogEntity> GetLightingDayLogList(string panelDate, LightingCategoryEnum categoryId)
        {
            try
            {
                DateTime _nowTime = DateTime.Now;
                DateTime _srcDate;
                List<LightingLogDao> _lightingLogList = new List<LightingLogDao>();

                if (DateTime.TryParseExact(panelDate, "yyyy-MM-dd", null, DateTimeStyles.None, out _))
                    DateTime.TryParseExact(panelDate, "yyyy-MM-dd", null, DateTimeStyles.None, out _srcDate);
                else
                    throw new Exception("查詢日期異常");

                _lightingLogList = _lightingLogRepository.SelectByConditions(startDate: _srcDate, endDate: _srcDate.AddDays(1).AddSeconds(-1), categoryId: categoryId);

                List<LightingLogEntity> _res = _lightingLogList.CopyAToB<LightingLogEntity>();

                return _res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<LightingLogEntity> GetLightingLogById(string panelId)
        {
            try
            {
                return _lightingLogRepository.SelectByConditions(panelId: panelId).CopyAToB<LightingLogEntity>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string CreateLightingLog(List<LightingLogEntity> lightingLogList, UserEntity userEntity)
        {
            try
            {
                string _createRes = "";
                DateTime _nowTime = DateTime.Now;
                List<LightingLogDao> _insLightingLogList = new List<LightingLogDao>();

                _insLightingLogList = lightingLogList.Select(log => new LightingLogDao
                {
                    CategoryId = log.CategoryId,
                    Floor = 2,
                    PanelId = log.PanelId,
                    PanelDate = log.PanelDate,
                    StatusId = log.StatusId,
                    DefectCatgId = log.DefectCatgId,
                    DefectCode = log.DefectCode,
                    CreateUser = $"{userEntity.JobId}-{userEntity.Name}",
                    CreateDate = _nowTime,
                    UpdateUser = $"{userEntity.JobId}-{userEntity.Name}",
                    UpdateDate = _nowTime
                }).ToList();

                using (TransactionScope _scope = new TransactionScope())
                {
                    bool _insRes;

                    _insRes = _lightingLogRepository.InsertLightingLog(_insLightingLogList) == _insLightingLogList.Count();

                    if (_insRes)
                        _scope.Complete();
                    else
                        _createRes = "新增記錄異常";
                }

                return _createRes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string UpdateLightingLog(List<LightingLogEntity> lightingLogList, UserEntity userEntity)
        {
            try
            {
                string _updateRes = "";
                DateTime _nowTime = DateTime.Now;

                List<LightingLogDao> _oldLightingLogList = new List<LightingLogDao>();
                List<LightingLogDao> _updLightingLogList = new List<LightingLogDao>();
                List<LightingLogDao> _insLightingLogList = new List<LightingLogDao>();

                _oldLightingLogList = _lightingLogRepository.SelectByConditions(snList: lightingLogList.Where(w => w.PanelSn != 0).Select(s => s.PanelSn).ToList());

                _updLightingLogList = lightingLogList.Where(w => w.PanelSn != 0).Select(log => new LightingLogDao
                {
                    PanelSn = log.PanelSn,
                    StatusId = log.StatusId,
                    DefectCatgId = log.DefectCatgId,
                    DefectCode = log.DefectCode,
                    UpdateUser = $"{userEntity.JobId}-{userEntity.Name}",
                    UpdateDate = _nowTime
                }).ToList();

                _updLightingLogList = _updLightingLogList.Except(_oldLightingLogList).ToList();

                _insLightingLogList = lightingLogList.Where(w => w.PanelSn == 0).Select(log => new LightingLogDao
                {
                    CategoryId = log.CategoryId,
                    Floor = 2,
                    PanelId = log.PanelId,
                    PanelDate = log.PanelDate,
                    StatusId = log.StatusId,
                    DefectCatgId = log.DefectCatgId,
                    DefectCode = log.DefectCode,
                    CreateUser = $"{userEntity.JobId}-{userEntity.Name}",
                    CreateDate = _nowTime,
                    UpdateUser = $"{userEntity.JobId}-{userEntity.Name}",
                    UpdateDate = _nowTime
                }).ToList();

                using (TransactionScope _scope = new TransactionScope())
                {
                    if (_lightingLogRepository.InsertLightingLog(_insLightingLogList) == _insLightingLogList.Count() &&
                        _lightingLogRepository.UpdateRWLog(_updLightingLogList) == _updLightingLogList.Count())
                        _scope.Complete();
                    else
                        _updateRes = "更新記錄異常";
                }

                return _updateRes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public (bool, string, string) DownloadLog(DateTime logDate)
        {
            try
            {
                DateTime _startDate = DateTime.Parse($"{logDate.ToString("yyyy-MM-dd")} 00:00:00");
                DateTime _endDate = DateTime.Parse($"{logDate.ToString("yyyy-MM-dd")} 23:59:59");

                var _lightingLogList = _lightingLogRepository.SelectByConditions(startDate: _startDate, endDate: _endDate);
                var _defRWDefectcode = _definitionRWDefectCodeRepository.SelectByConditions();

                if (_lightingLogList.Any())
                {
                    XSSFWorkbook workbook;

                    // 刪除暫存計算檔
                    string[] _dirAllFiles = Directory.GetFiles("..\\template\\");
                    if (_dirAllFiles.Length != 0)
                        foreach (string filePath in _dirAllFiles)
                        {
                            if (filePath.Contains("rw_log_export"))
                                File.Delete(filePath);
                        }

                    File.Copy($"..\\template\\rw_log.xlsx", $"..\\template\\rw_log_export.xlsx");

                    // 新增暫存計算檔
                    using (FileStream fs = new FileStream($"..\\template\\rw_log_export.xlsx", FileMode.Open, FileAccess.Read))
                    {
                        workbook = new XSSFWorkbook(fs); // 將剛剛的Excel (Stream）讀取到工作表裡面
                    }

                    ISheet _sheet = workbook.GetSheet("Sheet");
                    IRow _row;
                    ICell _cell;

                    ICellStyle _cellStyle = workbook.CreateCellStyle();
                    IFont _font = workbook.CreateFont();
                    _font.Color = IndexedColors.Black.Index;
                    _font.FontName = "新細明體";
                    _cellStyle.SetFont(_font);
                    _cellStyle.VerticalAlignment = VerticalAlignment.Center;
                    _cellStyle.Alignment = HorizontalAlignment.Center;
                    _cellStyle.BorderTop = BorderStyle.Thin;
                    _cellStyle.BorderRight = BorderStyle.Thin;
                    _cellStyle.BorderBottom = BorderStyle.Thin;
                    _cellStyle.BorderLeft = BorderStyle.Thin;

                    for (int r = 0; r < _lightingLogList.Count; r++)
                    {
                        var _tmpDefCode = _defRWDefectcode.FirstOrDefault(f => (int)f.CategoryId == _lightingLogList[r].DefectCatgId && f.Code == (_lightingLogList[r]?.DefectCode ?? ""));

                        _sheet.CreateRow(r + 1);
                        _row = _sheet.GetRow(r + 1);
                        _row.CreateCell(0);
                        _cell = _row.GetCell(0);
                        _cell.SetCellValue(_lightingLogList[r].CategoryId.GetDescription());
                        _cell.CellStyle = _cellStyle;
                        _row.CreateCell(1);
                        _cell = _row.GetCell(1);
                        _cell.SetCellValue(_lightingLogList[r].PanelId);
                        _cell.CellStyle = _cellStyle;
                        _row.CreateCell(2);
                        _cell = _row.GetCell(2);
                        _cell.SetCellValue(_lightingLogList[r].PanelDate.ToString("yyyy-MM-dd HH:mm:ss"));
                        _cell.CellStyle = _cellStyle;
                        _row.CreateCell(3);
                        _cell = _row.GetCell(3);
                        _cell.SetCellValue(_lightingLogList[r].StatusId == 0 ? "NG" : "OK");
                        _cell.CellStyle = _cellStyle;
                        _row.CreateCell(4);
                        _cell = _row.GetCell(4);
                        _cell.SetCellValue(_tmpDefCode != null ? $"{_tmpDefCode.Code}-{_tmpDefCode.Desc}" : "");
                        _cell.CellStyle = _cellStyle;
                        _row.CreateCell(5);
                        _cell = _row.GetCell(5);
                        _cell.SetCellValue(_lightingLogList[r].UpdateUser);
                        _cell.CellStyle = _cellStyle;
                    }

                    // 回寫本地計算檔
                    using (FileStream fs = new FileStream($"..\\template\\rw_log_export.xlsx", FileMode.Create))
                    {
                        workbook.Write(fs);
                    }
                }

                return (true, $"..\\template\\rw_log_export.xlsx", $"rw_log_export.xlsx");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string Upload(string jobId, ApplyAreaEnum applyAreaId, int itemId, IFormFile uploadFile, UserEntity userEntity)
        {
            try
            {
                var _url = "D:\\";
                var _folder = $@"CertifiedUpload\{jobId}\{applyAreaId.GetDescription()}\{itemId}";

                if (!Directory.Exists($@"{_url}\{_folder}"))
                {
                    Directory.CreateDirectory($@"{_url}\{_folder}");
                }

                string[] _nameArray = uploadFile.FileName.Split(".");

                var path = $@"{_url}\{_folder}\{_nameArray[0]}_{DateTime.Now.ToString("yyyyMMdd")}.{_nameArray[1]}";

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    uploadFile.CopyTo(stream);
                }

                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
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


        #region === MPS ===

        public string MPSUpload(IFormFile uploadFile, UserEntity userEntity)
        {
            try
            {
                string _uplRes = "";
                string[] _fileNameAry = uploadFile.FileName.Split(".");
                var fileName = Path.GetFileName($"{_fileNameAry[0]}_{Guid.NewGuid().ToString("N").Substring(0, 4)}.{_fileNameAry[1]}");

                _ftpService.FTP_Upload(uploadFile, "FTP_MPS", fileName, false, "");

                string[] _nameAry = uploadFile.FileName.Split(".");

                MPSUploadHistoryDao _mpsUplHisDao = new MPSUploadHistoryDao
                {
                    FileName = fileName,
                    UploadTime = DateTime.Now,
                    UploadUser = userEntity.Name
                };

                using (TransactionScope _scope = new TransactionScope())
                {
                    bool _insRes;

                    _insRes = _mpsUploadHistoryRepository.Insert(_mpsUplHisDao) == 1;

                    if (_insRes)
                        _scope.Complete();
                    else
                        _uplRes = "上傳記錄新增異常";
                }

                return _uplRes;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public (bool, string, string) Download()
        {
            try
            {
                MPSUploadHistoryDao _mpsDao = _mpsUploadHistoryRepository.SelectLatest();

                if (_mpsDao == null)
                    return (true, "", "");

                var _fileStr = _ftpService.FTP_Download("FTP_MPS", _mpsDao.FileName);

                return (true, _fileStr, _mpsDao.FileName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
