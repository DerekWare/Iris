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

        public override IReadOnlyCollection<Color> GetPalette(IDevice targetDevice)
        {
            var palette = new Color[targetDevice.ZoneCount];

            for(var i = 0; i < targetDevice.ZoneCount; ++i)
            {
                palette[i] = new Color { Hue = i / (double)targetDevice.ZoneCount, Saturation = Saturation, Brightness = Brightness, Kelvin = Kelvin };
            }

            return palette;
        }
    }
}
