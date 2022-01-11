using System;
using System.ComponentModel;
using DerekWare.Collections;
using DerekWare.Diagnostics;
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

        [DefaultValue(Common.Direction.Forward)]
        protected Direction Direction = Direction.Forward;

        protected double IncrementalOffset;

        protected TimeSpan NextChange = TimeSpan.Zero;

        protected bool ClampRefreshRate = true;

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

        protected virtual double GetColorOffset(RenderState renderState)
        {
            UpdateDirection(renderState);

            // Using different algorithms for Random and the others because of possible drift
            // that isn't apparent in Random. Random keeps track of the current position within
            // the zones as a double (0-1) and increments or decrements based on the CycleIncrement
            // value in the render state.
            if(Behavior != EffectBehavior.Random)
            {
                return renderState.CyclePosition;
            }

            IncrementalOffset += renderState.CycleIncrement * (int)Direction;
            IncrementalOffset %= 1.0;

            if(IncrementalOffset < 0)
            {
                IncrementalOffset = 1.0 - IncrementalOffset;
            }

            return IncrementalOffset;
        }

        protected override bool UpdateColors(RenderState renderState, ref Color[] colors, ref TimeSpan transitionDuration)
        {
            var offset = (int)(GetColorOffset(renderState) * ZoneCount);

            foreach(var i in Palette)
            {
                colors.SetWrappingValue(offset++, i);
            }

            if(Direction == Direction.Backward)
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
                    Direction = Direction.Forward;
                    break;

                case EffectBehavior.Backward:
                    Direction = Direction.Backward;
                    break;

                case EffectBehavior.Bounce:
                    // Every cycle, reverse the direction
                    if(renderState.CycleCountChanged)
                    {
                        Direction = Direction == Direction.Forward ? Direction.Backward : Direction.Forward;
                    }

                    break;

                case EffectBehavior.Random:
                    // If we passed the randomly chosen position, reverse direction
                    if(renderState.TotalElapsed >= NextChange)
                    {
                        NextChange += Random.NextTimeSpan(Duration);
                        Direction = Direction == Direction.Forward ? Direction.Backward : Direction.Forward;
                    }

                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        protected override TimeSpan ValidateRefreshRate()
        {
            if(!ClampRefreshRate)
            {
                return base.ValidateRefreshRate();
            }
            
            var refreshRate = base.ValidateRefreshRate().TotalSeconds;
            refreshRate = Math.Max(refreshRate, 1.0 / ZoneCount);
            refreshRate = Math.Max(refreshRate, 1.0 / Duration.TotalSeconds);
            return TimeSpan.FromSeconds(refreshRate);
        }
    }
}
