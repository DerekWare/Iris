using System.Collections.Generic;
using System.ComponentModel;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.Reflection;
using Newtonsoft.Json;

namespace DerekWare.HomeAutomation.Common.Themes
{
    [Name("Bright White")]
    public class BrightWhite : Theme
    {
        [Browsable(false), JsonIgnore]
        public override bool IsDynamic => true;

        [Browsable(false), JsonIgnore]
        public override bool IsMultiZone => false;

        public override object Clone()
        {
            return MemberwiseClone();
        }

        public override IReadOnlyCollection<Color> GetPalette(IDevice targetDevice)
        {
            return new[] { Colors.Colors.White };
        }
    }
}
