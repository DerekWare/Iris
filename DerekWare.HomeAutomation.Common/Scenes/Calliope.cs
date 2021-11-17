using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DerekWare.HomeAutomation.Common.Colors;

namespace DerekWare.HomeAutomation.Common.Scenes
{
    public class Calliope : Scene
    {
        readonly Random Random = new();

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

        public override IEnumerable<Color> GetPalette(IDevice targetDevice)
        {
            for(var i = 0; i < targetDevice.ZoneCount; ++i)
            {
                yield return new Color
                {
                    Hue = Random.NextDouble(),
                    Saturation = (Random.NextDouble() * (MaxSaturation - MinSaturation)) + MinSaturation,
                    Brightness = Brightness,
                    Kelvin = Kelvin
                };
            }
        }

        public override object Clone()
        {
            return MemberwiseClone();
        }
    }
}
