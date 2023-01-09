using MOD4.Web.DomainService.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MOD4.Web.DomainService
{
    public interface IBookingMeetingDomainService
    {
        List<BookingRoomEntity> GetList();

        string Create(BookingRoomEntity bookingEntity);

        string Update(BookingRoomEntity updBookingEntity);

        string Delete(int meetingSn);
    }
}
