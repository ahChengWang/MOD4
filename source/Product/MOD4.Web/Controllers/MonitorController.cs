using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MOD4.Web.DomainService;
using MOD4.Web.ViewModel;
using Newtonsoft.Json;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace MOD4.Web.Controllers
{
    //[Authorize]
    public class MonitorController : BaseController
    {
        private readonly ILogger<ExtensionController> _logger;

        public MonitorController(IHttpContextAccessor httpContextAccessor,
            IAccountDomainService accountDomainService,
            ILogger<ExtensionController> logger)
            : base(httpContextAccessor, accountDomainService)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("[controller]/ws")]
        public void ReceiveWebSocket([FromBody] MonitorViewModel monitorVM)
        {
            var webSocket = new ClientWebSocket();
            webSocket.ConnectAsync(new Uri($"ws://localhost:48291/CarUX/send"), CancellationToken.None).Wait();
            ClientWebSocket socket = new ClientWebSocket();

            // 將訊息轉換成 byte 陣列
            byte[] buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
            {
                Area = "ASSY",
                Key = monitorVM.AreaId,
                IsAbnormal = monitorVM.IsAbnormal,
                Code = monitorVM.Code,
                Desc = monitorVM.Desc
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
