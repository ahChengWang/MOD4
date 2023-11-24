﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MOD4.Web.DomainService;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Models;
using MOD4.Web.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using Utility.Helper;

namespace MOD4.Web.Controllers
{
    [Authorize]
    public class MonitorController : BaseController
    {
        private readonly ILogger<ExtensionController> _logger;
        private readonly IMonitorDomainService _monitorDomainService;
        private readonly IOptionDomainService _optionDomainService;

        public MonitorController(IHttpContextAccessor httpContextAccessor,
            IAccountDomainService accountDomainService,
            IMonitorDomainService monitorDomainService,
            IOptionDomainService optionDomainService,
            ILogger<ExtensionController> logger)
            : base(httpContextAccessor, accountDomainService)
        {
            _monitorDomainService = monitorDomainService;
            _optionDomainService = optionDomainService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                var _result = _monitorDomainService.GetAlarmEq();

                MonitorViewModel _responseVM = new MonitorViewModel
                {
                    AlarmDayTop = _result.AlarmDayTop.Select(res => new MonitorAlarmDayTopViewModel
                    {
                        EqNumber = res.EqNumber,
                        ProdNo = res.ProdNo,
                        StatusCode = res.StatusCode,
                        Comment = res.Comment,
                        RepairedTime = res.RepairedTime
                    }).ToList(),
                    EqInfoList = new List<MonitorEqInfoViewModel>()
                };

                _result.MapAreaList.ForEach(map =>
                {
                    MonitorEqInfoViewModel _tmpEqInfo = new MonitorEqInfoViewModel();

                    _tmpEqInfo.EqNumber = map.EqNumber;
                    _tmpEqInfo.DefTopRate = map.DefTopRate;
                    _tmpEqInfo.DefLeftRate = map.DefLeftRate;
                    _tmpEqInfo.DefWith = map.DefWidth;
                    _tmpEqInfo.DefHeight = map.DefHeight;
                    _tmpEqInfo.Border = map.Border;
                    _tmpEqInfo.Background = map.Background;

                    var _tmpProdPerInfo = _result.ProdPerInfo.FirstOrDefault(f => f.EqNumber == map.EqNumber);

                    if (_tmpProdPerInfo != null)
                    {
                        _tmpEqInfo.ProdPerInfo = new MonitorProdPerInfoViewModel();
                        _tmpEqInfo.ProdPerInfo.EqNumber = _tmpProdPerInfo.EqNumber;
                        _tmpEqInfo.ProdPerInfo.ProdNo = _tmpProdPerInfo.ProdNo;
                        _tmpEqInfo.ProdPerInfo.EqNumber = _tmpProdPerInfo.EqNumber;
                        _tmpEqInfo.ProdPerInfo.PassQty = _tmpProdPerInfo.PassQty;

                        if (_result.AlarmList.FirstOrDefault(f => f.EqNumber == map.EqNumber) != null)
                        {
                            var _tmpAlarm = _result.AlarmList.FirstOrDefault(f => f.EqNumber == map.EqNumber);

                            _tmpEqInfo.AlarmInfo = new MonitorAlarmViewModel
                            {
                                EqNumber = _tmpAlarm.EqNumber,
                                ProdNo = _tmpAlarm.ProdNo,
                                StatusCode = _tmpAlarm.StatusCode,
                                Comment = _tmpAlarm.Comment,
                                StartTime = _tmpAlarm.StartTime,
                                IsAbnormal = true,
                                IsFrontEnd = _tmpAlarm.IsFrontEnd
                            };
                        }
                    }

                    _responseVM.EqInfoList.Add(_tmpEqInfo);
                });

                return View(_responseVM);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpPost("[controller]/ws")]
        public void ReceiveWebSocket([FromBody] MonitorViewModel monitorVM)
        {
            var webSocket = new ClientWebSocket();
            webSocket.ConnectAsync(new Uri($"ws://127.0.0.1:80/CarUX/send"), CancellationToken.None).Wait();
            ClientWebSocket socket = new ClientWebSocket();

            // 將訊息轉換成 byte 陣列
            byte[] buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
            {
                Area = "ASSY"
            }));

