using System.ComponentModel;

namespace MOD4.Web.Enum
{
    public enum DemandStatusEnum
    {
        // ===== 共用狀態 =====
        [Description("待評估")]
        Pending = 1,

        [Description("已退件")]
        Rejected = 2,

        [Description("已完成")]
        Completed = 4,

        // ===== 系統需求單狀態 =====
        [Description("進行中")]
        Processing = 3,

        [Description("已取消")]
        Cancel = 5,

        [Description("待確認")]
        Verify = 6,

        // ===== MES 權限申請狀態 =====
        [Description("簽核中")]
        Signing = 7,

        [Description("核准")]
        Approve = 8,

        [Description("待設定")]
        Setting = 9,
    }
}
