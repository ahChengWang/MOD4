using MOD4.Web.Enum;
using System;

namespace MOD4.Web.DomainService.Entity
{
    public class CIMTestBookingEntity
    {
        public int Sn { get; set; }

        public CIMTestTypeEnum CIMTestTypeId { get; set; }

        public string Date { get; set; }

        public string Name { get; set; }

        public string JobId { get; set; }

        public string Subject { get; set; }

        public CIMTestDayTypeEnum CIMTestDayTypeId { get; set; }

        public int Floor { get; set; }

        public int Days { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string BackgroundColor { get; set; }

        public string Remark { get; set; }
    }
}
