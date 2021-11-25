namespace DerekWare.HomeAutomation.Common.Themes
{
    public interface IThemeFactory : IFactory<ITheme>
    {
    }

    public class ThemeFactory : Factory<ITheme>, IThemeFactory
    {
        public static readonly ThemeFactory Instance = new();

        ThemeFactory()
        {
        }
    }
}
