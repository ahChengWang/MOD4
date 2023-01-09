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

            List<MeetingCreateViewModel> _response = _bookingMeetingDomainService.GetList().Select(data => new MeetingCreateViewModel
            {
                Sn = data.Sn,
                MeetingRoomId = data.MeetingRoomId,
                MeetingRoom = data.MeetingRoom,
                Name = data.Name,
                Subject = data.Subject,
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

        [HttpPost]
        public IActionResult Create([FromForm] MeetingCreateViewModel createViewModel)
        {
            var _result = _bookingMeetingDomainService.Create(new BookingRoomEntity
            {
                MeetingRoomId = createViewModel.MeetingRoomId,
                Date = createViewModel.Date,
                Time = createViewModel.TimeStart,
                Name = createViewModel.Name,
                Subject = createViewModel.Subject,
                RepeatWeekly = createViewModel.RepeatWeekly
            });

            if (_result != "")
            {
                return Json(new { IsSuccess = false, msg = _result });
            }
            else
                return Json(new { IsSuccess = true, msg = "" });
        }

        [HttpPost]
        public IActionResult Update([FromForm] MeetingCreateViewModel updateViewModel)
        {
            var _result = _bookingMeetingDomainService.Update(new BookingRoomEntity
            {
                Sn = updateViewModel.Sn,
                StartTime = updateViewModel.ResizeStartTime,
                EndTime = updateViewModel.ResizeEndTime
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
