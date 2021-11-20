using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using DerekWare.HomeAutomation.Common.Colors;

namespace DerekWare.HomeAutomation.Common.Scenes
{
    [Browsable(false), Description("The user scene is created via a scene editor and saved in the application settings.")]
    public class UserScene : Scene
    {
        [XmlIgnore]
        public override bool IsDynamic => false;

        [XmlIgnore]
        public override bool IsMultiZone => Palette.Count > 1;

        public new string Name { get => base.Name; set => base.Name = value; }

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
