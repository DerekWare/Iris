using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using DerekWare.HomeAutomation.Common.Colors;

namespace DerekWare.HomeAutomation.Common.Scenes
{
    [Browsable(false), Description("The user scene is created via a scene editor and saved in the application settings.")]
    public class UserScene : Scene
    {
        protected string _Name;

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
