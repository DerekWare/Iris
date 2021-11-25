using DerekWare.HomeAutomation.Common.Colors;

namespace DerekWare.HomeAutomation.Common.Effects
{
    public abstract class SingleColorEffectRenderer : EffectRenderer
    {
        protected abstract bool GetColor(RenderState state, out Color color);

        public override bool IsMultiZone => false;

        protected override void Update(RenderState state)
        {
            if(GetColor(state, out var color))
            {
                Device.SetColor(color, RefreshRate);
            }
        }
    }
}
