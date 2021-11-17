using System;
using System.Collections.Generic;
using System.Linq;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.Threading;

namespace DerekWare.HomeAutomation.Common.Effects
{
    public abstract class MultiZoneColorEffectRenderer : EffectRenderer
    {
        protected abstract bool GetColors(RenderState renderState, out IReadOnlyCollection<Color> colors);

        public override bool IsMultiZone => true;

        protected virtual IReadOnlyCollection<Color> OriginalPalette { get; set; }
        protected virtual IReadOnlyCollection<Color> Palette { get; set; }
        protected virtual int ZoneCount { get; private set; } = 1;

        protected override void DoWork(Thread sender, DoWorkEventArgs e)
        {
            ZoneCount = Device.ZoneCount;
            OriginalPalette = Device.MultiZoneColors.ToList();
            Palette = OriginalPalette.ToList();

            base.DoWork(sender, e);

            Device.SetMultiZoneColors(OriginalPalette, TimeSpan.Zero);
        }

        protected override void Update(RenderState state)
        {
            if(GetColors(state, out var colors))
            {
                Device.SetMultiZoneColors(colors, FirstRun ? TimeSpan.Zero : RefreshRate);
            }
        }
    }
}
