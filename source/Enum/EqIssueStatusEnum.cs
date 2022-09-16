using System.ComponentModel;

namespace MOD4.Web.Enum
{
    public enum EqIssueStatusEnum
    {
        [Description("待PM處理")]
        PendingPM = 1,

        [Description("待工程師處理")]
        PendingENG = 2,

        [Description("完成")]
        Complete = 3,
    }
}
