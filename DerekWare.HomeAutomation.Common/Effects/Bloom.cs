using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common.Colors;
using Newtonsoft.Json;

namespace DerekWare.HomeAutomation.Common.Effects
{
    [Description("Randomly selects a zone and color and expands outward.")]
    public class Bloom : MultiZoneColorEffectRenderer
    {
        readonly Random Random = new();

        Color BloomColor;
        int CurrentCount;
        int TargetCount = -1;
        int ZoneIndex;

        [Browsable(false)]
        public double Kelvin => 1;

        [Browsable(false)]
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
                ZoneIndex = Random.Next(0, ZoneCount);
                TargetCount = Random.Next(3, ZoneCount);
                CurrentCount = 1;

                BloomColor = new Color(Random.NextDouble(),
                                       (Random.NextDouble() * (MaxSaturation - MinSaturation)) + MinSaturation,
                                       (Random.NextDouble() * (MaxBrightness - MinBrightness)) + MinBrightness,
                                       Kelvin);
            }

            var index = ZoneIndex - (CurrentCount / 2);

            for(var i = 0; i < CurrentCount; ++i)
            {
                colors.SetWrappingValue(index + i, BloomColor);
            }

            CurrentCount += 2;

            return true;
        }
    }
}
