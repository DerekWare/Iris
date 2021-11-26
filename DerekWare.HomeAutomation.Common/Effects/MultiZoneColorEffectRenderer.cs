using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.Threading;
using Newtonsoft.Json;
using DoWorkEventArgs = DerekWare.Threading.DoWorkEventArgs;

namespace DerekWare.HomeAutomation.Common.Effects
{
    public abstract class MultiZoneColorEffectRenderer : EffectRenderer
    {
        protected abstract bool UpdateColors(RenderState renderState, ref Color[] colors);

        // The target colors to set
        Color[] Colors;

        [Browsable(false), JsonIgnore]
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
            if(!UpdateColors(state, ref Colors))
            {
                return;
            }

            if(Colors.Length > 1)
            {
                Device.SetMultiZoneColors(Colors, RefreshRate);
            }
            else
            {
                Device.SetColor(Colors[0], RefreshRate);
            }
        }
    }
}
