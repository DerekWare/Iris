using System.Collections.Generic;
using System.ComponentModel;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.Reflection;

namespace DerekWare.HomeAutomation.Common.Themes
{
    [Name("Bright White")]
    public class BrightWhite : Theme
    {
        [Browsable(false)]
        public override bool IsDynamic => true;

        [Browsable(false)]
        public override bool IsMultiZone => false;

        public override object Clone()
        {
            return Reflection.Clone(this);
        }

        public override IReadOnlyCollection<Color> GetPalette(IDevice targetDevice)
        {
            return new[] { Colors.Colors.White };
        }
    }
}
