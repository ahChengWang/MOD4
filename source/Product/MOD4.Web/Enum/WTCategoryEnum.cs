using System;
using System.ComponentModel;

namespace MOD4.Web.Enum
{
    [Flags]
    public enum WTCategoryEnum
    {
        [Description("前段日班")]
        FrontA = 1,

        [Description("前段夜班")]
        FrontB = 2,

        [Description("後段日班")]
        BackendA = 4,

        [Description("後段夜班")]
        BackendB = 8,
    }
}
