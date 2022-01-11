using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.Reflection;

namespace DerekWare.HomeAutomation.Common.Effects
{
    [Name("Meteor Shower"), Description("Occasionally shoots one or more meteors across your device(s). This effect is meant for lightstrips.")]
    public class MeteorShower : MultiZoneColorEffectRenderer
    {
        enum MeteorDirection
        {
            Forward = 1,
            Backward = -1
        }

        const double MaxHue = 1;
        const double MinHue = 0;

        [Range(1, 10)]
        public int MinCount = 1;

        readonly Random Random = new();
        MeteorDirection Direction;
        MeteorContext[] MeteorContexts;

        TimeSpan NextTime = TimeSpan.MinValue;
        int Offset;

        public MeteorShower()
        {
            Duration = TimeSpan.FromSeconds(30);
            RefreshRate = TimeSpan.FromMilliseconds(50);
        }

        [Description("The maximum time between meteors.")]
        public override TimeSpan Duration { get => base.Duration; set => base.Duration = value; }

        [Range(0.0, 1.0)]
        public double MaxBrightness { get; set; } = 1;

        [Range(1, 10)]
        public int MaxCount { get; set; } = 10;

        [Range(1, 100)]
        public int MaxDistance { get; set; } = 50;

        [Range(1, 100)]
        public int MaxLength { get; set; } = 25;

        [Range(0.0, 1.0)]
        public double MaxSaturation { get; set; } = 1;

        [Range(0.0, 1.0)]
        public double MinBrightness { get; set; } = 0.25;

        [Range(1, 100)]
        public int MinDistance { get; set; } = 1;

        [Range(1, 100)]
        public int MinLength { get; set; } = 5;

        [Range(0.0, 1.0)]
        public double MinSaturation { get; set; } = 0.25;

        public override object Clone()
        {
            return Reflection.Clone(this);
        }

        protected override bool UpdateColors(RenderState renderState, ref Color[] colors, ref TimeSpan transitionDuration)
        {
            if(renderState.TotalElapsed < NextTime)
            {
                colors = Colors.Colors.Black.Repeat(ZoneCount).ToArray();
                return true;
            }

            // Create new meteors and pick the direction of motion
            if(MeteorContexts is null)
            {
                MeteorContexts = CreateContexts().ToArray();
                Direction = Random.NextDouble() >= 0.5 ? MeteorDirection.Forward : MeteorDirection.Backward;
                Offset = 0;
            }

            // Imagine an offscreen drawing surface that includes all the visible color zones, 
            // plus extra padding zones on either side to accomodate the parts of the meteors
            // that aren't visible as they fly across the zones. meteorLength represents the second
            // value.
            var meteorLength = MeteorContexts[MeteorContexts.Length - 1].Offset + MeteorContexts[MeteorContexts.Length - 1].Length;

            // totalLength represents the both the visible and offscreen zones
            var totalLength = ZoneCount + (meteorLength * 2);

            // Create a color array that represents all of the visible and offscreen zones,
            // initialized to black.
            colors = Colors.Colors.Black.Repeat(totalLength).ToArray();

            // meteorOffset represents where in the total zones the meteors are currently positioned
            var offset = Offset++;

            // Copy the meteor colors into the larger color array
            foreach(var meteor in MeteorContexts)
            {
                Array.Copy(meteor.Colors, 0, colors, offset + meteor.Offset, meteor.Length);
            }

            // Trim the colors to the zone count and reverse based on direction
            var meteorColors = colors.Skip(meteorLength).Take(ZoneCount);

            if(Direction == MeteorDirection.Backward)
            {
                meteorColors = meteorColors.Reverse();
            }

            colors = meteorColors.ToArray();
            transitionDuration = TimeSpan.Zero;

            // Reset next pass?
            if(Offset >= (meteorLength + ZoneCount))
            {
                MeteorContexts = null;
                NextTime = renderState.TotalElapsed + TimeSpan.FromSeconds(GetRandomValue(0, Duration.TotalSeconds));
            }

            return true;
        }

        MeteorContext CreateContext()
        {
            return new MeteorContext(
                new Color(GetRandomValue(MinHue, MaxHue), GetRandomValue(MinSaturation, MaxSaturation), GetRandomValue(MinBrightness, MaxBrightness), 1),
                (int)Math.Round(GetRandomValue(MinLength, MaxLength)));
        }

        IEnumerable<MeteorContext> CreateContexts()
        {
            var count = (int)Math.Round(GetRandomValue(MinCount, MaxCount));
            var offset = 0;

            for(var i = 0; i < count; ++i)
            {
                var c = CreateContext();
                c.Offset = offset;
                offset += c.Length + (int)Math.Round(GetRandomValue(MinDistance, MaxDistance));
                yield return c;
            }
        }

        double GetRandomValue(double min, double max)
        {
            return (Random.NextDouble() * (max - min)) + min;
        }

        class MeteorContext
        {
            public readonly Color[] Colors;
            public int Offset;

            public MeteorContext(Color color, int length)
            {
                // Note that these colors are right to left, so the tail is at 0.
                Colors = new Color[length];

                for(var i = 0; i < length; ++i)
                {
                    var m = (i * 1) / (double)length;
                    Colors[i] = new Color(color.Hue, color.Saturation, color.Brightness * m, color.Kelvin);
                }
            }

            public int Length => Colors.Length;
        }
    }
}
