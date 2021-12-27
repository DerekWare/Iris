using DerekWare.HomeAutomation.Common.Themes;

namespace DerekWare.Iris.Controls
{
    public class ThemeTabControl : DevicePropertyTabControl<IReadOnlyThemeProperties>
    {
        public ThemeTabControl()
        {
            Factory = ThemeFactory.Instance;
        }
    }
}
