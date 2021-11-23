using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.HomeAutomation.Common.Scenes;

namespace DerekWare.HomeAutomation.Common.Effects
{
    [Description("Moves the current colors left, right or back and forth.")]
    public class Move : MultiZoneColorEffectRenderer
    {
        public enum EffectBehavior
        {
            Left,
            Right,
            Bounce,
            Random
        }

        public enum EffectDirection
        {
            Left,
            Right
        }

        protected readonly Random Random = new();
        protected double ColorOffset;
        protected EffectDirection Direction;
        protected TimeSpan NextChange = TimeSpan.Zero;

        public Move()
        {
            RefreshRate = TimeSpan.FromSeconds(1);
        }

        public override bool IsMultiZone => true;
        public virtual EffectBehavior Behavior { get; set; }

        public override object Clone()
        {
            return MemberwiseClone();
        }

        protected override bool UpdateColors(RenderState renderState, ref Color[] colors)
        {
            // Decide the direction of movement based on the effect behavior
            switch(Behavior)
            {
                case EffectBehavior.Left:
                    Direction = EffectDirection.Left;
                    break;

                case EffectBehavior.Right:
                    Direction = EffectDirection.Right;
                    break;

                case EffectBehavior.Bounce:
                    // Every cycle, reverse the direction
                    if(renderState.CycleCountChanged)
                    {
                        Direction = Direction == EffectDirection.Left ? EffectDirection.Right : EffectDirection.Left;
                    }

                    break;

                case EffectBehavior.Random:
                    // If we passed the randomly chosen position, reverse direction
                    if(renderState.TotalElapsed >= NextChange)
                    {
                        NextChange += TimeSpan.FromSeconds(Duration.TotalSeconds * Random.NextDouble());
                        Direction = Direction == EffectDirection.Left ? EffectDirection.Right : EffectDirection.Left;
                    }

                    break;
            }

            int colorOffset;

            // Using different algorithms for Random and the others because of possible drift
            // that isn't apparent in Random.
            if(Behavior == EffectBehavior.Random)
            {
                // Calculate how far to move the offset into the palette based on how far we've moved
                // from the last update.
                var cycleIncrement = renderState.CycleIncrement * (Direction == EffectDirection.Left ? 1 : -1);
                ColorOffset += cycleIncrement;
                colorOffset = (int)(ColorOffset * ZoneCount);
            }
            else
            {
                colorOffset = (int)((Direction == EffectDirection.Left ? renderState.CyclePosition : 1 - renderState.CyclePosition) * ZoneCount);
            }

            foreach(var i in Palette)
            {
                colors.SetWrappingValue(colorOffset++, i);
            }

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
