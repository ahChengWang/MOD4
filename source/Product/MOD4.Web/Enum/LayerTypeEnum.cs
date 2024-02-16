using System.ComponentModel;

namespace MOD4.Web.Enum
{
    public enum LayerTypeEnum
    {
        [Description("柱位圖")]
        PillarPlace = 1,

        [Description("內部隔間圖")]
        InternalPart= 2,

        [Description("設備外框圖")]
        EqFrame = 3,

        [Description("設備外框含周邊設備圖")]
        EqFrameSurroundEq = 4,

        [Description("設備細部含周邊設備圖")]
        EqDetailSurroundEq = 5,
    }
}
