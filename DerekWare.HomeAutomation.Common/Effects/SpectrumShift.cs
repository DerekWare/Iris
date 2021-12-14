using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.Reflection;

namespace DerekWare.HomeAutomation.Common.Effects
{
    [Name("Spectrum Shift"),
     Description(
         "Combines the Spectrum theme and the Move effect, but with more mathematically correct color values during motion than the Move effect can provide. This is particularly evident on groups or devices with fewer color zones, or 1 (where the Move effect can't do anything).")]
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

        [Range(0.0, 1.0)]
        public double Brightness { get; set; } = 1;

        [Range(0.0, 1.0)]
        public double Kelvin { get; set; } = 1;

        [Range(-1.0, 1.0), Description("Start the spectrum from a specific point in the spectrum.")]
        public double Offset { get; set; }

        [Range(0.0, 1.0)]
        public double Saturation { get; set; } = 1;

        [Description(
             "The window size is how much of the visible spectrum to show at once. A window size of 1 will show all of it. A window size of 0 will show a single color. A window size of 0.5 will show half of the spectrum at one time. The effect will continue to rotate through the entire spectrum regardless of the window size."),
         Range(0.0, 1.0)]
        public double Window { get; set; } = 1;

        public override object Clone()
        {
            return Reflection.Clone(this);
        }

        protected override bool UpdateColors(RenderState renderState, ref Color[] colors, ref TimeSpan transitionDuration)
        {
            // Decide the direction of movement based on the effect behavior
            UpdateDirection(renderState);

            var offset = renderState.CyclePosition + Offset;

            // Using different algorithms for Random and the others because of possible drift
            // that isn't apparent in Random. Random keeps track of the current position within
            // the zones as a double (0-1) and increments or decrements based on the CycleIncrement
            // value in the render state.
            if(Behavior == EffectBehavior.Random)
            {
                ColorOffset += renderState.CycleIncrement * (Direction == EffectDirection.Forward ? 1 : -1);

                if(ColorOffset >= 1)
                {
                    ColorOffset -= Math.Floor(ColorOffset);
                }
                else if(ColorOffset < 0)
                {
                    ColorOffset += Math.Floor(-ColorOffset) + 1;
                }

                offset = ColorOffset;
            }

            for(var i = 0; i < colors.Length; ++i)
            {
                var hue = offset + (((double)i / ZoneCount) * Window);

                switch(hue)
                {
                    case >= 1:
                        hue -= Math.Floor(hue);
                        break;
                    case < 0:
                        hue += Math.Floor(-hue) + 1;
                        break;
                }

                colors[i] = new Color(hue, Saturation, Brightness, Kelvin);
            }

            if((Behavior != EffectBehavior.Random) && (Direction == EffectDirection.Forward))
            {
                Array.Reverse(colors);
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
