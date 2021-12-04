using System;
using System.Collections.Generic;
using System.Linq;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.Threading;

namespace DerekWare.HomeAutomation.Common.Effects
{
    public abstract class MultiZoneColorEffectRenderer : EffectRenderer
    {
        protected abstract bool UpdateColors(RenderState renderState, ref Color[] colors, ref TimeSpan transitionDuration);

        // The target colors to set
        Color[] Colors;

        public override bool IsMultiZone => true;

        protected int ZoneCount => Device.ZoneCount;

        // The original colors from the theme or device
        protected virtual IReadOnlyList<Color> Palette { get; private set; }

        protected override void DoWork(Thread sender, DoWorkEventArgs e)
        {
            Palette = Device.MultiZoneColors.ToArray();
            Colors = Palette.ToArray();

            base.DoWork(sender, e);
        }

        protected override void Update(RenderState state)
        {
            var transitionDuration = RefreshRate;

            if(!UpdateColors(state, ref Colors, ref transitionDuration))
            {
                return;
            }

            if(Colors.Length > 1)
            {
                Device.SetMultiZoneColors(Colors, transitionDuration);
            }
            else
            {
                Device.SetColor(Colors[0], transitionDuration);
            }
        }
    }
}
