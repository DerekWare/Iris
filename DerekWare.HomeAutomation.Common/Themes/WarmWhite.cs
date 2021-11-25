using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.Reflection;

namespace DerekWare.HomeAutomation.Common.Themes
{
    [Name("Warm White")]
    public class WarmWhite : Theme
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
            return new[] { Colors.Colors.WarmWhite };
        }
    }
}
