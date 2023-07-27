using MOD4.Web.DomainService.Entity;
using MOD4.Web.Enum;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MOD4.Web.DomainService
{
    public interface IBookingMeetingDomainService
    {
        List<CIMTestBookingEntity> GetList();

        string GetAnnouncement();

        string UpdateAnnouncement(string announcement);

        string GetFreeTimeRoom(MeetingRoomEnum roomId, string date);

        (List<CIMTestBookingEntity>, string) Create(CIMTestBookingEntity cimBookingEntity, UserEntity userEntity);

        (string, CIMTestBookingEntity) Update(CIMTestBookingEntity updBookingEntity, UserEntity userEntity);

        string Delete(int meetingSn);
    }
}
