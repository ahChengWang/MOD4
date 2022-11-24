using System.ComponentModel;

namespace MOD4.Web.Enum
{
    public enum FabInTypeEnum
    {
        [Description("參觀")]
        Visit = 1,

        [Description("稽核")]
        Audit = 2,

        [Description("MOVE IN")]
        MoveIn = 3,

        [Description("其他")]
        Other = 4,
    }
}
