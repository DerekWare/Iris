using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.Reflection;

namespace DerekWare.HomeAutomation.Common.Scenes
{
    [Name("Warm White")]
    public class WarmWhite : Scene
    {
        [Browsable(false), XmlIgnore]
        public override bool IsDynamic => true;

        [Browsable(false), XmlIgnore]
        public override bool IsMultiZone => false;

        public override object Clone()
        {
            return MemberwiseClone();
        }

        public override IReadOnlyCollection<Color> GetPalette(IDevice targetDevice)
        {
            return new[] { StandardColors.WarmWhite };
        }
    }
}
