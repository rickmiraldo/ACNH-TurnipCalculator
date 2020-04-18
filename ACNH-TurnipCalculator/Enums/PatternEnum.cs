using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ACNH_TurnipCalculator.Enums
{
    public enum PatternEnum
    {
        [Description("Random")]
        Random,

        [Description("Large Spike")]
        LargeSpike,

        [Description("Decreasing")]
        Decreasing,

        [Description("Small Spike")]
        SmallSpike,

        [Description("I don't remember")]
        Unkown
    }
}
