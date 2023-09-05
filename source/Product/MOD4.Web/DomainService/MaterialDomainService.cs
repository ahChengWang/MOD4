using Ionic.Zip;
using Microsoft.AspNetCore.Http;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using MOD4.Web.Repostory;
using MOD4.Web.Repostory.Dao;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Utility.Helper;

namespace MOD4.Web.DomainService
{
    public class MaterialDomainService : BaseDomainService, IMaterialDomainService
    {
        private readonly ISAPMaterialRepository _sapMaterialRepository;

        private Dictionary<string, XSSFColor> _bgColorDic = new Dictionary<string, XSSFColor>
        {
            {"sapH",new XSSFColor(new byte[3]{ 62, 163, 96})},
            {"calcH",new XSSFColor(new byte[3]{ 62, 163, 96})},
            {"rptH",new XSSFColor(new byte[3]{ 184, 121, 55})},
            {"calcContent",new XSSFColor(new byte[3]{136, 191, 154 })},
            {"rptContent",new XSSFColor(new byte[3]{ 194, 153, 110})},
        };
        private Dictionary<string, string> _woTypeDic = new Dictionary<string, string>
        {
            {"PROD","P"},
            {"QTAP","P"},
            {"RES0","P"},
            {"LCME","P"},
            {"INT0","E"},
            {"LCM0","E"},
            {"RWKP","R"},
            {"RWK0","R"},
            {"RIN0","R"},
            {"RWK1","R"},
            {"RQR0","R"},
            {"RWKY","R"},
            {"RWDG","R"},
            {"RMA0","R"},
            {"RMB0","M"},
            {"PRDG","P"},
            {"RDD0","R"}
        };
        private Dictionary<MatlCodeTypeEnum, string> _codeTypeFolderDic = new Dictionary<MatlCodeTypeEnum, string>
        {
            {MatlCodeTypeEnum.Code5,"\\Code5"},
            {MatlCodeTypeEnum.Code13,"\\Code13"}
        };

        public MaterialDomainService(ISAPMaterialRepository sapMaterialRepository)
        {
            _sapMaterialRepository = sapMaterialRepository;
        }

