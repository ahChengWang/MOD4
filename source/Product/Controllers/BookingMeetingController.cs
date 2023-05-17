using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MOD4.Web.DomainService;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using MOD4.Web.ViewModel;
using System.Collections.Generic;
using System.Linq;
using Utility.Helper;

namespace MOD4.Web.Controllers
{
    [Authorize]
    public class BookingMeetingController : BaseController
    {
        private readonly ILogger<BookingMeetingController> _logger;
        private readonly IBookingMeetingDomainService _bookingMeetingDomainService;

        public BookingMeetingController(ILogger<BookingMeetingController> logger,
            IHttpContextAccessor httpContextAccessor,
            IAccountDomainService accountDomainService,
            IBookingMeetingDomainService bookingMeetingDomainService)
            : base(httpContextAccessor, accountDomainService)
        {
            _logger = logger;
            _bookingMeetingDomainService = bookingMeetingDomainService;
        }

        public IActionResult Index()
        {
            UserEntity _userInfo = GetUserInfo();
            var _userCurrentPagePermission = _userInfo.UserMenuPermissionList.FirstOrDefault(f => f.MenuSn == MenuEnum.BookingMeeting);
            ViewBag.UserPermission = new UserPermissionViewModel
            {
                AccountSn = _userCurrentPagePermission.AccountSn,
                MenuSn = _userCurrentPagePermission.MenuSn,
                AccountPermission = _userCurrentPagePermission.AccountPermission
            };

            List<CIMTestBookingViewModel> _response = _bookingMeetingDomainService.GetList().Select(data => new CIMTestBookingViewModel
            {
                Sn = data.Sn,
                Name = data.Name,
                JobId = data.JobId,
                Subject = data.Subject,
                CIMTestTypeId = data.CIMTestTypeId,
                CIMTestType = data.CIMTestTypeId.GetDescription(),
                CIMTestDayTypeId = data.CIMTestDayTypeId,
                CIMTestDayType = data.CIMTestDayTypeId.GetDescription(),
                FloorId = data.Floor,
                StartYear = data.StartTime.Year,
                StartMonth = data.StartTime.Month,
                StartDay = data.StartTime.Day,
                StartHour = data.StartTime.Hour,
                StartMinute = data.StartTime.Minute,
                StartSecond = data.StartTime.Second,
                EndYear = data.EndTime.Year,
                EndMonth = data.EndTime.Month,
                EndDay = data.EndTime.Day,
                EndHour = data.EndTime.Hour,
                EndMinute = data.EndTime.Minute,
                EndSecond = data.EndTime.Second,
                BackgroundColor = data.BackgroundColor
            }).ToList();

            return View(_response);
        }

        [HttpGet]
        public IActionResult Search([FromQuery] MeetingRoomEnum roomId, string searchDate)
        {
            string _response = _bookingMeetingDomainService.GetFreeTimeRoom(roomId, searchDate);

            return Json(_response);
        }

        [HttpPost]
        public IActionResult Create([FromForm] CIMTestBookingViewModel createViewModel)
        {
            var _result = _bookingMeetingDomainService.Create(new CIMTestBookingEntity
            {
                Date = createViewModel.Date,
                CIMTestDayTypeId = createViewModel.CIMTestDayTypeId,
                CIMTestTypeId = createViewModel.CIMTestTypeId,
                Floor = createViewModel.FloorId,
                Name = createViewModel.Name,
                JobId = createViewModel.JobId,
                Subject = createViewModel.Subject,
            });

            if (_result != "")
            {
                return Json(new { IsSuccess = false, msg = _result });
            }
            else
                return Json(new { IsSuccess = true, msg = "" });
        }

        [HttpPost]
        public IActionResult Update([FromForm] CIMTestBookingViewModel updateViewModel)
        {
            var _result = _bookingMeetingDomainService.Update(new CIMTestBookingEntity
            {
                Sn = updateViewModel.Sn,
                Date = updateViewModel.Date,
                CIMTestTypeId = updateViewModel.CIMTestTypeId,
                Floor = updateViewModel.FloorId,
                Name = updateViewModel.Name,
                JobId = updateViewModel.JobId,
                Subject = updateViewModel.Subject,
                CIMTestDayTypeId = updateViewModel.CIMTestDayTypeId,
            });

            if (_result != "")
                return Json(new { IsSuccess = false, msg = _result });
            else
                return Json(new { IsSuccess = true, msg = "" });
        }

        [HttpDelete("[controller]/Cancel/{meetingSn}")]
        public IActionResult Cancel(int meetingSn)
        {
            var _result = _bookingMeetingDomainService.Delete(meetingSn);

            if (_result != "")
                return Json(new { IsSuccess = false, msg = _result });
            else
                return Json(new { IsSuccess = true, msg = "" });
        }
    }
}
