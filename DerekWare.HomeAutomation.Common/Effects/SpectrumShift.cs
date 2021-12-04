using System;
using System.ComponentModel;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.Reflection;

namespace DerekWare.HomeAutomation.Common.Effects
{
    [Name("Spectrum Shift"),
     Description("Combines the Spectrum theme and the Move effect, but with more mathematically\n" +
                 "correct color values during motion than the Move effect can provide. This is\n" +
                 "particularly evident on groups or devices with fewer color zones, or 1 (where\n" +
                 "the Move effect can't do anything).")]
    public class SpectrumShift : MultiZoneColorEffectRenderer
    {
        public enum EffectBehavior
        {
            Forward,
            Backward,
            Bounce,
            Random
        }

        public enum EffectDirection
        {
            Forward,
            Backward
        }

        protected readonly Random Random = new();
        protected double ColorOffset;
        protected EffectDirection Direction;
        protected TimeSpan NextChange = TimeSpan.Zero;

        public SpectrumShift()
        {
            Duration = TimeSpan.FromSeconds(10);
            RefreshRate = TimeSpan.FromSeconds(0.5);
        }

        public virtual EffectBehavior Behavior { get; set; }
        public double Brightness { get; set; } = 1;
        public double Kelvin { get; set; } = 1;
        public double Saturation { get; set; } = 1;

        public override object Clone()
        {
            return Reflection.Clone(this);
        }

        protected override bool UpdateColors(RenderState renderState, ref Color[] colors, ref TimeSpan transitionDuration)
        {
            // Decide the direction of movement based on the effect behavior
            UpdateDirection(renderState);

            var cyclePosition = renderState.CyclePosition;

            // Using different algorithms for Random and the others because of possible drift
            // that isn't apparent in Random. Random keeps track of the current position within
            // the zones as a double (0-1) and increments or decrements based on the CycleIncrement
            // value in the render state.
            if(Behavior == EffectBehavior.Random)
            {
                var cycleIncrement = renderState.CycleIncrement * (Direction == EffectDirection.Forward ? 1 : -1);
                ColorOffset += cycleIncrement;

                if(ColorOffset >= 1)
                {
                    ColorOffset -= Math.Floor(ColorOffset);
                }
                else if(ColorOffset < 0)
                {
                    ColorOffset += Math.Floor(-ColorOffset) + 1;
                }

                cyclePosition = ColorOffset;
            }
            else if(Direction == EffectDirection.Forward)
            {
                cyclePosition = 1.0 - cyclePosition;
            }

            for(var i = 0; i < colors.Length; ++i)
            {
                var hue = cyclePosition + ((double)i / ZoneCount);

                if(hue >= 1)
                {
                    hue -= Math.Floor(hue);
                }
                else if(hue < 0)
                {
                    hue += Math.Floor(-hue) + 1;
                }

                colors[i] = new Color(hue, Saturation, Brightness, Kelvin);
            }

            return true;
        }

        protected virtual void UpdateDirection(RenderState renderState)
        {
            // Decide the direction of movement based on the effect behavior
            switch(Behavior)
            {
                case EffectBehavior.Forward:
                    Direction = EffectDirection.Forward;
                    break;

                case EffectBehavior.Backward:
                    Direction = EffectDirection.Backward;
                    break;

                case EffectBehavior.Bounce:
                    // Every cycle, reverse the direction
                    if(renderState.CycleCountChanged)
                    {
                        Direction = Direction == EffectDirection.Forward ? EffectDirection.Backward : EffectDirection.Forward;
                    }

                    break;

                case EffectBehavior.Random:
                    // If we passed the randomly chosen position, reverse direction
                    if(renderState.TotalElapsed >= NextChange)
                    {
                        NextChange += TimeSpan.FromSeconds(Duration.TotalSeconds * Random.NextDouble());
                        Direction = Direction == EffectDirection.Forward ? EffectDirection.Backward : EffectDirection.Forward;
                    }

                    break;

                default:
                    throw new NotSupportedException();
            }
        }
    }
}
