using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.HomeAutomation.Common.Scenes;

namespace DerekWare.HomeAutomation.Common.Effects
{
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

                while(ColorOffset >= 1)
                {
                    ColorOffset -= 1;
                }

                while(ColorOffset < 0)
                {
                    ColorOffset += 1;
                }

                colorOffset = (int)(ColorOffset * colors.Length);
            }
            else
            {
                colorOffset = (int)((Direction == EffectDirection.Left ? renderState.CyclePosition : 1 - renderState.CyclePosition) * colors.Length);
            }

            foreach(var i in Palette)
            {
                colorOffset %= colors.Length;
                colors[colorOffset++] = i;
            }

            return true;
        }

        protected override TimeSpan ValidateRefreshRate()
        {
            var refreshRate = base.ValidateRefreshRate().TotalSeconds;
            refreshRate = Math.Max(refreshRate, 1.0 / Palette.Count);
            refreshRate = Math.Max(refreshRate, 1.0 / Duration.TotalSeconds);
            return TimeSpan.FromSeconds(refreshRate);
        }
    }
}
