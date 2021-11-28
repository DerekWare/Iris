using System.Collections.Generic;
using System.ComponentModel;
using DerekWare.HomeAutomation.Common.Colors;

namespace DerekWare.HomeAutomation.Common.Themes
{
    [Browsable(false), Description("The user theme is created via a theme editor and saved in the application settings.")]
    public class UserTheme : Theme
    {
        [Browsable(false)]
        public override bool IsDynamic => false;

        [Browsable(false)]
        public override bool IsMultiZone => Palette.Count > 1;

        public override string Name { get => base.Name; set => base.Name = value; }

        public List<Color> Palette { get; set; } = new();

        public override object Clone()
        {
            return MemberwiseClone();
        }

        public override IReadOnlyCollection<Color> GetPalette(IDevice targetDevice)
        {
            return Palette;
        }
    }
}
