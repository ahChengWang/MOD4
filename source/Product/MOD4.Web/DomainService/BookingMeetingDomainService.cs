using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using MOD4.Web.Repostory;
using MOD4.Web.Repostory.Dao;
using NLog;
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
        private readonly ITaiwanCalendarRepository _taiwanCalendarRepository;
        private readonly IAccountDomainService _accountDomainService;
        private readonly IMAppDomainService _mappDomainService;
        private readonly Dictionary<MeetingRoomEnum, string> _roomBGCss;
        private readonly Dictionary<CIMTestTypeEnum, string> _cimTestTypeBGCss;

        public BookingMeetingDomainService(IBookingMeetingRepository bookingMeetingRepository,
            ICIMTestBookingRepository cimTestBookingRepository,
            ITaiwanCalendarRepository taiwanCalendarRepository,
            IAccountDomainService accountDomainService,
            IMAppDomainService mappDomainService)
        {
            _bookingMeetingRepository = bookingMeetingRepository;
            _cimTestBookingRepository = cimTestBookingRepository;
            _taiwanCalendarRepository = taiwanCalendarRepository;
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
            DateTime _nowTime = DateTime.Now;
            DateTime _startTime = DateTime.Parse($"{_nowTime.AddMonths(-6).ToString("yyyy/MM")}/01");

            var _bookingList = _cimTestBookingRepository.SelectByConditions(startTime: _startTime);

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

        public string GetAnnouncement()
        {
            var _bookingList = _cimTestBookingRepository.SelectByConditions(testTypeId: CIMTestTypeEnum.Announcement);

            return _bookingList.FirstOrDefault()?.Remark ?? "";
        }

        public string UpdateAnnouncement(string announcement)
        {
            try
            {
                string _updResponse = "";

                using (TransactionScope _scope = new TransactionScope())
                {
                    var _updRes = _cimTestBookingRepository.UpdateAnn(announcement) == 1;

                    if (_updRes)
                        _scope.Complete();
                    else
                        _updResponse = "更新異常";
                }

                return _updResponse;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

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

        public (List<CIMTestBookingEntity>, string) Create(CIMTestBookingEntity bookingEntity, UserEntity userEntity)
        {
            string _createRes = "";
            List<CIMTestBookingEntity> _newBookingList = new List<CIMTestBookingEntity>();

            if (string.IsNullOrEmpty(bookingEntity.Date?.Trim() ?? "") ||
                string.IsNullOrEmpty(bookingEntity.Name?.Trim() ?? "") || string.IsNullOrEmpty(bookingEntity.Subject?.Trim() ?? ""))
                return (null, "欄位不可空白");

            if (!DateTime.TryParse($"{bookingEntity.Date}", out _))
                return (null, "預約時間異常");

            var _accountInfoList = _accountDomainService.GetAccountInfoByConditions(0, bookingEntity.Name.Trim(), bookingEntity.JobId.Trim(), null);

            if (_accountInfoList == null || !_accountInfoList.Any())
                return (null, "請確認預約人姓名及工號");

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

            List<TaiwanCalendarDao> _yearHolidayList = _taiwanCalendarRepository.SelectByConditions(_startTime.Year);
            // if 預約到跨年度, 查詢明年度休假日
            if (_startTime.AddDays(bookingEntity.Days - 1).Month == 12)
            {
                _yearHolidayList.AddRange(_taiwanCalendarRepository.SelectByConditions(_startTime.Year + 1));
            }

            List<CIMTestBookingDao> _insCIMBookingList = new List<CIMTestBookingDao>();
            int _dayDelay = 0;

            // 處理多日預約
            for (int day = 0; day < bookingEntity.Days; day++)
            {
                if (_yearHolidayList.Any())
                {
                    while (_yearHolidayList.Any(d => d.Date == _startTime.AddDays(day + _dayDelay).Date))
                    {
                        _dayDelay++;
                    }
                }
                else
                {
                    if (_startTime.AddDays(day).DayOfWeek == DayOfWeek.Saturday)
                        _dayDelay += 2;
                }
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

            // 檢核會議重複
            var _meetingOverlap = _cimTestBookingRepository.VerifyOverlapByTimeList(_insCIMBookingList.Select(s => s.StartTime).ToList(), _insCIMBookingList.Select(s => s.EndTime).ToList());
            if (_meetingOverlap.Any())
            {
                string _overlapMsg = "預約會議時間與以下重疊 \n";

                foreach (var booking in _meetingOverlap)
                {
                    _overlapMsg += $"CIM 測試: {booking.Subject}; 預約人: {booking.Name}; {booking.StartTime:yyyy-MM-dd} {booking.StartTime.ToString("HH:mm")}-{booking.EndTime.ToString("HH:mm")} \n";
                }
                return (null, _overlapMsg);
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
                    chatId = "CarUX串CIM群",
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
                    CCUserList = new List<string> { "ALLAN.RO@CARUX.COM", "MORRISE.CHEN@CARUX.COM", "CHRIS31.WANG@INNOLUX.COM", "AKUEI.YANG@INNOLUX.COM", "FLOWER.LIN@CARUX.COM" },
                    Content = "<br /> Dear Sir ,<br /><br />" +
                    $"您 <a style='text-decoration:underline;font-weight:800'>【{bookingEntity.Subject}({bookingEntity.CIMTestTypeId.GetDescription()})】</a><a> CIM 測機排程已成功預約</a>， <br /><br />" +
                    $"日期 {_startTime} ~ {_endTime}， <br /><br />" +
                    $"詳細內容請至 <a href='http://CUX003184s/CarUX/BookingMeeting' target='_blank'>CIM 測機排程</a> 連結查看， <br /><br />" +
                    "謝謝"
                });

                // 查詢新增預約
                _newBookingList = _cimTestBookingRepository.SelectByConditions(
                    testTypeId: bookingEntity.CIMTestTypeId,
                    jobId: bookingEntity.JobId,
                    floor: bookingEntity.Floor,
                    testDayTypeId: bookingEntity.CIMTestDayTypeId,
                    startTime: _insCIMBookingList.Min(book => book.StartTime),
                    endTime: _insCIMBookingList.Max(book => book.EndTime)).Select(booking => new CIMTestBookingEntity
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

            return (_newBookingList, _createRes);
        }

        public (string, CIMTestBookingEntity) Update(CIMTestBookingEntity updBookingEntity, UserEntity userEntity)
        {
            string _createRes = "";

            if (updBookingEntity.Sn == 0)
                return ("排程異常", null);

            var _origBooking = _cimTestBookingRepository.SelectByConditions(sn: updBookingEntity.Sn).FirstOrDefault();

            if (_origBooking == null)
                return ("排程異常", null);

            var _accountInfoList = _accountDomainService.GetAccountInfoByConditions(0, updBookingEntity.Name.Trim(), updBookingEntity.JobId.Trim(), null);

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
            var _meetingOverlap = _cimTestBookingRepository.VerifyOverlap(_startTime, _endTime, sn: _origBooking.Sn);
            if (_meetingOverlap.Any())
            {
                string _overlapMsg = "預約時間與以下重疊 \n";

                foreach (var booking in _meetingOverlap)
                {
                    _overlapMsg += $"CIM 測試: {booking.Subject}; 預約人: {booking.Name}; 時間: {booking.StartTime.ToString("HH:mm:ss")} - {booking.EndTime.ToString("HH:mm:ss")} \n";
                }
                return (_overlapMsg, null);
            }

            CIMTestBookingDao _updCIMBooking = new CIMTestBookingDao()
            {
                Sn = updBookingEntity.Sn,
                CIMTestTypeId = updBookingEntity.CIMTestTypeId,
                Floor = updBookingEntity.Floor,
                Name = updBookingEntity.Name,
                JobId = updBookingEntity.JobId,
                Subject = updBookingEntity.Subject,
                CIMTestDayTypeId = updBookingEntity.CIMTestDayTypeId,
                StartTime = _startTime,
                EndTime = _endTime,
                BackgroundColor = _cimTestTypeBGCss[updBookingEntity.CIMTestTypeId],
                Remark = updBookingEntity.Remark + (updBookingEntity.CIMTestTypeId == CIMTestTypeEnum.Done ? $"[Last Status {_origBooking.CIMTestTypeId.GetDescription()}]" : ""),
            };

            //_origBooking.CIMTestTypeId = updBookingEntity.CIMTestTypeId;
            //_origBooking.Floor = updBookingEntity.Floor;
            //_origBooking.Name = updBookingEntity.Name;
            //_origBooking.JobId = updBookingEntity.JobId;
            //_origBooking.Subject = updBookingEntity.Subject;
            //_origBooking.CIMTestDayTypeId = updBookingEntity.CIMTestDayTypeId;
            //_origBooking.StartTime = _startTime;
            //_origBooking.EndTime = _endTime;
            //_origBooking.BackgroundColor = _cimTestTypeBGCss[updBookingEntity.CIMTestTypeId];
            //_origBooking.Remark = $"{_origBooking.Remark}{updBookingEntity.Remark}[Last Status{updBookingEntity.CIMTestTypeId.GetDescription()}]";

            using (TransactionScope _scope = new TransactionScope())
            {
                var _updResult = _cimTestBookingRepository.Update(_updCIMBooking) == 1;

                if (_updResult)
                    _scope.Complete();
                else
                    _createRes = "更新CIM test異常";
            }

            if (string.IsNullOrEmpty(_createRes) && _updCIMBooking.CIMTestTypeId != CIMTestTypeEnum.Done)
            {
                //_mappDomainService.SendMsgToOneAsync(
                //    $"【CIM 測機排程】預約更新通知, 申請人:{_updCIMBooking.Name}, 內容:{_updCIMBooking.Subject} {_updCIMBooking.CIMTestTypeId.GetDescription()}, 日期:{_startTime.ToShortDateString()} (網址:http://CUX003184s/CarUX/BookingMeeting)",
                //    "260837");

                _mappDomainService.SendMsgWithTagAsync(new MAppTagUserEntity
                {
                    url = "http://mapp.local/teamplus_innolux/EIM/Messenger/MessengerMain.aspx",
                    chatId = "CarUX串CIM群",
                    account = userEntity.Account,
                    password = _accountDomainService.Decrypt(userEntity.Password),
                    sendInfo = new List<MAppTagDetailEntity> {
                            new MAppTagDetailEntity {
                                message = $"【CIM 測機排程】預約更新通知, 申請人:{(userEntity.JobId == _updCIMBooking.JobId ? _updCIMBooking.Name : "@")} , 內容:{_updCIMBooking.Subject} {_updCIMBooking.CIMTestTypeId.GetDescription()}" +
                                          $", 日期:{_startTime.ToShortDateString()} (網址:http://CUX003184s/CarUX/BookingMeeting)",
                                users = new Dictionary<int, string>{
                                    { 1 ,_updCIMBooking.Name }
                                }
                            }
                        }
                });

                _mailServer.Send(new MailEntity
                {
                    To = _accountInfoList.FirstOrDefault().Mail,
                    Subject = $"【CIM 測機排程】預約申請更新通知 - 申請人:{_updCIMBooking.Name}",
                    CCUserList = new List<string> { "ALLAN.RO@CARUX.COM", "MORRISE.CHEN@CARUX.COM", "CHRIS31.WANG@INNOLUX.COM", "AKUEI.YANG@INNOLUX.COM", "FLOWER.LIN@CARUX.COM" },
                    Content = "<br /> Dear Sir ,<br /><br />" +
                    $"您 <a style='text-decoration:underline;font-weight:900'>【{_updCIMBooking.Subject}({_updCIMBooking.CIMTestTypeId.GetDescription()})】</a><a> CIM 測機排程已成功預約</a>， <br /><br />" +
                    $"日期 {_startTime} ~ {_endTime}， <br /><br />" +
                    $"詳細內容請至 <a href='http://CUX003184s/CarUX/BookingMeeting' target='_blank'>CIM 測機排程</a> 連結查看， <br /><br />" +
                    "謝謝"
                });
            }

            return (_createRes, new CIMTestBookingEntity
            {
                Sn = _updCIMBooking.Sn,
                CIMTestTypeId = _updCIMBooking.CIMTestTypeId,
                Floor = _updCIMBooking.Floor,
                Name = _updCIMBooking.Name,
                JobId = _updCIMBooking.JobId,
                Subject = _updCIMBooking.Subject,
                CIMTestDayTypeId = _updCIMBooking.CIMTestDayTypeId,
                StartTime = _updCIMBooking.StartTime,
                EndTime = _updCIMBooking.EndTime,
                BackgroundColor = _updCIMBooking.BackgroundColor,
                Remark = _updCIMBooking.Remark
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
