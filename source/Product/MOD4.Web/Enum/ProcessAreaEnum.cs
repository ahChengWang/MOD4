using System;
using System.ComponentModel;

namespace MOD4.Web.Enum
{
    [Flags]
    public enum ProcessAreaEnum
    {
        [Description("All")]
        All = 1,

        [Description("Bonding")]
        Bonding = 2,

        [Description("LAM")]
        LAM = 4,

        [Description("ASSY")]
        ASSY = 8,

        [Description("CDP")]
        CDP = 16,

        [Description("RW")]
        RW = 32,

        [Description("Other")]
        Other = 64,
    }
}
