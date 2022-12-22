using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MOD4.Web.ViewModel;

namespace MOD4.Web.Controllers
{
    [Authorize]
    public class BookingMeetingController : Controller
    {
        private readonly ILogger<BookingMeetingController> _logger;

        public BookingMeetingController(ILogger<BookingMeetingController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create([FromForm] MeetingViewModel createViewModel)
        {

            return RedirectToAction("Index");
        }
    }
}
