using System;
using System.Collections.Generic;
using System.ComponentModel;
using DerekWare.HomeAutomation.Common.Colors;

namespace DerekWare.HomeAutomation.Common.Themes
{
    [Description("Apply the colors of flame of various temperatures.")]
    public class Flame : Theme
    {
        public enum FlameDirection
        {
            Forward,
            Backward
        }

        public enum FlameTemperature
        {
            Cool,
            Warm,
            Hot
        }

        static readonly Dictionary<FlameTemperature, Tuple<Color, Color>> ColorMap = new()
        {
            { FlameTemperature.Cool, new Tuple<Color, Color>(new Color(0, 1, 0.25, 1), new Color(45.0 / 360, 1, 1, 1)) },
            { FlameTemperature.Warm, new Tuple<Color, Color>(new Color(0, 1, 0.25, 1), new Color(45.0 / 360, 1, 1, 1)) },
            { FlameTemperature.Hot, new Tuple<Color, Color>(new Color(240.0 / 360, 1, 0.5, 1), new Color(180.0 / 360, 0, 1, 1)) }
        };

        public override bool IsDynamic => true;
        public override bool IsMultiZone => true;
        public FlameDirection Direction { get; set; }
        public FlameTemperature Temperature { get; set; }

        public override object Clone()
        {
            return Reflection.Clone(this);
        }

        public override IReadOnlyCollection<Color> GetPalette(IDevice targetDevice)
        {
            var (min, max) = ColorMap[Temperature];
            var colors = new List<Color>();

            for(var i = 0; i < targetDevice.ZoneCount; ++i)
            {
                colors.Add(GetColor(min, max, i, targetDevice.ZoneCount));
            }

            if(Direction == FlameDirection.Backward)
            {
                colors.Reverse();
            }

            return colors;
        }

        static Color GetColor(Color min, Color max, int index, int count)
        {
            return new Color(GetColorValue(min.Hue, max.Hue, index, count),
                             GetColorValue(min.Saturation, max.Saturation, index, count),
                             GetColorValue(min.Brightness, max.Brightness, index, count),
                             GetColorValue(min.Kelvin, max.Kelvin, index, count));
        }

        static double GetColorValue(double min, double max, int index, int count)
        {
            return min + (((max - min) * index) / (count - 1));
        }
    }
}
