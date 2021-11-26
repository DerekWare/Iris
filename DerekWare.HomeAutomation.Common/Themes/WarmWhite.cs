using System.Collections.Generic;
using System.ComponentModel;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.Reflection;
using Newtonsoft.Json;

namespace DerekWare.HomeAutomation.Common.Themes
{
    [Name("Warm White")]
    public class WarmWhite : Theme
    {
        [Browsable(false)]
        public override bool IsDynamic => true;

        [Browsable(false)]
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
