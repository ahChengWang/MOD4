using Microsoft.AspNetCore.Http;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using MOD4.Web.Repostory;
using MOD4.Web.Repostory.Dao;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        #region SAP 工單

        public List<SAPWorkOrderEntity> GetSAPDropDownList()
        {
            try
            {
                var _sapWOCatch = CatchHelper.Get($"sapDropdown");
                List<SAPWorkOrderDao> _sapWOList = new List<SAPWorkOrderDao>();

                if (_sapWOCatch == null)
                {
                    _sapWOList = _sapMaterialRepository.SelectSAPwoByConditions();
                    CatchHelper.Set("sapDropdown", JsonConvert.SerializeObject(_sapWOList.Select(s => new
                    {
                        s.Order,
                        s.Prod,
                        s.MaterialNo,
                        s.SAPNode
                    })), 432000);
                }
                else
                    _sapWOList = JsonConvert.DeserializeObject<List<SAPWorkOrderDao>>(_sapWOCatch);


                return _sapWOList.CopyAToB<SAPWorkOrderEntity>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SAPWorkOrderEntity> GetSAPWorkOredr(string workOrder, string prodNo, string sapNode, string matrlNo)
        {
            try
            {
                List<SAPWorkOrderDao> _sapWOList = _sapMaterialRepository.SelectSAPwoByConditions(
                    workOrder: workOrder?.Split(",").ToList() ?? null,
                    prodNo: prodNo,
                    sapNode: sapNode?.Split(",").Select(s => s == "0" ? "" : s).ToList() ?? null,
                    matrlNo: matrlNo?.Split(",").ToList() ?? null);

                return _sapWOList.CopyAToB<SAPWorkOrderEntity>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public (bool, string, string) GetSAPwoDisburseDownload(string workOrder, string prodNo, string sapNode, string matrlNo)
        {
            try
            {
                XSSFWorkbook workbook;
                Dictionary<string, Dictionary<string, double>> _stockDic = new Dictionary<string, Dictionary<string, double>>();

                var _stockQueryTask = Task.Run(() =>
                {
                    Parallel.ForEach(new List<string> { "3100", "3200", "3201", "410H", "420H" },
                        new ParallelOptions { MaxDegreeOfParallelism = 5 },
                        (sloc) =>
                        {
                            var _stockTask = _inxReportService.GetSTOCKReportAsync<INXStockRptEntity>(sloc);

                            lock (this)
                                _stockDic.Add(sloc, _stockTask.Result.lists.GroupBy(b => b.MATERIAL).Select(s => new
                                {
                                    MATERIAL = s.Key,
                                    Detail = s.ToList()
                                }).ToDictionary(dic => dic.MATERIAL, dic => dic.Detail.Sum(sum => Convert.ToDouble(sum.QTY))));
                        });
                });

                List<SAPWorkOrderDao> _sapWOList = _sapMaterialRepository.SelectSAPwoByConditions(
                    workOrder: workOrder?.Split(",").ToList() ?? null,
                    prodNo: prodNo,
                    sapNode: sapNode?.Split(",").Select(s => s == "0" ? "" : s).ToList() ?? null,
                    matrlNo: matrlNo?.Split(",").ToList() ?? null).ToList();

                // 刪除暫存計算檔
                string[] _dirAllFiles = Directory.GetFiles("..\\tempDownSAPClose\\");
                if (_dirAllFiles.Length != 0)
                    foreach (string filePath in _dirAllFiles)
                        File.Delete(filePath);

                string _fileName = $"工單撥料_{DateTime.Now.ToString("yyMMdd")}";

                workbook = new XSSFWorkbook();
                _stockQueryTask.Wait();

                //File.Copy($"..\\template\\工單強結認列_sample.xlsx", $"..\\tempDownSAPClose\\{_fileName}.xlsx");

                if (_sapWOList.Any())
                {
                    ISheet _sheet = workbook.CreateSheet($"Sheet");
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

                    ICellStyle _cellStyleSpcl = workbook.CreateCellStyle();
                    IFont _fontSpcl = workbook.CreateFont();
                    _fontSpcl.Color = IndexedColors.Red.Index;
                    _fontSpcl.FontName = "新細明體";
                    _cellStyleSpcl.SetFont(_fontSpcl);
                    _cellStyleSpcl.VerticalAlignment = VerticalAlignment.Center;
                    _cellStyleSpcl.Alignment = HorizontalAlignment.Center;
                    _cellStyleSpcl.BorderTop = BorderStyle.Thin;
                    _cellStyleSpcl.BorderRight = BorderStyle.Thin;
                    _cellStyleSpcl.BorderBottom = BorderStyle.Thin;
                    _cellStyleSpcl.BorderLeft = BorderStyle.Thin;

                    _row = _sheet.CreateRow(0);
                    _cell = _row.CreateCell(0);
                    _cell.SetCellValue("Order");
                    _cell.CellStyle = _cellStyle;
                    _cell = _row.CreateCell(1);
                    _cell.SetCellValue("機種");
                    _cell.CellStyle = _cellStyle;
                    _cell = _row.CreateCell(2);
                    _cell.SetCellValue("料號");
                    _cell.CellStyle = _cellStyle;
                    _cell = _row.CreateCell(3);
                    _cell.SetCellValue("應發數量");
                    _cell.CellStyle = _cellStyle;
                    _cell = _row.CreateCell(4);
                    _cell.SetCellValue("發料數量");
                    _cell.CellStyle = _cellStyle;
                    _cell = _row.CreateCell(5);
                    _cell.SetCellValue("退料數量");
                    _cell.CellStyle = _cellStyle;
                    _cell = _row.CreateCell(6);
                    _cell.SetCellValue("工單短溢領數");
                    _cell.CellStyle = _cellStyle;
                    _cell = _row.CreateCell(7);
                    _cell.SetCellValue("3100庫存量");
                    _cell.CellStyle = _cellStyle;
                    _cell = _row.CreateCell(8);
                    _cell.SetCellValue("3200庫存量");
                    _cell.CellStyle = _cellStyle;
                    _cell = _row.CreateCell(9);
                    _cell.SetCellValue("3201庫存量");
                    _cell.CellStyle = _cellStyle;
                    _cell = _row.CreateCell(10);
                    _cell.SetCellValue("410H庫存量");
                    _cell.CellStyle = _cellStyle;
                    _cell = _row.CreateCell(11);
                    _cell.SetCellValue("420H庫存量");
                    _cell.CellStyle = _cellStyle;

                    for (int r = 0; r < _sapWOList.Count; r++)
                    {
                        _row = _sheet.CreateRow(r + 1);
                        _cell = _row.CreateCell(0);
                        _cell.SetCellValue(_sapWOList[r].Order);
                        _cell.CellStyle = _cellStyle;
                        _cell = _row.CreateCell(1);
                        _cell.SetCellValue(_sapWOList[r].Prod);
                        _cell.CellStyle = _cellStyle;
                        _cell = _row.CreateCell(2);
                        _cell.SetCellValue(_sapWOList[r].MaterialNo);
                        _cell.CellStyle = _cellStyle;
                        _cell = _row.CreateCell(3);
                        _cell.SetCellValue(Convert.ToDouble(_sapWOList[r].ExptQty));
                        _cell.CellStyle = _cellStyle;
                        _cell = _row.CreateCell(4);
                        _cell.SetCellValue(Convert.ToDouble(_sapWOList[r].DisburseQty));
                        _cell.CellStyle = _cellStyle;
                        _cell = _row.CreateCell(5);
                        _cell.SetCellValue(Convert.ToDouble(_sapWOList[r].ReturnQty));
                        _cell.CellStyle = _cellStyle;
                        _cell = _row.CreateCell(6);
                        _cell.SetCellValue(Convert.ToDouble(_sapWOList[r].WOPremiumOut));
                        if (_sapWOList[r].WOPremiumOut < 0)
                            _cell.CellStyle = _cellStyleSpcl;
                        else
                            _cell.CellStyle = _cellStyle;

                        _cell = _row.CreateCell(7);
                        _cell.SetCellValue(_stockDic["3100"].ContainsKey(_sapWOList[r].MaterialNo) ? _stockDic["3100"][_sapWOList[r].MaterialNo] : 0);
                        _cell.CellStyle = _cellStyle;
                        _cell = _row.CreateCell(8);
                        _cell.SetCellValue(_stockDic["3200"].ContainsKey(_sapWOList[r].MaterialNo) ? _stockDic["3200"][_sapWOList[r].MaterialNo] : 0);
                        _cell.CellStyle = _cellStyle;
                        _cell = _row.CreateCell(9);
                        _cell.SetCellValue(_stockDic["3201"].ContainsKey(_sapWOList[r].MaterialNo) ? _stockDic["3201"][_sapWOList[r].MaterialNo] : 0);
                        _cell.CellStyle = _cellStyle;
                        _cell = _row.CreateCell(10);
                        _cell.SetCellValue(_stockDic["410H"].ContainsKey(_sapWOList[r].MaterialNo) ? _stockDic["410H"][_sapWOList[r].MaterialNo] : 0);
                        _cell.CellStyle = _cellStyle;
                        _cell = _row.CreateCell(11);
                        _cell.SetCellValue(_stockDic["420H"].ContainsKey(_sapWOList[r].MaterialNo) ? _stockDic["420H"][_sapWOList[r].MaterialNo] : 0);
                        _cell.CellStyle = _cellStyle;
                    }
                }

                // 回寫本地計算檔
                using (FileStream fs = new FileStream($"..\\tempDownSAPClose\\{_fileName}.xlsx", FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(fs);
                    fs.Close();
                }

                return (true, $"..\\tempDownSAPClose\\{_fileName}.xlsx", $"{_fileName}.xlsx");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public (bool, string, string) GetSAPwoCloseDownload(string workOrder, string prodNo, string sapNode, string matrlNo)
        {
            try
            {
                XSSFWorkbook workbook;

                List<SAPWorkOrderDao> _sapWOList = _sapMaterialRepository.SelectSAPwoByConditions(
                    workOrder: workOrder?.Split(",").ToList() ?? null,
                    prodNo: prodNo,
                    sapNode: sapNode?.Split(",").Select(s => s == "0" ? "" : s).ToList() ?? null,
                    matrlNo: matrlNo?.Split(",").ToList() ?? null).Where(wo => wo.DiffQty != 0).ToList();

                // 刪除暫存計算檔
                string[] _dirAllFiles = Directory.GetFiles("..\\tempDownSAPClose\\");
                if (_dirAllFiles.Length != 0)
                    foreach (string filePath in _dirAllFiles)
                        File.Delete(filePath);

                string _fileName = $"工單強結認列_{DateTime.Now.ToString("MMdd")}";

                File.Copy($"..\\template\\工單強結認列_sample.xlsx", $"..\\tempDownSAPClose\\{_fileName}.xlsx");

                // 新增暫存計算檔
                using (FileStream fs = new FileStream($"..\\tempDownSAPClose\\{_fileName}.xlsx", FileMode.Open, FileAccess.Read))
                {
                    workbook = new XSSFWorkbook(fs); // 將剛剛的Excel (Stream）讀取到工作表裡面
                }

                if (_sapWOList.Any())
                {
                    float _tllCont = _sapWOList.Count();

                    if (Math.Ceiling(_tllCont / 25) > 20)
                        throw new Exception("總比數 > 500, 請減少單次匯出輸出筆數");

                    for (int i = 0; i < Math.Ceiling(_tllCont / 25); i++)
                    {
                        List<SAPWorkOrderDao> _exportWOList = _sapWOList.OrderBy(no => no.Order).ThenBy(o => o.MaterialNo).Skip(25 * i).Take(25).ToList();

                        ISheet _sheet = workbook.GetSheet($"Sheet{i + 1}");
                        IRow _row;
                        ICell _cell;
                        ICellStyle _cellSpecStyle = workbook.CreateCellStyle();
                        IFont _fontSpec = workbook.CreateFont();
                        _fontSpec.Color = IndexedColors.Red.Index;
                        _fontSpec.FontName = "新細明體";
                        _cellSpecStyle.SetFont(_fontSpec);
                        _cellSpecStyle.VerticalAlignment = VerticalAlignment.Center;
                        _cellSpecStyle.Alignment = HorizontalAlignment.Center;
                        _cellSpecStyle.BorderTop = BorderStyle.Thin;
                        _cellSpecStyle.BorderRight = BorderStyle.Thin;
                        _cellSpecStyle.BorderBottom = BorderStyle.Thin;
                        _cellSpecStyle.BorderLeft = BorderStyle.Thin;

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

                        _row = _sheet.GetRow(1);
                        _cell = _row.GetCell(18);
                        _cell.SetCellValue(DateTime.Now.ToString("yyyy/M/d HH:mm:ss"));

                        for (int r = 0; r < _exportWOList.Count; r++)
                        {
                            _row = _sheet.GetRow(r + 4);
                            _cell = _row.GetCell(0);
                            _cell.SetCellValue(_exportWOList[r].Order);
                            _cell.CellStyle = _cellStyle;
                            _cell = _row.GetCell(1);
                            _cell.SetCellValue(_exportWOList[r].Prod);
                            _cell.CellStyle = _cellStyle;
                            _cell = _row.GetCell(2);
                            _cell.SetCellValue(_exportWOList[r].StartDate);
                            //_cell.CellStyle = _cellStyle;
                            _cell = _row.GetCell(3);
                            _cell.SetCellValue(_exportWOList[r].FinishDate);
                            //_cell.CellStyle = _cellStyle;
                            _cell = _row.GetCell(4);
                            _cell.SetCellValue(Convert.ToDouble(_exportWOList[r].ScrapQty));
                            _cell.CellStyle = _cellStyle;
                            _cell = _row.GetCell(5);
                            _cell.SetCellValue(_exportWOList[r].MatlShortName);
                            _cell.CellStyle = _cellStyle;
                            _cell = _row.GetCell(6);
                            _cell.SetCellValue(_exportWOList[r].MaterialNo);
                            _cell.CellStyle = _cellStyle;
                            if (_exportWOList[r].DiffQty > 0)
                            {
                                _cell = _row.GetCell(7);
                                _cell.SetCellValue("超撥");
                                _cell.CellStyle = _cellSpecStyle;
                                _cell = _row.GetCell(9);
                                _cell.SetCellValue(Convert.ToDouble(_exportWOList[r].DiffQty));
                                _cell.CellStyle = _cellStyle;
                            }
                            else
                            {
                                _cell = _row.GetCell(7);
                                _cell.SetCellValue("欠撥");
                                _cell.CellStyle = _cellStyle;
                                _cell = _row.GetCell(9);
                                _cell.SetCellValue(Convert.ToDouble(_exportWOList[r].DiffQty));
                                _cell.CellStyle = _cellSpecStyle;
                            }
                        }
                    }
                }

                // 回寫本地計算檔
                using (FileStream fs = new FileStream($"..\\tempDownSAPClose\\{_fileName}.xlsx", FileMode.Create))
                {
                    workbook.Write(fs);
                }

                return (true, $"..\\tempDownSAPClose\\{_fileName}.xlsx", $"{_fileName}.xlsx");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Material Setting

        /// <summary>
        /// SAP 撥料檔上傳流程：本地新增計算檔 -> 上傳檔計算 -> 寫入計算檔 -> 上傳 FTP -> 觸發下載計算檔
        /// </summary>
        /// <param name="uploadFile">上傳檔案</param>
        /// <param name="userEntity">使用者資訊</param>
        /// <returns></returns>
        public (bool, string, string) UploadAndCalculate(IFormFile uploadFile, UserEntity userEntity)
        {
            try
            {
                if ((uploadFile?.Length ?? 0) > 0)   // 檢查 <input type="file"> 是否輸入檔案？
                {
                    XSSFWorkbook workbook;
                    List<SAPWorkOrderDao> _insSAPWorkOrderList = new List<SAPWorkOrderDao>();
                    SAPWorkOrderDao _tmpDao = new SAPWorkOrderDao();
                    List<SAPWorkOrderDao> _updSAPwoList = new List<SAPWorkOrderDao>();
                    List<SAPWorkOrderDao> _insSAPwoList = new List<SAPWorkOrderDao>();
                    DateTime _nowTime = DateTime.Now;

                    string[] _fileNameAry = uploadFile.FileName.Split(".");
                    var fileName = Path.GetFileName($"{_fileNameAry[0]}_{_nowTime.ToString("yyMMddHHmmss")}.{_fileNameAry[1]}");

                    // 先撈取 report 4.18 工單資料
                    var _woTask = GetZipsumReport418Async(_nowTime);

                    List<DefinitionMaterialDao> _defMaterial = _sapMaterialRepository.SelectAllMatlDef();
                    List<MaterialSettingDao> _matlAllSetting = _sapMaterialRepository.SelectMatlAllSetting(null);

                    List<MaterialSettingEntity> _matlSettingCode5 = (from matl in _matlAllSetting
                                                                     join def in _defMaterial
                                                                     on matl.MatlSn equals def.Sn
                                                                     where def.CodeTypeId == MatlCodeTypeEnum.Code5
                                                                     select new MaterialSettingEntity
                                                                     {
                                                                         MatlNo = def.MatlNo,
                                                                         MatlName = def.MatlName,
                                                                         UseNode = def.UseNode,
                                                                         LossRate = matl.LossRate
                                                                     }).ToList();

                    List<MaterialSettingEntity> _matlSettingCode13 = (from matl in _matlAllSetting
                                                                      join def in _defMaterial
                                                                      on matl.MatlSn equals def.Sn
                                                                      where def.CodeTypeId == MatlCodeTypeEnum.Code13
                                                                      select new MaterialSettingEntity
                                                                      {
                                                                          MatlNo = def.MatlNo,
                                                                          MatlName = def.MatlName,
                                                                          UseNode = def.UseNode,
                                                                          LossRate = matl.LossRate
                                                                      }).ToList();

                    //List<SAPWorkOrderDao> _oldSAPwoList = _sapMaterialRepository.SelectSAPwoByConditions();

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
                    ICell _icScrap = row.CreateCell(29);

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
                    _icScrap.SetCellValue("IC 總顆數");
                    _icScrap.CellStyle = _calculateCellStyle;

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
                            DiffRate = Convert.ToDecimal(row.GetCell(18).NumericCellValue),
                        };

                        var _currentSetting = _matlSettingCode13.FirstOrDefault(f => f.MatlNo == _tmpDao.MaterialNo)
                                ?? _matlSettingCode5.FirstOrDefault(f => f.MatlNo == _tmpDao.MaterialNo.Substring(0, 5)) ?? null;
                        var _currZipWOStatus = _workOrderEntity?.FirstOrDefault(f => f.WorkOrder == _tmpDao.Order) ?? new WorkOrderEntity
                        {
                            WOStatus = "NA",
                            WOType = "",
                            WOComment = "",
                            Scrap = 0
                        };
                        var _currMatlDef = _defMaterial.FirstOrDefault(f => f.CodeTypeId == MatlCodeTypeEnum.Code5 && f.MatlNo == _tmpDao.MaterialNo.Substring(0, 5));

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

                        _tmpDao.MatlShortName = _currentSetting?.MatlName ?? _currMatlDef?.MatlName ?? "";
                        _shortName = row.CreateCell(23);
                        _shortName.SetCellType(CellType.String);
                        _shortName.SetCellValue(_tmpDao.MatlShortName);
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
                        _useNode.SetCellValue(_currentSetting?.UseNode ?? _currMatlDef?.UseNode ?? "");
                        _useNode.CellStyle = _calculateCellStyle;

                        _tmpDao.WOComment = _currZipWOStatus.WOComment;
                        _comment = row.CreateCell(27);
                        _comment.SetCellType(CellType.String);
                        _comment.SetCellValue(_tmpDao.WOComment);
                        _comment.CellStyle = _calculateCellStyle;

                        _tmpDao.MESScrap = _currZipWOStatus.Scrap;
                        _mesScrap = row.CreateCell(28);
                        _mesScrap.SetCellType(CellType.Numeric);
                        _mesScrap.SetCellValue(_tmpDao.MESScrap);
                        _mesScrap.CellStyle = _calculateCellStyle;

                        #endregion

                        _insSAPWorkOrderList.Add(_tmpDao);

                    }
                    #endregion

                    Dictionary<string, int> _icScrapDic = _insSAPWorkOrderList.Where(w => w.MatlShortName == "IC").GroupBy(gb => gb.Order).Select(s => new
                    {
                        OrderNo = s.Key,
                        ICScrap = Convert.ToInt32(s.Sum(sum => sum.Unit))
                    }).ToDictionary(dicKey => dicKey.OrderNo, dicVal => dicVal.ICScrap);

                    int _currICScrap = 0;

                    // for迴圈的「啟始值」要加一，表示不包含 Excel表頭列
                    for (int i = (_sapDataSheet.FirstRowNum + 1); i <= _sapDataSheet.LastRowNum; i++)
                    {
                        // 每一列做迴圈
                        row = (XSSFRow)_sapDataSheet.GetRow(i); // 不包含 Excel表頭列

                        if (_icScrapDic.ContainsKey(row.GetCell(0).StringCellValue))
                        {
                            _currICScrap = _icScrapDic[row.GetCell(0).StringCellValue];

                            _icScrap = row.CreateCell(29);
                            _icScrap.SetCellType(CellType.Numeric);
                            _icScrap.SetCellValue(_currICScrap);
                            _icScrap.CellStyle = _calculateCellStyle;
                        }
                    }

                    _insSAPWorkOrderList.ForEach(ins => ins.ICScrap = _icScrapDic.TryGetValue(ins.Order, out _) ? _icScrapDic[ins.Order] : 0);

                    // 回寫本地計算檔
                    using (FileStream fs = new FileStream($"..\\tempFileProcess\\{fileName}", FileMode.Open, FileAccess.Write))
                    {
                        workbook.Write(fs);
                        fs.Close();
                    }

                    // FTP 上傳
                    _ftpService.FTP_Upload(uploadFile, "FTP_SAP", fileName, true, $"..\\tempFileProcess\\{fileName}");

                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 2, 0)))
                    {
                        _sapMaterialRepository.TruncateSAPwo();
                        _sapMaterialRepository.InsertSAPwo(_insSAPWorkOrderList);
                        scope.Complete();
                    }

                    CatchHelper.Delete("sapDropdown");
                    CatchHelper.Set("sapDropdown", JsonConvert.SerializeObject(_insSAPWorkOrderList.Select(s => new
                    {
                        s.Order,
                        s.Prod,
                        s.MaterialNo,
                        s.SAPNode
                    })), 432000);

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

        public List<MaterialSettingEntity> GetMaterialSetting(MatlCodeTypeEnum codeTypeId)
        {
            try
            {
                List<DefinitionMaterialDao> _defMaterial = _sapMaterialRepository.SelectAllMatlDef(codeTypeId);

                List<MaterialSettingDao> _matlAllSetting = _sapMaterialRepository.SelectMatlAllSetting(_defMaterial.Select(s => s.Sn).ToList());

                return (from setting in _matlAllSetting
                        join def in _defMaterial
                        on setting.MatlSn equals def.Sn
                        select new MaterialSettingEntity
                        {
                            MatlNo = def.MatlNo,
                            MatlCatg = def.MatlCatg,
                            MatlName = def.MatlName,
                            UseNode = def.UseNode,
                            LossRate = setting.LossRate
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

                List<DefinitionMaterialDao> _defMaterial = _sapMaterialRepository.SelectAllMatlDef(codeTypeId);

                List<MaterialSettingDao> _oldMatlSetting = _sapMaterialRepository.SelectMatlAllSetting(_defMaterial.Select(s => s.Sn).ToList());

                List<MaterialSettingDao> _updateMatlSetting = (from old in _oldMatlSetting
                                                               join def in _defMaterial
                                                               on old.MatlSn equals def.Sn
                                                               join upd in updEntity
                                                               on def.MatlNo equals upd.MatlNo
                                                               where old.LossRate != upd.LossRate
                                                               select new MaterialSettingDao
                                                               {
                                                                   MatlSn = def.Sn,
                                                                   LossRate = upd.LossRate,
                                                                   OldLossRate = old.LossRate
                                                               }).ToList();

                List<MaterialSettingHistoryDao> _updateMatlHis = _updateMatlSetting
                    .Select(setting => new MaterialSettingHistoryDao
                    {
                        MatlSn = setting.MatlSn,
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
        /// 5碼、13碼 料號耗損設定檔
        /// </summary>
        /// <param name="uploadFile">上傳檔案</param>
        /// <param name="codeFolder">存放上傳檔資料夾</param>
        /// <param name="userEntity">使用者資訊</param>
        /// <returns></returns>
        public (bool, string) UploadCodeRate(IFormFile uploadFile, MatlCodeTypeEnum codeTypeId, UserEntity userEntity)
        {
            try
            {
                bool _uplRes = false;
                int _uplCnt = 0;

                if (uploadFile.Length > 0)   // 檢查 <input type="file"> 是否輸入檔案？
                {
                    XSSFWorkbook workbook;
                    List<SAPWorkOrderDao> _insSAPWorkOrderList = new List<SAPWorkOrderDao>();
                    MaterialSettingDao _tmpDao = new MaterialSettingDao();
                    List<SAPWorkOrderDao> _updSAPwoList = new List<SAPWorkOrderDao>();
                    List<MaterialSettingDao> _insMatlSettingList = new List<MaterialSettingDao>();
                    DateTime _nowTime = DateTime.Now;

                    List<DefinitionMaterialDao> _defMaterial = _sapMaterialRepository.SelectAllMatlDef(codeTypeId);

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

                        if (row.GetCell(0) == null || string.IsNullOrEmpty(row.GetCell(0).StringCellValue))
                            break;

                        _uplCnt++;

                        int _matlSn = _defMaterial.FirstOrDefault(f => f.MatlNo == row.GetCell(0).StringCellValue)?.Sn ?? 0;

                        if (_matlSn == 0)
                            throw new Exception($"查無料號({row.GetCell(0).StringCellValue})部材簡稱");
                        if (codeTypeId == MatlCodeTypeEnum.Code5 && row.GetCell(0).StringCellValue.Length > 6 ||
                            codeTypeId == MatlCodeTypeEnum.Code13 && row.GetCell(0).StringCellValue.Length != 13)
                            throw new Exception($"請確認檔案內是否都為{codeTypeId.GetDescription()}, 異常料號({row.GetCell(0).StringCellValue})");

                        _tmpDao = new MaterialSettingDao()
                        {
                            MatlSn = _matlSn,
                            LossRate = Convert.ToDecimal(row.GetCell(1).NumericCellValue),
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
                        {
                            scope.Complete();
                            _uplRes = true;
                        }
                    }

                    return (_uplRes, _uplRes ? $"成功上傳筆數：{_uplCnt}\n請確認筆數是否正確" : "更新異常");
                }
                else
                    return (true, $"成功上傳筆數：{_uplCnt}");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public (bool, string, string) MatlSettingDownload(MatlCodeTypeEnum codeTypeId)
        {
            try
            {

                List<DefinitionMaterialDao> _defMaterial = _sapMaterialRepository.SelectAllMatlDef(codeTypeId);

                List<MaterialSettingDao> _matlSetting = _sapMaterialRepository.SelectMatlAllSetting(_defMaterial.Select(s => s.Sn).ToList());

                List<MaterialSettingEntity> _matlSettingList = (from set in _matlSetting
                                                                join def in _defMaterial
                                                                on set.MatlSn equals def.Sn
                                                                select new MaterialSettingEntity
                                                                {
                                                                    MatlNo = def.MatlNo,
                                                                    MatlName = def.MatlName,
                                                                    MatlCatg = def.MatlCatg,
                                                                    UseNode = def.UseNode,
                                                                    LossRate = set.LossRate
                                                                }).ToList();

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
                //ICell _name = _hRow.CreateCell(1);
                //_name.SetCellValue("品名");
                //ICell _desc = _hRow.CreateCell(2);
                //_desc.SetCellValue("料號分類說明");
                //ICell _useNode = _hRow.CreateCell(3);
                //_useNode.SetCellValue("使用站點");
                ICell _rate = _hRow.CreateCell(1);
                _rate.SetCellValue("耗損率");

                for (int i = 1; i < _matlSettingList.Count; i++)
                {
                    _hRow = _sheet.CreateRow(i);
                    _mNumber = _hRow.CreateCell(0);
                    _mNumber.SetCellValue(_matlSettingList[i].MatlNo);
                    //_name = _hRow.CreateCell(1);
                    //_name.SetCellValue(_matlSettingList[i].MatlName);
                    //_desc = _hRow.CreateCell(2);
                    //_desc.SetCellValue(_matlSettingList[i].MatlCatg);
                    //_useNode = _hRow.CreateCell(3);
                    //_useNode.SetCellValue(_matlSettingList[i].UseNode);
                    _rate = _hRow.CreateCell(1);
                    _rate.SetCellValue(Convert.ToDouble(_matlSettingList[i].LossRate));
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

        private async Task<List<WorkOrderEntity>> GetZipsumReport418Async(DateTime dateStr)
        {
            List<WorkOrderEntity> _wo418Entity = new List<WorkOrderEntity>();

            var _rpt418List = await _inxReportService.Get418NewReportAsync<INXRpt418Entity>(dateStr);

            var _parallelRes = Parallel.ForEach(_rpt418List.Date.Data.Table,
                new ParallelOptions { MaxDegreeOfParallelism = 8 },
                (rptData) =>
                {
                    List<WorkOrderEntity> _tmpWOEntity = new List<WorkOrderEntity>();
                    _tmpWOEntity.Add(new WorkOrderEntity
                    {
                        WorkOrder = rptData.WO_NBR,
                        WOStatus = rptData.WO_STATUS,
                        LcmProduct = rptData.PROD_NBR,
                        WOType = _woTypeDic[rptData.LCM_OWNER],
                        WOComment = rptData.COMMENT1,
                        Scrap = Convert.ToInt32(rptData.SCRAP_QTY),
                        ActualQty = Convert.ToInt32(rptData.ACTUAL_QTY)
                    });

                    lock (this)
                        _wo418Entity.AddRange(_tmpWOEntity);
                });

            //foreach (var rptData in _rpt418List.Date.Data.Table)
            //    _wo418Entity.Add(new WorkOrderEntity
            //    {
            //        WorkOrder = rptData.WO_NBR,
            //        WOStatus = rptData.WO_STATUS,
            //        LcmProduct = rptData.PROD_NBR,
            //        WOType = _woTypeDic[rptData.LCM_OWNER],
            //        WOComment = rptData.COMMENT1,
            //        Scrap = Convert.ToInt32(rptData.SCRAP_QTY),
            //        ActualQty = Convert.ToInt32(rptData.ACTUAL_QTY)
            //    });

            //string _qStr = $"wo_area=&wotype=ALL&iswodueday=N&Calendar1={dateStr}&calendar2={dateStr}&iswostartday=N&Calendar2_1={dateStr}&calendar2_2={dateStr}&workorder=&prod_type=ALL&wo_status=ALL&mvin_rate=ALL&mvou_rate=ALL&G_FAC=6&Shop=MOD4&calendar_1={dateStr}&calendar_2={dateStr}&calendar_2_1={dateStr}&calendar_2_2={dateStr}";
            //var data = new StringContent(_qStr, Encoding.UTF8, "application/x-www-form-urlencodedn");
            //List<string> _status;

            //using (var client = new HttpClient())
            //{
            //    HttpResponseMessage response = await client.PostAsync("http://zipsum/modreport/Report/MOD4/NHWoUnClose60DataSet.asp", data);

            //    response.Content.Headers.ContentType.CharSet = "Big5";

            //    string result = response.Content.ReadAsStringAsync().Result;

            //    result = result.Remove(0, 32900);

            //    _status = result.Split("<SCRIPT LANGUAGE=vbscript>").ToList();
            //}

            //_status.RemoveAt(_status.Count - 1);

            //_status.ForEach(wo =>
            //{
            //    var _tmp = wo.Split("ReportGrid1.TextMatrix");
            //    _wo418Entity.Add(new WorkOrderEntity
            //    {
            //        WorkOrder = _tmp[1].Split("\"")[1],
            //        WOStatus = _tmp[2].Split("\"")[1],
            //        LcmProduct = _tmp[3].Split("\"")[1],
            //        WOType = _woTypeDic[_tmp[5].Split("\"")[1]],
            //        WOComment = _tmp[13].Split("\"")[1],
            //        Scrap = Convert.ToInt32(_tmp[28].Split("\"")[1]),
            //        ActualQty = Convert.ToInt32(_tmp[16].Split("\"")[1])
            //    });
            //});

            return _wo418Entity;
        }
        #endregion

    }
}