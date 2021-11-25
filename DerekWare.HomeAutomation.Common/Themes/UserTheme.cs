using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using DerekWare.HomeAutomation.Common.Colors;

namespace DerekWare.HomeAutomation.Common.Themes
{
    [Browsable(false), Description("The user theme is created via a theme editor and saved in the application settings.")]
    public class UserTheme : Theme
    {
        protected string _Name = "My Theme";

        [Browsable(false), XmlIgnore]
        public override bool IsDynamic => false;

        [Browsable(false), XmlIgnore]
        public override bool IsMultiZone => Palette.Count > 1;

        public new string Name { get => _Name; set => _Name = value; }

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
