using DerekWare.HomeAutomation.Common.Effects;

namespace DerekWare.Iris.Controls
{
    public class EffectTabControl : DevicePropertyTabControl<IReadOnlyEffectProperties>
    {
        public EffectTabControl()
        {
            Factory = EffectFactory.Instance;
        }
    }
}
