using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MOD4.Web.DomainService
{
    public interface IBookingMeetingDomainService
    {
        List<CIMTestBookingEntity> GetList();

        string GetFreeTimeRoom(MeetingRoomEnum roomId, string date);

        string Create(CIMTestBookingEntity cimBookingEntity);

        string Update(CIMTestBookingEntity updBookingEntity);

        string Delete(int meetingSn);
    }
}
