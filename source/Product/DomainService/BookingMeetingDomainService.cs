using Microsoft.Extensions.Configuration;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using MOD4.Web.Repostory;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Utility.Helper;

namespace MOD4.Web.DomainService
{
    public class BookingMeetingDomainService : BaseDomainService, IBookingMeetingDomainService
    {
        private readonly IBookingMeetingRepository _bookingMeetingRepository;
        private readonly ICIMTestBookingRepository _cimTestBookingRepository;
        private readonly IAccountDomainService _accountDomainService;
        private readonly IMAppDomainService _mappDomainService;
        private readonly Dictionary<MeetingRoomEnum, string> _roomBGCss;
        private readonly Dictionary<CIMTestTypeEnum, string> _cimTestTypeBGCss;

        public BookingMeetingDomainService(IBookingMeetingRepository bookingMeetingRepository,
            ICIMTestBookingRepository cimTestBookingRepository,
            IAccountDomainService accountDomainService,
            IMAppDomainService mappDomainService)
        {
            _bookingMeetingRepository = bookingMeetingRepository;
            _cimTestBookingRepository = cimTestBookingRepository;
            _accountDomainService = accountDomainService;
            _mappDomainService = mappDomainService;
            _roomBGCss = new Dictionary<MeetingRoomEnum, string> {
                { MeetingRoomEnum.R201,"#01803B" },
                { MeetingRoomEnum.R202,"#F1E515" },
                { MeetingRoomEnum.R204,"#C50C0C" },
                { MeetingRoomEnum.InfoCenter,"#3F48CC" },
                { MeetingRoomEnum.R206,"#A349A4" },
            };
            _cimTestTypeBGCss = new Dictionary<CIMTestTypeEnum, string> {
                { CIMTestTypeEnum.FABTest,"#4bd60f" },
                { CIMTestTypeEnum.LABTest,"#f0d107" },
                { CIMTestTypeEnum.Done,"#666666" },
            };
        }


        public List<CIMTestBookingEntity> GetList()
        {
            var _bookingList = _cimTestBookingRepository.SelectByConditions();

            return _bookingList.Select(booking => new CIMTestBookingEntity
            {
                Sn = booking.Sn,
                Name = booking.Name,
                Subject = booking.Subject,
                CIMTestTypeId = booking.CIMTestTypeId,
                Floor = booking.Floor,
                JobId = booking.JobId,
                CIMTestDayTypeId = booking.CIMTestDayTypeId,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                BackgroundColor = booking.BackgroundColor
            }).ToList();
        }


        public string GetFreeTimeRoom(MeetingRoomEnum roomId, string date)
        {
            DateTime _startTime = DateTime.Parse(date);
            DateTime _endTime = DateTime.Parse(date).AddDays(1).AddMilliseconds(-1);

            var _alreadyBookingMeeting = _bookingMeetingRepository.VerifyOverlap(roomId, _startTime, _endTime).OrderBy(o => o.StartTime);

            if (_alreadyBookingMeeting.Any())
            {
                string _response = $"會議室 {roomId.GetDescription()} {date} 空閒時段如下：\n";
                DateTime _tmpStartTime = DateTime.Parse($"{date} 08:00:00");

                foreach (BookingMeetingDao meeting in _alreadyBookingMeeting)
                {
                    if (meeting.StartTime > _tmpStartTime)
                    {
                        _response += $"{_tmpStartTime.ToString("HH:mm:ss")} - {meeting.StartTime.ToString("HH:mm:ss")}\n";
                        _tmpStartTime = meeting.EndTime;
                    }
                    else
                        _tmpStartTime = meeting.EndTime;
                }
                _response += $"{_tmpStartTime.ToString("HH:mm:ss")} - \n";
                return _response;
            }
            else
                return $"{date} 全時段都可預約";
        }

