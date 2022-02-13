using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DerekWare.HomeAutomation.Common.Effects
{
    [Description("Ramps the brightness of a device or group over time, preserving the colors.")]
    public class Brightness : MultiZoneColorEffectRenderer
    {
        [Range(0.0, 1.0)]
        public double BeginningBrightness { get; set; } = 0;

        [Range(0.0, 1.0)]
        public double EndingBrightness { get; set; } = 1;

        public override object Clone()
        {
            return MemberwiseClone();
        }

        protected override bool UpdateColors(RenderState renderState, ref Color[] colors, ref TimeSpan transitionDuration)
        {
            // Only run for one cycle
            if(renderState.CycleCount > 0)
            {
                colors = colors.Select(color => new Color(color) { Brightness = EndingBrightness }).ToArray();
                Task.Run(Stop);
                return true;
            }

            var brightness = (renderState.CyclePosition * (EndingBrightness - BeginningBrightness)) + BeginningBrightness;
            colors = colors.Select(color => new Color(color) { Brightness = brightness }).ToArray();
            return true;
        }
    }
}
