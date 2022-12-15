using System.ComponentModel;

namespace MOD4.Web.Enum
{
    public enum DemandStatusEnum
    {
        [Description("待評估")]
        Pending = 1,

        [Description("已退件")]
        Rejected = 2,

        [Description("進行中")]
        Processing = 3,

        [Description("已完成")]
        Completed = 4,

        [Description("已取消")]
        Cancel = 5,

        [Description("待確認")]
        Verify = 6,
    }
}
