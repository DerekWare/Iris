using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common.Colors;

namespace DerekWare.HomeAutomation.Common.Effects
{
    [Description("Expando picks a random zone, fills it with a random color and expands until it takes up the whole lightstrip.")]
    public class Expando : MultiZoneColorEffectRenderer
    {
        static readonly Random Random = new();

        Color[] Colors;
        int CycleCount = -1;
        Color NextColor;
        int ZoneIndex = -1;

        public double Kelvin => 1;

        [Range(0.0, 1.0)]
        public double MaxBrightness { get; set; } = 1;

        [Range(0.0, 1.0)]
        public double MaxSaturation { get; set; } = 1;

        [Range(0.0, 1.0)]
        public double MinBrightness { get; set; } = 1;

        [Range(0.0, 1.0)]
        public double MinSaturation { get; set; } = 1;

        public override object Clone()
        {
            return MemberwiseClone();
        }

        protected override bool GetColors(RenderState renderState, out IReadOnlyCollection<Color> colors)
        {
            if(renderState.CycleCount != CycleCount)
            {
                CycleCount = renderState.CycleCount;

                // Ensure the color palette is full of the previous color so we don't leave any
                // leftover fragments when we transition.
                if(NextColor is not null && !Colors.IsNullOrEmpty())
                {
                    for(var i = 0; i < ZoneCount; ++i)
                    {
                        Colors[i] = NextColor;
                    }
                }

                // Pick a new random zone
                ZoneIndex = Random.Next(0, ZoneCount);

                // Pick a random color
                NextColor = new Color(Random.NextDouble(),
                                      (Random.NextDouble() * (MaxSaturation - MinSaturation)) + MinSaturation,
                                      (Random.NextDouble() * (MaxBrightness - MinBrightness)) + MinBrightness,
                                      Kelvin);
            }

            // Fill zones starting and ZoneIndex and expanding outward
            Colors ??= Palette.ToArray();
            var count = ((int)(renderState.CyclePosition * ZoneCount) / 2) * 2;
            var offset = ZoneIndex - (count / 2);

            for(var i = 0; i < count; ++i)
            {
                if(offset >= ZoneCount)
                {
                    offset -= ZoneCount;
                }
                else if(offset < 0)
                {
                    offset += ZoneCount;
                }

                Colors[offset] = NextColor;
                offset++;
            }

            colors = Colors;
            return true;
        }

        protected override TimeSpan ValidateRefreshRate()
        {
            var refreshRate = base.ValidateRefreshRate().TotalSeconds;
            refreshRate = Math.Max(refreshRate, 1.0 / ZoneCount);
            refreshRate = Math.Max(refreshRate, 1.0 / Duration.TotalSeconds);
            return TimeSpan.FromSeconds(refreshRate);
        }
    }
}
