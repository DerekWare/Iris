using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using DerekWare.HomeAutomation.Common.Colors;

namespace DerekWare.HomeAutomation.Common.Themes
{
    public class Spectrum : Theme
    {
        public enum ThemeDirection
        {
            Forward,
            Backward
        }

        readonly Random Random = new();

        [Browsable(false), XmlIgnore]
        public override bool IsDynamic => true;

        [Browsable(false), XmlIgnore]
        public override bool IsMultiZone => true;

        public double Brightness { get; set; } = 1;

        public ThemeDirection Direction { get; set; }

        public double Kelvin { get; set; } = 1;

        [Description("If true, the spectrum starts from a random point in the spectrum.")]
        public bool RandomOffset { get; set; }

        public double Saturation { get; set; } = 1;

        public override object Clone()
        {
            return MemberwiseClone();
        }

        public override IReadOnlyCollection<Color> GetPalette(IDevice targetDevice)
        {
            var colors = new List<Color>();
            var offset = RandomOffset ? Random.NextDouble() : 0;

            for(var i = 0; i < targetDevice.ZoneCount; ++i)
            {
                var hue = (double)i / targetDevice.ZoneCount;
                hue += offset;
                hue -= (int)hue;

                colors.Add(new Color { Hue = hue, Saturation = Saturation, Brightness = Brightness, Kelvin = Kelvin });
            }

            if(Direction == ThemeDirection.Backward)
            {
                colors.Reverse();
            }

            return colors;
        }
    }
}
