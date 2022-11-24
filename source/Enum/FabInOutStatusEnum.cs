using System.ComponentModel;

namespace MOD4.Web.Enum
{
    public enum FabInOutStatusEnum
    {
        [Description("簽核中")]
        Processing = 1,

        [Description("已駁回")]
        Rejected = 2,

        [Description("已完成")]
        Completed = 3,

        [Description("已作廢")]
        Cancel = 4,

        [Description("核准")]
        Approve = 5,
    }
}