            // 發送訊息
            webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None).Wait();

            //var buffer = new byte[1024 * 4];
            //var res = webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).Result;
            //var userName = "anonymous";
            //while (!res.CloseStatus.HasValue)
            //{
            //    var cmd = Encoding.UTF8.GetString(buffer, 0, res.Count);
            //    res = webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).Result;
            //}
            //webSocket.CloseAsync(res.CloseStatus.Value, res.CloseStatusDescription, CancellationToken.None);
        }

        [HttpGet("[controller]/Setting")]
        public IActionResult Setting()
        {
            try
            {
                var _settingDatas = _monitorDomainService.GetMonitorMainList(1206);
                ViewBag.NodeOptions = _optionDomainService.GetNodeList();
                ViewBag.ProdOptions = JsonConvert.SerializeObject(_optionDomainService.GetLcmProdOptions());
                var _allEqList = _optionDomainService.GetEqIDAreaList();
                ViewBag.EqIDMappingOption = JsonConvert.SerializeObject(_allEqList);

                return View(new MonitorSettingMainViewModel
                {
                    MonitorProdTTList = _settingDatas.ProdTTDetails.Select(tt => new MonitorProdTTViewModel 
                    {
                        Node = tt.Node,
                        LcmProdSn = tt.LcmProdSn,
                        DownEquipment = tt.DownEquipment,
                        //DownEqOptions = _allEqList.Where(w => w.OPERATION == tt.Node.ToString())?.Select(s => new EqMappingViewModel
                        //{
                        //    EQUIP_NBR = s.EQUIP_NBR,
                        //    OPERATION = s.OPERATION
                        //}).ToList() ?? new List<EqMappingViewModel>(),
                        TimeTarget = tt.TimeTarget,
                        ProdDesc  = tt.ProdDesc
                    }).ToList(),
                    SettingDetail = _settingDatas.SettingDetails.Select(setting => new MonitorSettingViewModel
                    {
                        Node = setting.Node,
                        EqNumber = setting.EqNumber,
                        Border = setting.Border,
                        Background = setting.Background,
                        DefTopRate = setting.DefTopRate,
                        DefLeftRate = setting.DefLeftRate,
                        DefWidth = setting.DefWidth,
                        DefHeight = setting.DefHeight
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpGet("[controller]/Setting/TT/{prodSn}")]
        public IActionResult SettingTTSearch(int prodSn)
        {
            try
            {
                var _settingDatas = _monitorDomainService.GetMonitorProdTTList(prodSn);

                return Json(new ResponseViewModel<List<MonitorProdTTViewModel>> 
                { 
                    Data = _settingDatas.Select(tt => new MonitorProdTTViewModel
                    {
                        Node = tt.Node,
                        LcmProdSn = tt.LcmProdSn,
                        DownEquipment = tt.DownEquipment,
                        TimeTarget = tt.TimeTarget,
                        ProdDesc = tt.ProdDesc
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                return Json(new ResponseViewModel<List<MonitorProdTTViewModel>>
                {
                    IsSuccess = false,
                    Msg = ex.Message
                });
            }
        }

        [HttpPost("[controller]/Setting/TT")]
        public IActionResult SettingProdTT(List<MonitorProdTTViewModel> prodTTSetting)
        {
            try
            {
                string _result = _monitorDomainService.UpdateProdTT(prodTTSetting.CopyAToB<MonitorProdTTEntity>(), GetUserInfo());

                return Json(new ResponseViewModel<string>
                {
                    IsSuccess = string.IsNullOrEmpty(_result),
                    Msg = _result
                });
            }
            catch (Exception ex)
            {
                return Json(new ResponseViewModel<string>
                {
                    IsSuccess = false,
                    Msg = ex.Message
                });
            }
        }

        [HttpPost("[controller]/Setting/MapArea")]
        public IActionResult SettingMapArea(List<MonitorSettingViewModel> mapAreaSetting)
        {
            try
            {
                string _result = _monitorDomainService.UpdateInsertMapArea(mapAreaSetting.CopyAToB<MonitorSettingEntity>(), GetUserInfo());

                return Json(new ResponseViewModel<string>
                {
                    IsSuccess = string.IsNullOrEmpty(_result),
                    Msg = _result
                });
            }
            catch (Exception ex)
            {
                return Json(new ResponseViewModel<string>
                {
                    IsSuccess = false,
                    Msg = ex.Message
                });
            }
        }

        private void Echo(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            //等待接收訊息
            var receiveResult = webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).Result;
            //檢查是否為連線狀態
            while (!receiveResult.CloseStatus.HasValue)
            {
                //訊息發到前端
                webSocket.SendAsync(
                    new ArraySegment<byte>(buffer, 0, receiveResult.Count),
                    receiveResult.MessageType,
                    receiveResult.EndOfMessage,
                    CancellationToken.None
                    );
                //繼續等待接收訊息
                receiveResult = webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).Result;

            }
            //關閉連線
            //webSocket.CloseAsync(receiveResult.CloseStatus.Value, receiveResult.CloseStatusDescription, CancellationToken.None);

        }

    }
}
