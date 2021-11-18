using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using DerekWare.HomeAutomation.Common.Colors;

namespace DerekWare.HomeAutomation.Common.Scenes
{
    public class Spectrum : Scene
    {
        [Browsable(false), XmlIgnore]
        public override bool IsDynamic => true;

        [Browsable(false), XmlIgnore]
        public override bool IsMultiZone => true;

        public double Brightness { get; set; } = 1;
        public double Kelvin { get; set; } = 1;
        public double Saturation { get; set; } = 1;

        public override object Clone()
        {
            return MemberwiseClone();
        }

        public override IEnumerable<Color> GetPalette(IDevice targetDevice)
        {
            double count = targetDevice.ZoneCount;

            for(var i = 0; i < count; ++i)
            {
                yield return new Color { Hue = i / count, Saturation = Saturation, Brightness = Brightness, Kelvin = Kelvin };
            }
        }
    }
}