        public List<MaterialSettingEntity> GetMaterialSetting(MatlCodeTypeEnum codeTypeId)
        {
            try
            {
                List<MaterialSettingDao> _matlAllSetting = _sapMaterialRepository.SelectMatlAllSetting(codeTypeId);

                return _matlAllSetting.OrderBy(o => o.MatlNo).Select(matl => new MaterialSettingEntity
                {
                    Sn = matl.Sn,
                    MatlNo = matl.MatlNo,
                    MatlCatg = matl.MatlCatg,
                    MatlName = matl.MatlName,
                    UseNode = matl.UseNode,
                    LossRate = matl.LossRate
                }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string UpdateMaterialSetting(List<MaterialSettingEntity> updEntity, MatlCodeTypeEnum codeTypeId, UserEntity userEntity)
        {
            try
            {
                string _updRes = "";
                DateTime _nowTime = DateTime.Now;

                List<MaterialSettingDao> _oldMatlSetting = _sapMaterialRepository.SelectMatlAllSetting(codeTypeId);

                List<MaterialSettingDao> _updateMatlSetting = (from old in _oldMatlSetting
                                                               join upd in updEntity
                                                               on old.MatlNo equals upd.MatlNo
                                                               where old.LossRate != upd.LossRate
                                                               select new MaterialSettingDao
                                                               {
                                                                   MatlNo = old.MatlNo,
                                                                   LossRate = upd.LossRate,
                                                                   OldLossRate = old.LossRate
                                                               }).ToList();

                List<MaterialSettingHistoryDao> _updateMatlHis = _updateMatlSetting
                    .Select(setting => new MaterialSettingHistoryDao
                    {
                        MatlNo = setting.MatlNo,
                        LossRate = setting.OldLossRate,
                        UpdateUser = userEntity.Name,
                        UpdateTime = _nowTime
                    }).ToList();

                if (!_updateMatlSetting.Any())
                    return _updRes;

                using (TransactionScope scope = new TransactionScope())
                {
                    bool tmpRes = false;
                    tmpRes = _sapMaterialRepository.UpdateMatlSetting(_updateMatlSetting) == _updateMatlSetting.Count;
                    tmpRes = _sapMaterialRepository.InsertMatlSettingHistory(_updateMatlHis) == _updateMatlHis.Count;

                    if (tmpRes)
                        scope.Complete();
                    else
                        _updRes = "更新異常";
                }


                return _updRes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
                    List<SAPWorkOrderDao> _insSAPWorkOrderList = new List<SAPWorkOrderDao>();
                    MTDScheduleUpdateHistoryDao _mtdScheduleUpdateHistoryDao = new MTDScheduleUpdateHistoryDao();
                    SAPWorkOrderDao _tmpDao = new SAPWorkOrderDao();
                    List<SAPWorkOrderDao> _updSAPwoList = new List<SAPWorkOrderDao>();
                    List<SAPWorkOrderDao> _insSAPwoList = new List<SAPWorkOrderDao>();
                    DateTime _nowTime = DateTime.Now;

                    string[] _fileNameAry = uploadFile.FileName.Split(".");
                    var fileName = Path.GetFileName($"{_fileNameAry[0]}_{_nowTime.ToString("yyMMddHHmmss")}.{_fileNameAry[1]}");

                    // 先撈取 report 4.18 工單資料
                    var _woTask = GetZipsumReport418Async(_nowTime.ToString("yyyy/MM/dd"));
                    List<MaterialSettingDao> _matlAllSetting = _sapMaterialRepository.SelectMatlAllSetting();
                    List<MaterialSettingDao> _matlSettingCode5 = _matlAllSetting.Where(w => w.CodeTypeId == MatlCodeTypeEnum.Code5).ToList();
                    List<MaterialSettingDao> _matlSettingCode13 = _matlAllSetting.Where(w => w.CodeTypeId == MatlCodeTypeEnum.Code13).ToList();
                    List<SAPWorkOrderDao> _oldSAPwoList = _sapMaterialRepository.SelectSAPwoByConditions();

                    // 刪除暫存計算檔
                    string[] _dirAllFiles = Directory.GetFiles("..\\tempFileProcess\\");
                    if (_dirAllFiles.Length != 0)
                        foreach (string filePath in _dirAllFiles)
                            File.Delete(filePath);


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
                    XSSFSheet _sapDataSheet = (XSSFSheet)workbook.GetSheetAt(0);

                    // 表頭列新增"理論撥料"、"修正數"
                    XSSFRow row = (XSSFRow)_sapDataSheet.GetRow(0);

                    // font red
                    //XSSFCellStyle _headCellStyle = (XSSFCellStyle)workbook.CreateCellStyle();
                    //_headCellStyle.SetFillForegroundColor(_bgColorDic["sapH"]);
                    ICellStyle _headCellStyle = workbook.CreateCellStyle();
                    _headCellStyle.FillPattern = FillPattern.SolidForeground;
                    _headCellStyle.FillForegroundColor = IndexedColors.SeaGreen.Index;

                    //XSSFCellStyle _calculateCellStyle = (XSSFCellStyle)workbook.CreateCellStyle();
                    //_calculateCellStyle.FillBackgroundXSSFColor =IndexedColors.OliveGreen.;
                    ICellStyle _calculateCellStyle = workbook.CreateCellStyle();
                    _calculateCellStyle.FillPattern = FillPattern.SolidForeground;
                    _calculateCellStyle.FillForegroundColor = IndexedColors.Coral.Index;

                    ICell _theortOut = row.CreateCell(19);
                    ICell _diffOut = row.CreateCell(20);
                    ICell _woPremiumOut = row.CreateCell(21);
                    ICell _cantNeg = row.CreateCell(22);
                    ICell _shortName = row.CreateCell(23);
                    ICell _opiStatus = row.CreateCell(24);
                    ICell _woType = row.CreateCell(25);
                    ICell _useNode = row.CreateCell(26);
                    ICell _comment = row.CreateCell(27);
                    ICell _mesScrap = row.CreateCell(28);

                    _theortOut.SetCellValue("理論撥料");
                    _theortOut.CellStyle = _headCellStyle;
                    _diffOut.SetCellValue("修正數");
                    _diffOut.CellStyle = _headCellStyle;
                    _woPremiumOut.SetCellValue("工單短溢領數");
                    _woPremiumOut.CellStyle = _calculateCellStyle;
                    _cantNeg.SetCellValue("不得為負");
                    _cantNeg.CellStyle = _calculateCellStyle;
                    _shortName.SetCellValue("部材簡稱");
                    _shortName.CellStyle = _calculateCellStyle;
                    _opiStatus.SetCellValue("OPI工單狀態");
                    _opiStatus.CellStyle = _calculateCellStyle;
                    _woType.SetCellValue("工單型態");
                    _woType.CellStyle = _calculateCellStyle;
                    _useNode.SetCellValue("使用站點");
                    _useNode.CellStyle = _calculateCellStyle;
                    _comment.SetCellValue("Comment");
                    _comment.CellStyle = _calculateCellStyle;
                    _mesScrap.SetCellValue("MES報廢數");
                    _mesScrap.CellStyle = _calculateCellStyle;

                    ICellStyle _cellStyle = workbook.CreateCellStyle();
                    IFont _font = workbook.CreateFont();
                    _font.Color = IndexedColors.Red.Index;
                    _cellStyle.SetFont(_font);

                    //_headCellStyle.FillBackgroundXSSFColor = _bgColorDic["calcContent"];
                    //_calculateCellStyle.FillBackgroundXSSFColor = _bgColorDic["rptContent"];

                    _headCellStyle = workbook.CreateCellStyle();
                    _headCellStyle.FillPattern = FillPattern.SolidForeground;
                    _headCellStyle.FillForegroundColor = IndexedColors.LightGreen.Index;
                    _calculateCellStyle = workbook.CreateCellStyle();
                    _calculateCellStyle.FillPattern = FillPattern.SolidForeground;
                    _calculateCellStyle.FillForegroundColor = IndexedColors.LightYellow.Index;

                    List<WorkOrderEntity> _workOrderEntity = _woTask.Result;

                    // for迴圈的「啟始值」要加一，表示不包含 Excel表頭列
                    for (int i = (_sapDataSheet.FirstRowNum + 1); i <= _sapDataSheet.LastRowNum; i++)
                    {
                        // 每一列做迴圈
                        row = (XSSFRow)_sapDataSheet.GetRow(i); // 不包含 Excel表頭列

                        _tmpDao = new SAPWorkOrderDao()
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
                        };

                        var _currentSetting = _matlSettingCode13.FirstOrDefault(f => f.MatlNo == _tmpDao.MaterialNo)
                                ?? _matlSettingCode5.FirstOrDefault(f => f.MatlNo == _tmpDao.MaterialNo.Substring(0, 5)) ?? null;
                        var _currZipWOStatus = _workOrderEntity.FirstOrDefault(f => f.WorkOrder == _tmpDao.Order);

                        #region 計算"理論撥料"、"修正數"
                        var _culQty = Convert.ToInt32(row.GetCell(12).NumericCellValue) * ((_currentSetting?.LossRate ?? 0) + 1);
                        var _diffCulQty = Convert.ToInt32(row.GetCell(13).NumericCellValue) - Math.Round(_culQty) - _tmpDao.ReturnQty - _tmpDao.ScrapQty;

                        _theortOut = row.CreateCell(19);
                        _theortOut.SetCellType(CellType.Numeric);
                        _theortOut.SetCellValue(Convert.ToDouble(_culQty));
                        _theortOut.CellStyle = _headCellStyle;

                        _diffOut = row.CreateCell(20);
                        _diffOut.SetCellType(CellType.String);
                        _diffOut.SetCellValue(Convert.ToDouble(_diffCulQty));
                        _diffOut.CellStyle = _headCellStyle;

                        // 修正數 < 0 上紅色
                        if (_diffCulQty < 0)
                        {
                            _diffOut.CellStyle = _cellStyle;
                        }

                        _tmpDao.OverDisburse = _culQty;
                        _tmpDao.DiffDisburse = _diffCulQty;

                        #endregion

                        #region 其他欄位計算

                        _tmpDao.WOPremiumOut = _tmpDao.DisburseQty - _tmpDao.ReturnQty - _tmpDao.ScrapQty - _tmpDao.ExptQty;
                        _woPremiumOut = row.CreateCell(21);
                        _woPremiumOut.SetCellType(CellType.Numeric);
                        _woPremiumOut.SetCellValue(Convert.ToDouble(_tmpDao.WOPremiumOut));
                        _woPremiumOut.CellStyle = _calculateCellStyle;

                        _tmpDao.CantNegative = _tmpDao.DisburseQty - _tmpDao.ReturnQty - _tmpDao.ScrapQty;
                        _cantNeg = row.CreateCell(22);
                        _cantNeg.SetCellType(CellType.Numeric);
                        _cantNeg.SetCellValue(Convert.ToDouble(_tmpDao.CantNegative));
                        _cantNeg.CellStyle = _calculateCellStyle;

                        _shortName = row.CreateCell(23);
                        _shortName.SetCellType(CellType.String);
                        _shortName.SetCellValue(_currentSetting?.MatlName ?? "");
                        _shortName.CellStyle = _calculateCellStyle;

                        _tmpDao.OPIwoStatus = _currZipWOStatus.WOStatus == "complete" ? "complete"
                            : _currZipWOStatus.WOStatus == "release" && _currZipWOStatus.ActualQty > 0 ? "已下線" : "未下線";
                        _opiStatus = row.CreateCell(24);
                        _opiStatus.SetCellType(CellType.String);
                        _opiStatus.SetCellValue(_tmpDao.OPIwoStatus);
                        _opiStatus.CellStyle = _calculateCellStyle;

                        _tmpDao.WOType = _currZipWOStatus.WOType;
                        _woType = row.CreateCell(25);
                        _woType.SetCellType(CellType.String);
                        _woType.SetCellValue(_currZipWOStatus.WOType);
                        _woType.CellStyle = _calculateCellStyle;

                        _useNode = row.CreateCell(26);
                        _useNode.SetCellType(CellType.String);
                        _useNode.SetCellValue(_currentSetting?.UseNode ?? "");
                        _useNode.CellStyle = _calculateCellStyle;

                        #endregion

                        _insSAPWorkOrderList.Add(_tmpDao);
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

                    if (_oldSAPwoList.Any())
                    {
                        _updSAPwoList = (from oldSAP in _oldSAPwoList
                                         join newSAP in _insSAPWorkOrderList
                                         on new { oldSAP.Order, oldSAP.Prod, oldSAP.MaterialNo } equals new { newSAP.Order, newSAP.Prod, newSAP.MaterialNo }
                                         select new SAPWorkOrderDao
                                         {
                                             Sn = oldSAP.Sn,
                                             Order = oldSAP.Order,
                                             Prod = oldSAP.Prod,
                                             MaterialSpec = newSAP.MaterialSpec,
                                             MaterialNo = oldSAP.MaterialNo,
                                             MaterialName = newSAP.MaterialName,
                                             SAPNode = newSAP.SAPNode,
                                             Dept = newSAP.Dept,
                                             ProdQty = newSAP.ProdQty,
                                             StorageQty = newSAP.StorageQty,
                                             StartDate = newSAP.StartDate,
                                             FinishDate = newSAP.FinishDate,
                                             Unit = newSAP.Unit,
                                             ExptQty = newSAP.ExptQty,
                                             DisburseQty = newSAP.DisburseQty,
                                             ReturnQty = newSAP.ReturnQty,
                                             ActStorageQty = newSAP.ActStorageQty,
                                             ScrapQty = newSAP.ScrapQty,
                                             DiffQty = newSAP.DiffQty,
                                             DiffRate = newSAP.DiffRate,
                                             OverDisburse = newSAP.OverDisburse,
                                             DiffDisburse = newSAP.DiffDisburse,
                                             WOPremiumOut = newSAP.WOPremiumOut,
                                             CantNegative = newSAP.CantNegative,
                                             OPIwoStatus = newSAP.OPIwoStatus,
                                             WOType = newSAP.WOType
                                         }).ToList();

                        _insSAPwoList = _insSAPWorkOrderList.Where(sap => _oldSAPwoList.All(a => !(a.Order == sap.Order && a.Prod == sap.Prod && a.MaterialNo == sap.MaterialNo))).ToList();
                    }
                    else
                    {
                        _insSAPwoList = _insSAPWorkOrderList;
                    }


                    using (TransactionScope scope = new TransactionScope())
                    {
                        _sapMaterialRepository.InsertSAPwo(_insSAPwoList);
                        if (_updSAPwoList.Any())
                            _sapMaterialRepository.UpdateSAPwo(_updSAPwoList);

                        scope.Complete();
                    }

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

        /// <summary>
        /// 5碼、13碼 料號耗損設定檔
        /// </summary>
        /// <param name="uploadFile">上傳檔案</param>
        /// <param name="codeFolder">存放上傳檔資料夾</param>
        /// <param name="userEntity">使用者資訊</param>
        /// <returns></returns>
        public string UploadCodeRate(IFormFile uploadFile, MatlCodeTypeEnum codeTypeId, UserEntity userEntity)
        {
            try
            {
                string _uplRes = "";

                if (uploadFile.Length > 0)   // 檢查 <input type="file"> 是否輸入檔案？
                {
                    XSSFWorkbook workbook;
                    List<SAPWorkOrderDao> _insSAPWorkOrderList = new List<SAPWorkOrderDao>();
                    MTDScheduleUpdateHistoryDao _mtdScheduleUpdateHistoryDao = new MTDScheduleUpdateHistoryDao();
                    MaterialSettingDao _tmpDao = new MaterialSettingDao();
                    List<SAPWorkOrderDao> _updSAPwoList = new List<SAPWorkOrderDao>();
                    List<MaterialSettingDao> _insMatlSettingList = new List<MaterialSettingDao>();
                    DateTime _nowTime = DateTime.Now;

                    string[] _fileNameAry = uploadFile.FileName.Split(".");
                    var fileName = Path.GetFileName($"{_fileNameAry[0]}_{_nowTime.ToString("yyMMddHHmmss")}.{_fileNameAry[1]}");

                    using (var ms = new MemoryStream())
                    {
                        uploadFile.CopyTo(ms);

                        // NOPI 讀取檔案內容（Stream串流）
                        Stream stream = new MemoryStream(ms.ToArray());

                        workbook = new XSSFWorkbook(stream); // 將剛剛的Excel (Stream）讀取到工作表裡面
                        // XSSFWorkbook() 只能讀取 System.IO.Stream
                    }

                    #region
                    XSSFSheet _sapDataSheet = (XSSFSheet)workbook.GetSheetAt(0);

                    // 表頭列新增"理論撥料"、"修正數"
                    XSSFRow row = (XSSFRow)_sapDataSheet.GetRow(0);

                    // 檢核 code type 和資料內容
                    if (codeTypeId == MatlCodeTypeEnum.Code5 && ((XSSFRow)_sapDataSheet.GetRow(1)).GetCell(0).StringCellValue.Length > 6 ||
                        codeTypeId == MatlCodeTypeEnum.Code13 && ((XSSFRow)_sapDataSheet.GetRow(1)).GetCell(0).StringCellValue.Length != 13)
                        throw new Exception("請確認選取料號長度與上傳檔是否相符");

                    // for迴圈的「啟始值」要加一，表示不包含 Excel表頭列
                    for (int i = (_sapDataSheet.FirstRowNum + 1); i <= _sapDataSheet.LastRowNum; i++)
                    {
                        // 每一列做迴圈
                        row = (XSSFRow)_sapDataSheet.GetRow(i); // 不包含 Excel表頭列

                        if (row.GetCell(0) == null)
                            break;

                        _tmpDao = new MaterialSettingDao()
                        {
                            MatlNo = row.GetCell(0).StringCellValue,
                            CodeTypeId = codeTypeId,
                            MatlName = row.GetCell(1).StringCellValue,
                            MatlCatg = row.GetCell(2).StringCellValue,
                            UseNode = row.GetCell(3).StringCellValue,
                            LossRate = Convert.ToDecimal(row.GetCell(4).NumericCellValue),
                            UpdateUser = userEntity.Name,
                            UpdateTime = _nowTime
                        };

                        _insMatlSettingList.Add(_tmpDao);
                    }
                    #endregion

                    // FTP 上傳
                    _ftpService.FTP_Upload(uploadFile, $"FTP_SAP{_codeTypeFolderDic[codeTypeId]}", fileName, false, $"..\\{fileName}");

                    using (TransactionScope scope = new TransactionScope())
                    {
                        bool updRes = false;

                        _sapMaterialRepository.DeleteMatlSetting(codeTypeId);
                        updRes = _sapMaterialRepository.InsertMatlSetting(_insMatlSettingList) == _insMatlSettingList.Count;

                        if (updRes)
                            scope.Complete();
                        else
                            _uplRes = "上傳異常";
                    }

                    return _uplRes;
                }
                else
                    return _uplRes;
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

        public (bool, string, string) MatlSettingDownload(MatlCodeTypeEnum codeTypeId)
        {
            try
            {
                List<MaterialSettingDao> _matlSetting = _sapMaterialRepository.SelectMatlAllSetting(codeTypeId);

                // 刪除暫存計算檔
                string[] _dirAllFiles = Directory.GetFiles("..\\tempDownProcess\\");
                if (_dirAllFiles.Length != 0)
                    foreach (string filePath in _dirAllFiles)
                        File.Delete(filePath);

                // 新增暫存計算檔
                using (FileStream fs = new FileStream($"..\\tempDownProcess\\{codeTypeId.GetDescription()}.xlsx", FileMode.Create))
                {
                }

                XSSFWorkbook _workbook = new XSSFWorkbook();
                ISheet _sheet = _workbook.CreateSheet("Sheet1");
                IRow _hRow = _sheet.CreateRow(0);
                ICell _mNumber = _hRow.CreateCell(0);
                _mNumber.SetCellValue("料號");
                ICell _name = _hRow.CreateCell(1);
                _name.SetCellValue("品名");
                ICell _desc = _hRow.CreateCell(2);
                _desc.SetCellValue("料號分類說明");
                ICell _useNode = _hRow.CreateCell(3);
                _useNode.SetCellValue("使用站點");
                ICell _rate = _hRow.CreateCell(4);
                _rate.SetCellValue("耗損率");

                for (int i = 1; i < _matlSetting.Count; i++)
                {
                    _hRow = _sheet.CreateRow(i);
                    _mNumber = _hRow.CreateCell(0);
                    _mNumber.SetCellValue(_matlSetting[i].MatlNo);
                    _name = _hRow.CreateCell(1);
                    _name.SetCellValue(_matlSetting[i].MatlName);
                    _desc = _hRow.CreateCell(2);
                    _desc.SetCellValue(_matlSetting[i].MatlCatg);
                    _useNode = _hRow.CreateCell(3);
                    _useNode.SetCellValue(_matlSetting[i].UseNode);
                    _rate = _hRow.CreateCell(4);
                    _rate.SetCellValue(Convert.ToDouble(_matlSetting[i].LossRate));
                }

                // 回寫本地計算檔
                using (FileStream fs = new FileStream($"..\\tempDownProcess\\{codeTypeId.GetDescription()}.xlsx", FileMode.Open, FileAccess.Write))
                {
                    _workbook.Write(fs);
                    fs.Close();
                }

                return (true, $"..\\tempDownProcess\\{codeTypeId.GetDescription()}.xlsx", $"{codeTypeId.GetDescription()}.xlsx");

                //var _fileName = _ftpService.GetFTPLatestFile($"FTP_SAP{_codeTypeFolderDic[codeTypeId]}");

                //if (_fileName == "")
                //    return (false, "", "無上傳檔案");

                //var _fileStr = _ftpService.FTP_Download($"FTP_SAP{_codeTypeFolderDic[codeTypeId]}", _fileName);

                //return (true, _fileStr, _fileName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<List<WorkOrderEntity>> GetZipsumReport418Async(string dateStr)
        {

            string _qStr = $"wo_area=&wotype=ALL&iswodueday=N&Calendar1={dateStr}&calendar2={dateStr}&iswostartday=N&Calendar2_1={dateStr}&calendar2_2={dateStr}&workorder=&prod_type=ALL&wo_status=ALL&mvin_rate=ALL&mvou_rate=ALL&G_FAC=6&Shop=MOD4&calendar_1={dateStr}&calendar_2={dateStr}&calendar_2_1={dateStr}&calendar_2_2={dateStr}";
            var data = new StringContent(_qStr, Encoding.UTF8, "application/x-www-form-urlencodedn");

            List<string> _status;
            List<WorkOrderEntity> _wo418Entity = new List<WorkOrderEntity>();

            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsync("http://zipsum/modreport/Report/MOD4/NHWoUnClose60DataSet.asp", data);

                response.Content.Headers.ContentType.CharSet = "Big5";

                string result = response.Content.ReadAsStringAsync().Result;

                result = result.Remove(0, 32900);

                _status = result.Split("<SCRIPT LANGUAGE=vbscript>").ToList();
            }

            _status.RemoveAt(_status.Count - 1);

            _status.ForEach(wo =>
            {
                var _tmp = wo.Split("ReportGrid1.TextMatrix");
                _wo418Entity.Add(new WorkOrderEntity
                {
                    WorkOrder = _tmp[1].Split("\"")[1],
                    WOStatus = _tmp[2].Split("\"")[1],
                    LcmProduct = _tmp[3].Split("\"")[1],
                    WOType = _woTypeDic[_tmp[5].Split("\"")[1]],
                    ActualQty = Convert.ToInt32(_tmp[16].Split("\"")[1])
                });
            });

            return _wo418Entity;
        }
    }
}