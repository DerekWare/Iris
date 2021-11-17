using System;
using DerekWare.HomeAutomation.Common.Colors;

namespace DerekWare.HomeAutomation.Common.Effects
{
    public abstract class SingleColorEffectRenderer : EffectRenderer
    {
        protected abstract bool GetColor(RenderState state, out Color color);

        protected SingleColorEffectRenderer()
        {
            Duration = TimeSpan.FromSeconds(10);
            RefreshRate = TimeSpan.FromSeconds(5);
        }

        public override bool IsMultiZone => false;

        protected override void Update(RenderState state)
        {
            if(GetColor(state, out var color))
            {
                Device.SetColor(color, FirstRun ? TimeSpan.Zero : RefreshRate);
            }
        }
    }
}
