using System.ComponentModel;

namespace MOD4.Web.Enum
{
    public enum MESPermissionEnum
    {
        [Description("BOND")]
        MFG_BOND = 1,

        [Description("LAM")]
        MFG_LAM = 2,

        [Description("ASSY")]
        MFG_ASSY = 3,

        [Description("CDP")]
        MFG_CDP = 4,

        [Description("RW")]
        MFG_RW = 5,

        [Description("組長")]
        TeamLead = 11,

        [Description("領班")]
        Foreman = 12,

        [Description("NPE")]
        NPE = 21,

        [Description("INT")]
        INT = 22,

        [Description("FA")]
        FA = 23,

        [Description("D2檢")]
        D2Chk = 24,

        [Description("BOND")]
        BOND = 31,

        [Description("LAM")]
        LAM = 32,

        [Description("測試")]
        Test = 33,

        [Description("BOND-MQC")]
        BOND_MQC = 34,

        [Description("LAM-MQC")]
        LAM_MQC = 35,

        [Description("OQC作業")]
        OperationOQC = 41,

        [Description("OQC設定")]
        SettingOQC = 42,

        [Description("工單")]
        WorkOrder = 51,

        [Description("戰情小PC")]
        MiniPC = 52,

        [Description("生產計劃")]
        ProductionPlan = 61,
    }
}
