using System.ComponentModel;

namespace MOD4.Web.Enum
{
    public enum DefectCategoryEnum
    {
        [Description("破片")]
        Broken = 1,

        [Description("貼合")]
        Lam = 2,

        [Description("BOND")]
        Bond = 3,

        [Description("偏光板")]
        Polarizer = 4,
    }
}
