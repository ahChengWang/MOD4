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
        [Description("國定假日")]
        Holiday = 6,

        [Description("流程公告")]
        Announcement = 99,
    }
}
