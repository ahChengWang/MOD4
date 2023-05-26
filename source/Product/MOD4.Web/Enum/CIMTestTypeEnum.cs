using System.ComponentModel;

namespace MOD4.Web.Enum
{
    public enum CIMTestTypeEnum
    {
        [Description("LAB Test")]
        LABTest = 1,
        [Description("FAB Test")]
        FABTest = 2,
        [Description("Done")]
        Done = 3,
        [Description("Spec dev.")]
        SpecDev = 4,
        [Description("會議")]
        Meeting = 5,
    }
}
