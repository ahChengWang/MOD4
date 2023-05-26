using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MOD4.Web.DomainService;

namespace MOD4.Web.Controllers
{
    [Authorize]
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
    }
}
