using System.Collections.Generic;
using System.ComponentModel;

namespace DerekWare.HomeAutomation.Common.Themes
{
    [Browsable(false), Description("The user theme is created via a theme editor and saved in the application settings.")]
    public class UserTheme : Theme
    {
        [Browsable(false)]
        public override bool IsDynamic => false;

        [Browsable(false)]
        public override bool IsMultiZone => Palette.Count > 1;

        public List<Color> Palette { get; set; } = new();

        public override object Clone()
        {
            return Reflection.Clone(this);
        }

        public override IReadOnlyCollection<Color> GetPalette(IDevice targetDevice)
        {
            return Palette;
        }
    }
}
