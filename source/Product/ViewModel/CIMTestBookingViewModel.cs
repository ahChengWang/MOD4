using MOD4.Web.Enum;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MOD4.Web.ViewModel
{
    public class CIMTestBookingViewModel
    {
        public int Sn { get; set; }

        [DisplayName("測試階段")]
        public CIMTestTypeEnum CIMTestTypeId { get; set; }

        public string CIMTestType { get; set; }

        [DisplayName("日期")]
        [Required(ErrorMessage = "必填")]
        public string Date { get; set; }

        [DisplayName("名字")]
        [Required(ErrorMessage = "必填")]
        public string Name { get; set; }

        [DisplayName("工號")]
        [Required(ErrorMessage = "必填")]
        public string JobId { get; set; }

        [DisplayName("議題")]
        [Required(ErrorMessage = "必填")]
        public string Subject { get; set; }

        [DisplayName("測試時段")]
        public CIMTestDayTypeEnum CIMTestDayTypeId { get; set; }

        public string CIMTestDayType { get; set; }

        [DisplayName("樓層")]
        public int FloorId { get; set; }

        public int StartYear { get; set; }
        public int StartMonth { get; set; }
        public int StartDay { get; set; }
        public int StartHour { get; set; }
        public int StartMinute { get; set; }
        public int StartSecond { get; set; }
        public int EndYear { get; set; }
        public int EndMonth { get; set; }
        public int EndDay { get; set; }
        public int EndHour { get; set; }
        public int EndMinute { get; set; }
        public int EndSecond { get; set; }
        public string BackgroundColor { get; set; }

    }
}