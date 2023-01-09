using Microsoft.Extensions.Configuration;
using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using MOD4.Web.Repostory;
using MOD4.Web.Repostory.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace MOD4.Web.DomainService
{
    public class BookingMeetingDomainService : IBookingMeetingDomainService
    {
        private readonly IBookingMeetingRepository _bookingMeetingRepository;
        private readonly Dictionary<MeetingRoomEnum, string> _roomBGCss;

        public BookingMeetingDomainService(IBookingMeetingRepository bookingMeetingRepository)
        {
            _bookingMeetingRepository = bookingMeetingRepository;
            _roomBGCss = new Dictionary<MeetingRoomEnum, string> {
                { MeetingRoomEnum.R201,"#01803B" },
                { MeetingRoomEnum.R202,"#F1E515" },
                { MeetingRoomEnum.R204,"#C50C0C" },
                { MeetingRoomEnum.InfoCenter,"#3F48CC" },
                { MeetingRoomEnum.R206,"#A349A4" },
            };
        }


        public List<BookingRoomEntity> GetList()
        {
            var _bookingList = _bookingMeetingRepository.SelectByConditions();

            return _bookingList.Select(booking => new BookingRoomEntity
            {
                Sn = booking.Sn,
                MeetingRoomId = booking.MeetingRoomId,
                MeetingRoom = booking.MeetingRoomId.GetDescription(),
                StartTime = booking.StartTime,
                StartTimeStr = booking.StartTime.ToString("yyyy-MM-dd"),
                EndTime = booking.EndTime,
                EndTimeStr = booking.EndTime.ToString("yyyy-MM-dd"),
                Name = booking.Name,
                Subject = booking.Subject,
                BackgroundColor = booking.BackgroundColor
            }).ToList();
        }

        public string Create(BookingRoomEntity bookingEntity)
        {
            string _createRes = "";

            if (string.IsNullOrEmpty(bookingEntity.Date?.Trim() ?? "") || string.IsNullOrEmpty(bookingEntity.Time?.Trim() ?? "") ||
                string.IsNullOrEmpty(bookingEntity.Name?.Trim() ?? "") || string.IsNullOrEmpty(bookingEntity.Subject?.Trim() ?? ""))
                return "欄位不可空白";

            if (!DateTime.TryParse($"{bookingEntity.Date} {bookingEntity.Time.Split("-")[0]}", out _) ||
                !DateTime.TryParse($"{bookingEntity.Date} {bookingEntity.Time.Split("-")[1]}", out _))
                return "預約時間異常";

            DateTime _startTime = DateTime.Parse($"{bookingEntity.Date} {bookingEntity.Time.Split("-")[0]}");
            DateTime _endTime = DateTime.Parse($"{bookingEntity.Date} {bookingEntity.Time.Split("-")[1]}");

            List<BookingMeetingDao> _insMeetingList = new List<BookingMeetingDao>();
            string _overlapMsg = "預約會議時間與以下重疊 \n";
            bool _isoverlap = false;

            if (bookingEntity.RepeatWeekly)
            {
                for (int i = 0; i < 13; i++)
                {
                    DateTime _tmpStartTime = _startTime.AddDays(i * 7);
                    DateTime _tmpEndTime = _endTime.AddDays(i * 7);

                    // 檢核會議重複
                    var _overlapMeeting = _bookingMeetingRepository.VerifyOverlap(bookingEntity.MeetingRoomId, _tmpStartTime, _tmpEndTime);
                    if (_overlapMeeting.Any())
                    {
                        _isoverlap = true;
                        foreach (var booking in _overlapMeeting)
                        {
                            _overlapMsg += $"議題:{booking.Subject}; 預約人:{booking.Name}; 時間:{booking.StartTime.ToString("MM/dd HH:mm")}-{booking.EndTime.ToString("HH:mm")} \n";
                        }
                        continue;
                    }
                    _insMeetingList.Add(new BookingMeetingDao
                    {
                        MeetingRoomId = bookingEntity.MeetingRoomId,
                        Name = bookingEntity.Name,
                        Subject = bookingEntity.Subject,
                        StartTime = _tmpStartTime,
                        EndTime = _tmpEndTime,
                        BackgroundColor = _roomBGCss[bookingEntity.MeetingRoomId]
                    });
                }
                if (_isoverlap)
                    return _overlapMsg;
            }
            else
            {
                // 檢核會議重複
                var _meetingOverlap = _bookingMeetingRepository.VerifyOverlap(bookingEntity.MeetingRoomId, _startTime, _endTime);
                if (_meetingOverlap.Any())
                {
                    foreach (var booking in _meetingOverlap)
                    {
                        _overlapMsg += $"議題: {booking.Subject}; 預約人: {booking.Name}; 時間: {booking.StartTime.ToString("HH:mm:ss")} - {booking.EndTime.ToString("HH:mm:ss")} \n";
                    }
                    return _overlapMsg;
                }
                _insMeetingList.Add(new BookingMeetingDao
                {
                    MeetingRoomId = bookingEntity.MeetingRoomId,
                    Name = bookingEntity.Name,
                    Subject = bookingEntity.Subject,
                    StartTime = _startTime,
                    EndTime = _endTime,
                    BackgroundColor = _roomBGCss[bookingEntity.MeetingRoomId]
                });
            }


            using (TransactionScope _scope = new TransactionScope())
            {
                var _insResult = _bookingMeetingRepository.Insert(_insMeetingList) == _insMeetingList.Count;

                if (_insResult)
                    _scope.Complete();
                else
                    _createRes = "預約會議異常";
            }

            return _createRes;
        }

        public string Update(BookingRoomEntity updBookingEntity)
        {
            string _createRes = "";

            if (updBookingEntity.Sn == 0)
                return "會議異常";

            var _booking = _bookingMeetingRepository.SelectByConditions(sn: updBookingEntity.Sn).FirstOrDefault();

            if (_booking == null)
                return "會議異常";

            _booking.StartTime = updBookingEntity.StartTime;
            _booking.EndTime = updBookingEntity.EndTime;

            // 檢核會議重複
            var _meetingOverlap = _bookingMeetingRepository.VerifyOverlap(_booking.MeetingRoomId, _booking.StartTime, _booking.EndTime).Where(w => w.Sn != updBookingEntity.Sn);
            if (_meetingOverlap.Any())
            {
                string _msg = "預約會議時間與以下重疊 \n";
                foreach (var booking in _meetingOverlap)
                {
                    _msg += $"議題: {booking.Subject}; 預約人: {booking.Name}; 時間: {booking.StartTime.ToString("HH:mm:ss")} - {booking.EndTime.ToString("HH:mm:ss")} \n";
                }
                return _msg;
            }

            using (TransactionScope _scope = new TransactionScope())
            {
                var _updResult = _bookingMeetingRepository.Update(_booking) == 1;

                if (_updResult)
                    _scope.Complete();
                else
                    _createRes = "更新會議異常";
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
