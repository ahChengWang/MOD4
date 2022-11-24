using System.ComponentModel;

namespace MOD4.Web.Enum
{
    public enum FabInOutFlowEnum
    {
        [Description("一級主管")]
        TopLevel = 1,

        [Description("二級主管")]
        SecondLevel = 2,

        [Description("三級主管")]
        ThirdLevel = 3,
    }
}
