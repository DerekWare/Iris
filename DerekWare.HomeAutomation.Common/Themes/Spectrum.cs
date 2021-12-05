using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DerekWare.HomeAutomation.Common.Colors;

namespace DerekWare.HomeAutomation.Common.Themes
{
    public class Spectrum : Theme
    {
        public enum EffectDirection
        {
            Forward,
            Backward
        }

        readonly Random Random = new();

        [Browsable(false)]
        public override bool IsDynamic => true;

        [Browsable(false)]
        public override bool IsMultiZone => true;

        [Range(0.0, 1.0)]
        public double Brightness { get; set; } = 1;

        public EffectDirection Direction { get; set; }

        [Range(0.0, 1.0)]
        public double Kelvin { get; set; } = 1;

        [Range(-1.0, 1.0), Description("Start the spectrum from a specific point in the spectrum.")]
        public double Offset { get; set; }

        [Description("Start the spectrum from a random point in the spectrum.")]
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

        public override IReadOnlyCollection<Color> GetPalette(IDevice targetDevice)
        {
            var colors = new List<Color>();
            var offset = RandomOffset ? Random.NextDouble() : Offset;

            for(var i = 0; i < targetDevice.ZoneCount; ++i)
            {
                var hue = ((double)i / targetDevice.ZoneCount) * Window;
                hue += offset;

                switch(hue)
                {
                    case >= 1:
                        hue -= Math.Floor(hue);
                        break;
                    case < 0:
                        hue += Math.Floor(-hue) + 1;
                        break;
                }

                colors.Add(new Color { Hue = hue, Saturation = Saturation, Brightness = Brightness, Kelvin = Kelvin });
            }

            if(Direction == EffectDirection.Backward)
            {
                colors.Reverse();
            }

            return colors;
        }
    }
}
