using MOD4.Web.Enum;
using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MOD4.Web.ViewModel
{
    public class BaseAuditHistoryViewModel
    {
        public int AuditSn { get; set; }

        public int AuditAccountSn { get; set; }

        [Display(Name = "簽核人員")]
        public string AuditAccountName { get; set; }

        [Display(Name = "狀態")]
        public string Status { get; set; }

        [Display(Name = "接收時間")]
        public string ReceivedTimeStr { get; set; }

        public DateTime AuditTime { get; set; }

        [Display(Name = "簽核時間")]
        public string AuditTimeStr { get; set; }

        [Display(Name = "花費時間")]
        public string Duration { get; set; }

        [Display(Name = "備註")]
        public string AuditRemark { get; set; }

    }
}