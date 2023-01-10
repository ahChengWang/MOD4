using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MOD4.Web.DomainService
{
    public interface IBookingMeetingDomainService
    {
        List<BookingRoomEntity> GetList();

        string GetFreeTimeRoom(MeetingRoomEnum roomId, string date);

        string Create(BookingRoomEntity bookingEntity);

        string Update(BookingRoomEntity updBookingEntity);

        string Delete(int meetingSn);
    }
}
