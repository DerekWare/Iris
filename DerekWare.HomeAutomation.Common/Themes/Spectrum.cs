using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DerekWare.HomeAutomation.Common.Colors;

namespace DerekWare.HomeAutomation.Common.Themes
{
    public class Spectrum : Theme
    {
        readonly Random Random = new();

        [Browsable(false)]
        public override bool IsDynamic => true;

        [Browsable(false)]
        public override bool IsMultiZone => true;

        [Range(0.0, 1.0)]
        public double Brightness { get; set; } = 1;

        [DefaultValue(Common.Direction.Forward)]
        public Direction Direction { get; set; } = Direction.Forward;

        [Range(0.0, 1.0)]
        public double Kelvin { get; set; } = 1;

        [Range(-1.0, 1.0), Description("Start the colors from a specific point in the spectrum.")]
        public double Offset { get; set; }

        [Description("Start the colors from a random point in the spectrum.")]
        public bool RandomOffset { get; set; }

        [Range(0.0, 1.0)]
        public double Saturation { get; set; } = 1;

        [Description(
             "The window size is how much of the visible spectrum to show at once. A window size of 1 will show all of it. A window size of 0 will show a single color. A window size of 0.5 will show half of the spectrum at one time."),
         Range(0.0, 1.0)]
        public double Window { get; set; } = 1;

        public override object Clone()
        {
            return Reflection.Clone(this);
        }

        public Color[] GetPalette(int count)
        {
            var offset = RandomOffset ? Random.NextDouble() : Offset;

            if(Window <= 0)
            {
                count = 1;
            }

            var colors = new Color[count];

            for(var i = 0; i < count; ++i)
            {
                var hue = (offset + (((double)i / count) * Window)) % 1.0;

                if(hue < 0)
                {
                    hue = 1.0 - hue;
                }

                colors[i] = new Color(hue, Saturation, Brightness, Kelvin);
            }

            if(Direction == Direction.Backward)
            {
                Array.Reverse(colors);
            }

            return colors;
        }

        public override IReadOnlyCollection<Color> GetPalette(IDevice targetDevice)
        {
            return GetPalette(targetDevice.ZoneCount);
        }
    }
}
