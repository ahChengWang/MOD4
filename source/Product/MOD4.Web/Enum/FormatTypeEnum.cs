using System;
using System.ComponentModel;

namespace MOD4.Web.Enum
{
    [Flags]
    public enum FormatTypeEnum
    {
        [Description("紙本")]
        HardCopy = 1,

        [Description("PDF檔")]
        PDF = 2,

        [Description("CAD檔")]
        CAD = 4,
    }
}
