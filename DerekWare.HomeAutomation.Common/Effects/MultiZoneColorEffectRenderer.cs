using System.Collections.Generic;
using System.Linq;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.Threading;

namespace DerekWare.HomeAutomation.Common.Effects
{
    public abstract class MultiZoneColorEffectRenderer : EffectRenderer
    {
        protected abstract bool UpdateColors(RenderState renderState, ref Color[] colors);

        // The target colors to set
        Color[] Colors;

        public override bool IsMultiZone => true;

        // The original colors from the scene or device
        protected virtual IReadOnlyList<Color> Palette { get; private set; }
        protected int ZoneCount => Palette.Count;

        protected override void DoWork(Thread sender, DoWorkEventArgs e)
        {
            Palette = Device.MultiZoneColors.ToArray();
            Colors = Palette.ToArray();

            base.DoWork(sender, e);
        }

        protected override void Update(RenderState state)
        {
            if(UpdateColors(state, ref Colors))
            {
                if(Colors.Length == 1)
                {
                    Device.SetColor(Colors[0], RefreshRate);
                }
                else
                {
                    Device.SetMultiZoneColors(Colors, RefreshRate);
                }
            }
        }
    }
}
