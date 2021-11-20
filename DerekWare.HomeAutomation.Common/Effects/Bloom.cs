using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using DerekWare.HomeAutomation.Common.Colors;

namespace DerekWare.HomeAutomation.Common.Effects
{
    public class Bloom : MultiZoneColorEffectRenderer
    {
        readonly Random Random = new();

        Color BloomColor;
        int CurrentCount;
        int TargetCount = -1;
        int ZoneIndex;

        [Browsable(false), XmlIgnore]
        public double Kelvin => 1;

        [Browsable(false), XmlIgnore]
        public override TimeSpan Duration { get => RefreshRate; set { } }

        [Range(0.0, 1.0)]
        public double MaxBrightness { get; set; } = 1;

        [Range(0.0, 1.0)]
        public double MaxSaturation { get; set; } = 1;

        [Range(0.0, 1.0)]
        public double MinBrightness { get; set; } = 1;

        [Range(0.0, 1.0)]
        public double MinSaturation { get; set; } = 0.25;

        public override object Clone()
        {
            return MemberwiseClone();
        }

        protected override bool UpdateColors(RenderState renderState, ref Color[] colors)
        {
            // At the start of a cycle, pick a random zone and color, then expand out
            // a random number of zones.
            if(CurrentCount > TargetCount)
            {
                ZoneIndex = Random.Next(0, colors.Length);
                TargetCount = Random.Next(3, colors.Length);
                CurrentCount = 1;

                BloomColor = new Color(Random.NextDouble(),
                                       (Random.NextDouble() * (MaxSaturation - MinSaturation)) + MinSaturation,
                                       (Random.NextDouble() * (MaxBrightness - MinBrightness)) + MinBrightness,
                                       Kelvin);
            }

            var index = ZoneIndex - (CurrentCount / 2);

            while(index < 0)
            {
                index += colors.Length;
            }

            for(var i = 0; i < CurrentCount; ++i)
            {
                colors[index % colors.Length] = BloomColor;
                ++index;
            }

            CurrentCount += 2;

            return true;
        }
    }
}
