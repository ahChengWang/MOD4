using System.ComponentModel;

namespace MOD4.Web.ViewModel
{
    public class EqEmsDetailViewModel
    {
        [DisplayName("EQUIP No.")]
        public string EQUIPNo { get; set; }

        [DisplayName("Start Time")]
        public string StartTime { get; set; }

        [DisplayName("Close Time")]
        public bool CloseTime { get; set; }

        [DisplayName("Entity Status")]
        public bool EntityStatus { get; set; }

        [DisplayName("EQ Status")]
        public bool EQStatus { get; set; }

        [DisplayName("Stay Time")]
        public bool StayTime { get; set; }

        [DisplayName("Code")]
        public bool Code { get; set; }

        [DisplayName("Status Desc.")]
        public bool StatusDesc { get; set; }

        [DisplayName("Operator")]
        public bool Operator { get; set; }

        [DisplayName("Comment")]
        public bool Comment { get; set; }
    }
}
