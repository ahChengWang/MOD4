using MOD4.Web.Enum;
using System;

namespace MOD4.Web.Repostory.Dao
{
    public class CIMTestBookingDao
    {
        public int Sn { get; set; }

        public CIMTestTypeEnum CIMTestTypeId { get; set; }

        public string Date { get; set; }

        public string Name { get; set; }

        public string JobId { get; set; }

        public string Subject { get; set; }

        public int Floor { get; set; }

        public CIMTestDayTypeEnum CIMTestDayTypeId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string BackgroundColor { get; set; }
    }
}
