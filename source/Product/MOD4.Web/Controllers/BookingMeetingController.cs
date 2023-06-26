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
                BackgroundColor = data.BackgroundColor,
                Remark = data.Remark
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
                Days = createViewModel.Days,
                Name = createViewModel.Name,
                JobId = createViewModel.JobId,
                Subject = createViewModel.Subject,
                Remark = createViewModel.Remark
            }, GetUserInfo());

            return Json(new ResponseViewModel<List<string>>
            {
                IsSuccess = _result == "",
                Msg = _result == "" ? "" : _result
            });
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
                Remark = updateViewModel.Remark
            }, GetUserInfo());

            CIMTestBookingViewModel _response = new CIMTestBookingViewModel();

            if (string.IsNullOrEmpty(_result.Item1))
                _response = new CIMTestBookingViewModel
                {
                    Sn = _result.Item2.Sn,
                    Date = _result.Item2.Date,
                    CIMTestDayTypeId = _result.Item2.CIMTestDayTypeId,
                    CIMTestTypeId = _result.Item2.CIMTestTypeId,
                    FloorId = _result.Item2.Floor,
                    Days = _result.Item2.Days,
                    Name = _result.Item2.Name,
                    JobId = _result.Item2.JobId,
                    Subject = _result.Item2.Subject,
                    Remark = _result.Item2.Remark,
                    StartYear = _result.Item2.StartTime.Year,
                    StartMonth = _result.Item2.StartTime.Month,
                    StartDay = _result.Item2.StartTime.Day,
                    StartHour = _result.Item2.StartTime.Hour,
                    StartMinute = _result.Item2.StartTime.Minute,
                    StartSecond = _result.Item2.StartTime.Second,
                    EndYear = _result.Item2.EndTime.Year,
                    EndMonth = _result.Item2.EndTime.Month,
                    EndDay = _result.Item2.EndTime.Day,
                    EndHour = _result.Item2.EndTime.Hour,
                    EndMinute = _result.Item2.EndTime.Minute,
                    EndSecond = _result.Item2.EndTime.Second,
                    BackgroundColor = _result.Item2.BackgroundColor
                };

            return Json(new ResponseViewModel<CIMTestBookingViewModel>
            {
                IsSuccess = _result.Item1 == "",
                Data = _response,
                Msg = _result.Item1 == "" ? "" : _result.Item1
            });
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