        public string Create(CIMTestBookingEntity bookingEntity)
        {
            string _createRes = "";

            if (string.IsNullOrEmpty(bookingEntity.Date?.Trim() ?? "") ||
                string.IsNullOrEmpty(bookingEntity.Name?.Trim() ?? "") || string.IsNullOrEmpty(bookingEntity.Subject?.Trim() ?? ""))
                return "欄位不可空白";

            if (!DateTime.TryParse($"{bookingEntity.Date}", out _))
                return "預約時間異常";

            var _accountInfoList = _accountDomainService.GetAccountInfoByConditions(null, bookingEntity.Name.Trim(), bookingEntity.JobId.Trim(), null);

            if (_accountInfoList == null || !_accountInfoList.Any())
                return "請確認預約人姓名及工號";

            DateTime _startTime = DateTime.Now;
            DateTime _endTime = DateTime.Now;

            switch (bookingEntity.CIMTestDayTypeId)
            {
                case CIMTestDayTypeEnum.AM:
                    _startTime = DateTime.Parse($"{bookingEntity.Date} 08:00:00");
                    _endTime = DateTime.Parse($"{bookingEntity.Date} 12:00:00");
                    break;
                case CIMTestDayTypeEnum.PM:
                    _startTime = DateTime.Parse($"{bookingEntity.Date} 13:00:00");
                    _endTime = DateTime.Parse($"{bookingEntity.Date} 17:00:00");
                    break;
                case CIMTestDayTypeEnum.AllDay:
                    _startTime = DateTime.Parse($"{bookingEntity.Date} 08:00:00");
                    _endTime = DateTime.Parse($"{bookingEntity.Date} 17:00:00");
                    break;
                default:
                    break;
            }

            // 檢核會議重複
            var _meetingOverlap = _cimTestBookingRepository.VerifyOverlap(_startTime, _endTime);
            if (_meetingOverlap.Any())
            {
                string _overlapMsg = "預約會議時間與以下重疊 \n";

                foreach (var booking in _meetingOverlap)
                {
                    _overlapMsg += $"CIM 測試: {booking.Subject}; 預約人: {booking.Name}; 時間: {booking.StartTime.ToString("HH:mm:ss")} - {booking.EndTime.ToString("HH:mm:ss")} \n";
                }
                return _overlapMsg;
            }

            CIMTestBookingDao _insCIMBooking = new CIMTestBookingDao
            {
                CIMTestTypeId = bookingEntity.CIMTestTypeId,
                Floor = bookingEntity.Floor,
                Name = bookingEntity.Name,
                JobId = bookingEntity.JobId,
                Subject = bookingEntity.Subject,
                CIMTestDayTypeId = bookingEntity.CIMTestDayTypeId,
                StartTime = _startTime,
                EndTime = _endTime,
                BackgroundColor = _cimTestTypeBGCss[bookingEntity.CIMTestTypeId]
            };

            using (TransactionScope _scope = new TransactionScope())
            {
                var _insResult = _cimTestBookingRepository.Insert(new List<CIMTestBookingDao> { _insCIMBooking }) == 1;

                if (_insResult)
                    _scope.Complete();
                else
                    _createRes = "CIM test 預約異常";
            }

            if (string.IsNullOrEmpty(_createRes))
            {
                _mappDomainService.SendMsgToOneAsync($"CIM 測機排程預約成功通知, 申請人:{_insCIMBooking.Name}, 議題:{_insCIMBooking.Subject} {_insCIMBooking.CIMTestTypeId.GetDescription()}, 日期:{_startTime}~{_endTime}", "249367");
                _mailServer.Send(new MailEntity
                {
                    To = _accountInfoList.FirstOrDefault().Mail,
                    Subject = $"【CIM 測機排程】預約申請成功通知 - 申請人:{_insCIMBooking.Name}",
                    CCUserList = new List<string> { "morrise.chen@innlux.com", "flower.lin@innlux.com;" },
                    Content = "<br /> Dear Sir ,<br /><br />" +
                    $"您 <a style='text-decoration:underline'>[{_insCIMBooking.Subject}({_insCIMBooking.CIMTestTypeId.GetDescription()})]</a><a style='font-weight:900'> CIM 測機排程已成功預約</a>， <br /><br />" +
                    $"日期 {_startTime} ~ {_endTime}， <br /><br />" +
                    $"詳細內容請至 <a href='http://10.54.215.210/CarUX/BookingMeeting' target='_blank'>CIM 測機排程</a> 連結查看， <br /><br />" +
                    "謝謝"
                });
            }

            return _createRes;
        }

