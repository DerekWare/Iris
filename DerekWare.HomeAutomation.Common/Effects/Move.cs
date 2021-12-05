using System;
using System.ComponentModel;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common.Colors;

namespace DerekWare.HomeAutomation.Common.Effects
{
    [Description("Moves the current colors forward, backward or back and forth.")]
    public class Move : MultiZoneColorEffectRenderer
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

        public Move()
        {
            Duration = TimeSpan.FromSeconds(30);
            RefreshRate = TimeSpan.FromSeconds(1);
        }

        public virtual EffectBehavior Behavior { get; set; }

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

                if((ColorOffset < 0) || (ColorOffset >= 1))
                {
                    ColorOffset -= (int)ColorOffset;
                }

                cyclePosition = ColorOffset;
            }
            else if(Direction == EffectDirection.Backward)
            {
                cyclePosition = 1.0 - cyclePosition;
            }

            var colorOffset = (int)(cyclePosition * ZoneCount);

            foreach(var i in Palette)
            {
                colors.SetWrappingValue(colorOffset++, i);
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

        protected override TimeSpan ValidateRefreshRate()
        {
            var refreshRate = base.ValidateRefreshRate().TotalSeconds;
            refreshRate = Math.Max(refreshRate, 1.0 / ZoneCount);
            refreshRate = Math.Max(refreshRate, 1.0 / Duration.TotalSeconds);
            return TimeSpan.FromSeconds(refreshRate);
        }
    }
}
