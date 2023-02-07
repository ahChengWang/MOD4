using System.ComponentModel;

namespace MOD4.Web.Enum
{
    public enum MenuEnum
    {
        [Description("首頁")]
        Home = 1,
        [Description("會議室預約")]
        BookingMeeting = 3,
        [Description("分時產出")]
        Performance = 5,
        [Description("TT設定")]
        PerformanceSetting = 6,
        [Description("看板線體")]
        SettingLineEquipment = 8,
        [Description("即時機況")]
        Equipment = 9,
        [Description("機況看板")]
        EquipmentDashboard = 10,
        [Description("系統需求單")]
        Demand = 11,
        [Description("進出申請單")]
        AccessFab = 13,
        [Description("個人簽核單")]
        AccessFabAudit = 14,
        [Description("帳號管理")]
        AccountManagement = 15,
        [Description("擴充功能")]
        Extension = 16,
    }
}
