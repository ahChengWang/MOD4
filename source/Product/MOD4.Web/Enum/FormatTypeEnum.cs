using System;
using System.ComponentModel;

namespace MOD4.Web.Enum
{
    [Flags]
    public enum FormatTypeEnum
    {
        [Description("PDF檔")]
        PDF = 1,

        [Description("CAD檔")]
        CAD = 2,
    }
}
