using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using DerekWare.HomeAutomation.Common.Colors;
using Newtonsoft.Json;

namespace DerekWare.HomeAutomation.Common.Themes
{
    [Serializable, JsonObject]
    public class Spectrum : Theme, ISerializable
    {
        public enum SpectrumDirection
        {
            Forward,
            Backward
        }

        readonly Random Random = new();

        public Spectrum()
        {
        }

        public Spectrum(SerializationInfo info, StreamingContext context)
        {
            this.Deserialize(info, context);
        }

        [Browsable(false)]
        public override bool IsDynamic => true;

        [Browsable(false)]
        public override bool IsMultiZone => true;

        public double Brightness { get; set; } = 1;

        public SpectrumDirection Direction { get; set; }

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

            if(Direction == SpectrumDirection.Backward)
            {
                colors.Reverse();
            }

            return colors;
        }

        #region ISerializable

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            this.Serialize(info, context);
        }

        #endregion
    }
}
