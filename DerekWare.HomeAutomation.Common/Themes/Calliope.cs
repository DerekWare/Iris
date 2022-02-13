using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DerekWare.HomeAutomation.Common;

namespace DerekWare.HomeAutomation.Common.Themes
{
    public class Calliope : Theme
    {
        [Browsable(false)]
        public override bool IsDynamic => true;

        [Browsable(false)]
        public override bool IsMultiZone => true;

        [Range(typeof(double), "0", "1")]
        public double Brightness { get; set; } = 1;

        [Range(typeof(double), "0", "1")]
        public double Kelvin { get; set; } = 1;

        [Range(typeof(double), "0", "1")]
        public double MaxSaturation { get; set; } = 1;

        [Range(typeof(double), "0", "1")]
        public double MinSaturation { get; set; } = 0.25;

        public override object Clone()
        {
            return Reflection.Clone(this);
        }

        public override IReadOnlyCollection<Color> GetPalette(IDevice targetDevice)
        {
            var palette = new Color[targetDevice.ZoneCount];

            for(var i = 0; i < targetDevice.ZoneCount; ++i)
            {
                palette[i] = new Color
                {
                    Hue = Random.GetDouble(), Saturation = Random.GetDouble(MinSaturation, MaxSaturation), Brightness = Brightness, Kelvin = Kelvin
                };
            }

            return palette;
        }
    }
}