        public string Update(CIMTestBookingEntity updBookingEntity)
        {
            string _createRes = "";

            if (updBookingEntity.Sn == 0)
                return "排程異常";

            var _booking = _cimTestBookingRepository.SelectByConditions(sn: updBookingEntity.Sn).FirstOrDefault();

            if (_booking == null)
                return "排程異常";

            var _accountInfoList = _accountDomainService.GetAccountInfoByConditions(null, updBookingEntity.Name.Trim(), updBookingEntity.JobId.Trim(), null);

            if (_accountInfoList == null || !_accountInfoList.Any())
                return "請確認預約人姓名及工號";

            DateTime _startTime = DateTime.Now;
            DateTime _endTime = DateTime.Now;

            switch (updBookingEntity.CIMTestDayTypeId)
            {
                case CIMTestDayTypeEnum.AM:
                    _startTime = DateTime.Parse($"{updBookingEntity.Date} 08:00:00");
                    _endTime = DateTime.Parse($"{updBookingEntity.Date} 12:00:00");
                    break;
                case CIMTestDayTypeEnum.PM:
                    _startTime = DateTime.Parse($"{updBookingEntity.Date} 13:00:00");
                    _endTime = DateTime.Parse($"{updBookingEntity.Date} 17:00:00");
                    break;
                case CIMTestDayTypeEnum.AllDay:
                    _startTime = DateTime.Parse($"{updBookingEntity.Date} 08:00:00");
                    _endTime = DateTime.Parse($"{updBookingEntity.Date} 17:00:00");
                    break;
                default:
                    break;
            }

            _booking.StartTime = _startTime;
            _booking.EndTime = _endTime;
            _booking.CIMTestTypeId = updBookingEntity.CIMTestTypeId;
            _booking.Floor = updBookingEntity.Floor;
            _booking.Name = updBookingEntity.Name;
            _booking.JobId = updBookingEntity.JobId;
            _booking.Subject = updBookingEntity.Subject;
            _booking.CIMTestDayTypeId = updBookingEntity.CIMTestDayTypeId;
            _booking.StartTime = _startTime;
            _booking.EndTime = _endTime;
            _booking.BackgroundColor = _cimTestTypeBGCss[updBookingEntity.CIMTestTypeId];

            // 檢核會議重複
            var _meetingOverlap = _cimTestBookingRepository.VerifyOverlap(_startTime, _endTime, sn: _booking.Sn);
            if (_meetingOverlap.Any())
            {
                string _overlapMsg = "預約會議時間與以下重疊 \n";

                foreach (var booking in _meetingOverlap)
                {
                    _overlapMsg += $"CIM 測試: {booking.Subject}; 預約人: {booking.Name}; 時間: {booking.StartTime.ToString("HH:mm:ss")} - {booking.EndTime.ToString("HH:mm:ss")} \n";
                }
                return _overlapMsg;
            }

            using (TransactionScope _scope = new TransactionScope())
            {
                var _updResult = _cimTestBookingRepository.Update(_booking) == 1;

                if (_updResult)
                    _scope.Complete();
                else
                    _createRes = "更新CIM test異常";
            }

            if (string.IsNullOrEmpty(_createRes) && _booking.CIMTestTypeId != CIMTestTypeEnum.Done)
            {
                _mappDomainService.SendMsgToOneAsync($"CIM 測機排程預約更新通知, 申請人:{_booking.Name}, 議題:{_booking.Subject} {_booking.CIMTestTypeId.GetDescription()}, 日期:{_startTime}~{_endTime}", "249367");
                _mailServer.Send(new MailEntity
                {
                    To = _accountInfoList.FirstOrDefault().Mail,
                    Subject = $"【CIM 測機排程】預約申請更新通知 - 申請人:{_booking.Name}",
                    CCUserList = new List<string> { "morrise.chen@innlux.com", "flower.lin@innlux.com;" },
                    Content = "<br /> Dear Sir ,<br /><br />" +
                    $"您 <a style='text-decoration:underline'>[{_booking.Subject}({_booking.CIMTestTypeId.GetDescription()})]</a><a style='font-weight:900'> CIM 測機排程已成功預約</a>， <br /><br />" +
                    $"日期 {_startTime} ~ {_endTime}， <br /><br />" +
                    $"詳細內容請至 <a href='http://10.54.215.210/CarUX/BookingMeeting' target='_blank'>CIM 測機排程</a> 連結查看， <br /><br />" +
                    "謝謝"
                });
            }

            return _createRes;
        }

        public string Delete(int meetingSn)
        {
            string _delResult = "";
            var _booking = _bookingMeetingRepository.SelectByConditions(sn: meetingSn).FirstOrDefault();

            if (_booking == null)
                return "查無此會議";

            using (TransactionScope _scope = new TransactionScope())
            {
                var _updResult = _bookingMeetingRepository.Delete(meetingSn) == 1;

                if (_updResult)
                    _scope.Complete();
                else
                    _delResult = "取消會議異常";
            }

            return _delResult;
        }
    }
}
