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
        public enum MeteorTailStyle
        {
            Linear,
            Exponential
        }

        enum MeteorDirection
        {
            Backward = -1,
            Forward = 1
        }

        const double MaxHue = 1;
        const double MinHue = 0;

        [Range(1, 10)]
        public int MinCount = 1;

        MeteorDirection Direction;
        double Elapsed;
        MeteorContext[] Meteors;
        TimeSpan NextTime;

        public MeteorShower()
        {
            Duration = TimeSpan.FromSeconds(2);
            MinFrequency = TimeSpan.FromSeconds(1);
            MaxFrequency = TimeSpan.FromSeconds(30);
            RefreshRate = TimeSpan.Zero;
        }

        [Description("The speed at which the meteors travel across your device.")]
        public new TimeSpan Duration { get => base.Duration; set => base.Duration = value; }

        [Range(0.0, 1.0)]
        public double MaxBrightness { get; set; } = 1;

        [Range(1, 10)]
        public int MaxCount { get; set; } = 10;

        [Range(1, 100)]
        public int MaxDistance { get; set; } = 50;

        [Description("The maximum time between showers.")]
        public TimeSpan MaxFrequency { get; set; }

        [Range(1, 100)]
        public int MaxLength { get; set; } = 25;

        [Range(0.0, 1.0)]
        public double MaxSaturation { get; set; } = 1;

        [Range(0.0, 1.0)]
        public double MinBrightness { get; set; } = 0.25;

        [Range(1, 100)]
        public int MinDistance { get; set; } = 1;

        [Description("The minimum time between showers.")]
        public TimeSpan MinFrequency { get; set; }

        [Range(1, 100)]
        public int MinLength { get; set; } = 3;

        [Range(0.0, 1.0)]
        public double MinSaturation { get; set; } = 0.25;

        public MeteorTailStyle TailStyle { get; set; }

        public override object Clone()
        {
            return Reflection.Clone(this);
        }

        protected override bool UpdateColors(RenderState renderState, ref Color[] colors, ref TimeSpan transitionDuration)
        {
            transitionDuration = TimeSpan.Zero;

            if(renderState.TotalElapsed < NextTime)
            {
                colors = Colors.Colors.Black.Repeat(ZoneCount).ToArray();
                return true;
            }

            // Create new meteors and pick the direction of motion
            if(Meteors is null)
            {
                Meteors = CreateMeteors().ToArray();
                Direction = Random.GetEnumValue<MeteorDirection>();
                Elapsed = 0;
            }

            // Imagine an offscreen drawing surface that includes all the visible color zones, 
            // plus extra padding zones on either side to accomodate the parts of the meteors
            // that aren't visible as they fly across the zones. meteorLength represents the second
            // value.
            var meteorLength = Meteors[Meteors.Length - 1].Offset + Meteors[Meteors.Length - 1].Length;

            // totalLength represents the both the visible and offscreen zones
            var totalLength = ZoneCount + (meteorLength * 2);

            // Create a color array that represents all of the visible and offscreen zones,
            // initialized to black.
            colors = Colors.Colors.Black.Repeat(totalLength).ToArray();

            // offset represents where in the total zones the meteors are currently positioned
            Elapsed += renderState.CycleIncrement;
            var position = Elapsed * totalLength;
            var offset = Math.Min((int)Math.Round(position), meteorLength + ZoneCount);

            // Copy the meteor colors into the larger color array
            foreach(var meteor in Meteors)
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

            // Reset next pass?
            if(offset >= (meteorLength + ZoneCount))
            {
                Meteors = null;
                NextTime = renderState.TotalElapsed + Random.GetTimeSpan(MinFrequency, MaxFrequency);
            }

            return true;
        }

        MeteorContext CreateMeteor()
        {
            return new MeteorContext(
                new Color(Random.GetDouble(MinHue, MaxHue), Random.GetDouble(MinSaturation, MaxSaturation), Random.GetDouble(MinBrightness, MaxBrightness), 1),
                (int)Math.Round(Random.GetDouble(MinLength, MaxLength)),
                TailStyle);
        }

        IEnumerable<MeteorContext> CreateMeteors()
        {
            var count = (int)Math.Round(Random.GetDouble(MinCount, MaxCount));
            var offset = 0;

            for(var i = 0; i < count; ++i)
            {
                var c = CreateMeteor();
                c.Offset = offset;
                offset += c.Length + (int)Math.Round(Random.GetDouble(MinDistance, MaxDistance));
                yield return c;
            }
        }

        class MeteorContext
        {
            public readonly Color[] Colors;
            public int Offset;

            public MeteorContext(Color color, int length, MeteorTailStyle tailStyle)
            {
                // Brightness decreases right to left, so the tail ends at 0
                Colors = color.Repeat(length).Select(i => i.Clone()).ToArray();

                switch(tailStyle)
                {
                    case MeteorTailStyle.Linear:
                    {
                        for(var i = 0; i < length; ++i)
                        {
                            var m = (i + 1) / (double)length;
                            Colors[i].Brightness *= m;
                        }

                        break;
                    }

                    case MeteorTailStyle.Exponential:
                    {
                        var m = 1.0;

                        for(var i = length - 1; i >= 0; --i)
                        {
                            Colors[i].Brightness *= m;
                            m /= 2;
                        }

                        break;
                    }

                    default:
                        throw new ArgumentOutOfRangeException(nameof(tailStyle), tailStyle, null);
                }
            }

            public int Length => Colors.Length;
        }
    }
}
