using System.ComponentModel;

namespace MOD4.Web.Enum
{
    public enum MenuEnum
    {
        [Description("首頁")]
        Home = 1,
        [Description("CIM 測機排程")]
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
        //[Description("系統需求單")]
        //Demand = 11,
        [Description("進出申請單")]
        AccessFab = 13,
        [Description("個人簽核單")]
        AccessFabAudit = 14,
        [Description("帳號管理")]
        AccountManagement = 15,
        [Description("點燈記錄")]
        Extension = 17,
        [Description("SPC Online Chart")]
        SPCReport = 19,
        [Description("SPC參數設定")]
        SPCParaSetting = 20,
        [Description("系統需求單")]
        Demand = 21,
        [Description("權限申請單")]
        MESPermission = 22,
        [Description("MTD 量產")]
        MTDDashboard = 24,
        [Description("MTD 實驗")]
        MTDINT0 = 25,
        [Description("生產排程上傳")]
        Manufacture = 26,
        [Description("MTBF / MTTR")]
        MTBFMTTR = 27,
        [Description("2F生產排程")]
        MPS = 28,
        [Description("各部效率")]
        Efficiency = 29,
        [Description("各部效率-設定")]
        EffSetting = 30,
        [Description("SAP 管報")]
        Material = 33,
        [Description("SAP 檔案上傳")]
        MaterialUpload = 34,
        [Description("認證紀錄查詢")]
        CertificationPCES = 36,
        [Description("術科管理")]
        CertificationPCESSetting = 37,
        [Description("公告欄")]
        Bulletin = 39,
        [Description("Monitor")]
        Monitor = 40,
        [Description("Monitor TT")]
        MonitorTackTime = 41,
        [Description("參數設定")]
        MonitorSetting = 42,
    }
}
