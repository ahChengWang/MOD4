using System;
using System.ComponentModel;

namespace MOD4.Web.Enum
{
    [Flags]
    public enum FactoryFloorEnum
    {
        [Description("CarUX 2F")]
        SecFloor = 1,

        [Description("CarUX 3F")]
        ThirdFloor = 2,
    }
}
