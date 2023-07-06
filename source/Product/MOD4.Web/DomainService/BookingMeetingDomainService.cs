using Microsoft.Extensions.Configuration;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using MOD4.Web.Repostory;
using MOD4.Web.Repostory.Dao;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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
                { CIMTestTypeEnum.SpecDev,"#bf2c2e" },
                { CIMTestTypeEnum.Meeting,"#8427c2" },
                { CIMTestTypeEnum.Done,"#666666" },
            };
        }


        public List<CIMTestBookingEntity> GetList()
        {
            var _bookingList = _cimTestBookingRepository.SelectByConditions();

            _logHelper.WriteLog(LogLevel.Info, this.GetType().Name, $"使用者查詢");

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
                BackgroundColor = booking.BackgroundColor,
                Remark = booking.Remark
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

        public string Create(CIMTestBookingEntity bookingEntity, UserEntity userEntity)
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
            var _meetingOverlap = _cimTestBookingRepository.VerifyOverlap(_startTime, _endTime.AddDays(bookingEntity.Days - 1));
            if (_meetingOverlap.Any())
            {
                string _overlapMsg = "預約會議時間與以下重疊 \n";

                foreach (var booking in _meetingOverlap)
                {
                    _overlapMsg += $"CIM 測試: {booking.Subject}; 預約人: {booking.Name}; 時間: {booking.StartTime.ToString("HH:mm:ss")} - {booking.EndTime.ToString("HH:mm:ss")} \n";
                }
                return _overlapMsg;
            }

            List<CIMTestBookingDao> _insCIMBookingList = new List<CIMTestBookingDao>();
            int _dayDelay = 0;

            for (int day = 0; day < bookingEntity.Days; day++)
            {
                if (_startTime.AddDays(day).DayOfWeek == DayOfWeek.Saturday)
                    _dayDelay += 2;

                _insCIMBookingList.Add(new CIMTestBookingDao
                {
                    CIMTestTypeId = bookingEntity.CIMTestTypeId,
                    Floor = bookingEntity.Floor,
                    Name = bookingEntity.Name,
                    JobId = bookingEntity.JobId,
                    Subject = bookingEntity.Subject,
                    CIMTestDayTypeId = bookingEntity.CIMTestDayTypeId,
                    StartTime = _startTime.AddDays(day + _dayDelay),
                    EndTime = _endTime.AddDays(day + _dayDelay),
                    BackgroundColor = _cimTestTypeBGCss[bookingEntity.CIMTestTypeId],
                    Remark = bookingEntity.Remark
                });
            }

            using (TransactionScope _scope = new TransactionScope())
            {
                var _insResult = _cimTestBookingRepository.Insert(_insCIMBookingList) == _insCIMBookingList.Count;

                if (_insResult)
                    _scope.Complete();
                else
                    _createRes = "CIM test 預約異常";
            }

            if (string.IsNullOrEmpty(_createRes))
            {
                //_mappDomainService.SendMsgToOneAsync(
                //    $"【CIM 測機排程】預約成功通知, 申請人:{bookingEntity.Name}, 內容:{bookingEntity.Subject} {bookingEntity.CIMTestTypeId.GetDescription()}" +
                //    $", 日期:{_startTime.ToShortDateString()} ~ {_endTime.AddDays(bookingEntity.Days - 1 + _dayDelay).ToShortDateString()} (網址:http://CUX003184s/CarUX/BookingMeeting)",
                //    "260837");

                _mappDomainService.SendMsgWithTagAsync(new MAppTagUserEntity
                {
                    url = "http://mapp.local/teamplus_innolux/EIM/Messenger/MessengerMain.aspx",
                    chatId = "6cdef893-0d02-4f39-ab2b-9f6521c3f9cb",
                    account = userEntity.Account,
                    password = _accountDomainService.Decrypt(userEntity.Password),
                    sendInfo = new List<MAppTagDetailEntity> {
                            new MAppTagDetailEntity {
                                message = $"【CIM 測機排程】預約成功通知, 申請人:@ , 內容:{bookingEntity.Subject} {bookingEntity.CIMTestTypeId.GetDescription()}" +
                                          $", 日期:{_startTime.ToShortDateString()} ~ {_endTime.AddDays(bookingEntity.Days - 1 + _dayDelay).ToShortDateString()} (網址:http://CUX003184s/CarUX/BookingMeeting)",
                                users = new Dictionary<int, string>{
                                    { 1 ,bookingEntity.Name }
                                }
                            }
                        }
                });

                _mailServer.Send(new MailEntity
                {
                    To = _accountInfoList.FirstOrDefault().Mail,
                    Subject = $"【CIM 測機排程】預約申請成功通知 - 申請人:{bookingEntity.Name}",
                    CCUserList = new List<string> { "ALLAN.RO@INNOLUX.COM", "MORRISE.CHEN@INNOLUX.COM", "CHRIS31.WANG@INNOLUX.COM", "FLOWER.LIN@INNOLUX.COM" },
                    Content = "<br /> Dear Sir ,<br /><br />" +
                    $"您 <a style='text-decoration:underline;font-weight:800'>【{bookingEntity.Subject}({bookingEntity.CIMTestTypeId.GetDescription()})】</a><a> CIM 測機排程已成功預約</a>， <br /><br />" +
                    $"日期 {_startTime} ~ {_endTime}， <br /><br />" +
                    $"詳細內容請至 <a href='http://CUX003184s/CarUX/BookingMeeting' target='_blank'>CIM 測機排程</a> 連結查看， <br /><br />" +
                    "謝謝"
                });
            }

            return _createRes;
        }

        public (string, CIMTestBookingEntity) Update(CIMTestBookingEntity updBookingEntity, UserEntity userEntity)
        {
            string _createRes = "";

            if (updBookingEntity.Sn == 0)
                return ("排程異常", null);

            var _booking = _cimTestBookingRepository.SelectByConditions(sn: updBookingEntity.Sn).FirstOrDefault();

            if (_booking == null)
                return ("排程異常", null);

            var _accountInfoList = _accountDomainService.GetAccountInfoByConditions(null, updBookingEntity.Name.Trim(), updBookingEntity.JobId.Trim(), null);

            if (_accountInfoList == null || !_accountInfoList.Any())
                return ("請確認預約人姓名及工號", null);

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

            // 檢核會議重複
            var _meetingOverlap = _cimTestBookingRepository.VerifyOverlap(_startTime, _endTime, sn: _booking.Sn);
            if (_meetingOverlap.Any())
            {
                string _overlapMsg = "預約時間與以下重疊 \n";

                foreach (var booking in _meetingOverlap)
                {
                    _overlapMsg += $"CIM 測試: {booking.Subject}; 預約人: {booking.Name}; 時間: {booking.StartTime.ToString("HH:mm:ss")} - {booking.EndTime.ToString("HH:mm:ss")} \n";
                }
                return (_overlapMsg, null);
            }

            _booking.CIMTestTypeId = updBookingEntity.CIMTestTypeId;
            _booking.Floor = updBookingEntity.Floor;
            _booking.Name = updBookingEntity.Name;
            _booking.JobId = updBookingEntity.JobId;
            _booking.Subject = updBookingEntity.Subject;
            _booking.CIMTestDayTypeId = updBookingEntity.CIMTestDayTypeId;
            _booking.StartTime = _startTime;
            _booking.EndTime = _endTime;
            _booking.BackgroundColor = _cimTestTypeBGCss[updBookingEntity.CIMTestTypeId];
            _booking.Remark = updBookingEntity.Remark;

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
                //_mappDomainService.SendMsgToOneAsync(
                //    $"【CIM 測機排程】預約更新通知, 申請人:{_booking.Name}, 內容:{_booking.Subject} {_booking.CIMTestTypeId.GetDescription()}, 日期:{_startTime.ToShortDateString()} (網址:http://CUX003184s/CarUX/BookingMeeting)",
                //    "260837");

                _mappDomainService.SendMsgWithTagAsync(new MAppTagUserEntity
                {
                    url = "http://mapp.local/teamplus_innolux/EIM/Messenger/MessengerMain.aspx",
                    chatId = "6cdef893-0d02-4f39-ab2b-9f6521c3f9cb",
                    account = userEntity.Account,
                    password = _accountDomainService.Decrypt(userEntity.Password),
                    sendInfo = new List<MAppTagDetailEntity> {
                            new MAppTagDetailEntity {
                                message = $"【CIM 測機排程】預約更新通知, 申請人:{(userEntity.JobId == _booking.JobId ? _booking.Name : "@")} , 內容:{_booking.Subject} {_booking.CIMTestTypeId.GetDescription()}" +
                                          $", 日期:{_startTime.ToShortDateString()} (網址:http://CUX003184s/CarUX/BookingMeeting)",
                                users = new Dictionary<int, string>{
                                    { 1 ,_booking.Name }
                                }
                            }
                        }
                });

                _mailServer.Send(new MailEntity
                {
                    To = _accountInfoList.FirstOrDefault().Mail,
                    Subject = $"【CIM 測機排程】預約申請更新通知 - 申請人:{_booking.Name}",
                    CCUserList = new List<string> { "ALLAN.RO@INNOLUX.COM", "MORRISE.CHEN@INNOLUX.COM", "CHRIS31.WANG@INNOLUX.COM", "FLOWER.LIN@INNOLUX.COM" },
                    Content = "<br /> Dear Sir ,<br /><br />" +
                    $"您 <a style='text-decoration:underline;font-weight:900'>【{_booking.Subject}({_booking.CIMTestTypeId.GetDescription()})】</a><a> CIM 測機排程已成功預約</a>， <br /><br />" +
                    $"日期 {_startTime} ~ {_endTime}， <br /><br />" +
                    $"詳細內容請至 <a href='http://CUX003184s/CarUX/BookingMeeting' target='_blank'>CIM 測機排程</a> 連結查看， <br /><br />" +
                    "謝謝"
                });
            }

            return (_createRes, new CIMTestBookingEntity
            {
                Sn = _booking.Sn,
                CIMTestTypeId = _booking.CIMTestTypeId,
                Floor = _booking.Floor,
                Name = _booking.Name,
                JobId = _booking.JobId,
                Subject = _booking.Subject,
                CIMTestDayTypeId = _booking.CIMTestDayTypeId,
                StartTime = _booking.StartTime,
                EndTime = _booking.EndTime,
                BackgroundColor = _booking.BackgroundColor,
                Remark = _booking.Remark
            });
        }

        public string Delete(int meetingSn)
        {
            string _delResult = "";
            var _booking = _cimTestBookingRepository.SelectByConditions(sn: meetingSn);

            if (_booking == null || !_booking.Any())
                return "查無 CIM 排程預約";

            using (TransactionScope _scope = new TransactionScope())
            {
                var _updResult = _cimTestBookingRepository.Delete(meetingSn) == 1;

                if (_updResult)
                    _scope.Complete();
                else
                    _delResult = "取消會議異常";
            }

            return _delResult;
        }
    }
}
