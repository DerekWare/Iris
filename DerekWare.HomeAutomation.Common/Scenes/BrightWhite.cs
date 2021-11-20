using System.Collections.Generic;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.Reflection;

namespace DerekWare.HomeAutomation.Common.Scenes
{
    [Name("Bright White")]
    public class BrightWhite : Scene
    {
        public override bool IsDynamic => true;
        public override bool IsMultiZone => false;

        public override object Clone()
        {
            return MemberwiseClone();
        }

        public override IReadOnlyCollection<Color> GetPalette(IDevice targetDevice)
        {
            return new[] { StandardColors.White };
        }
    }
}
