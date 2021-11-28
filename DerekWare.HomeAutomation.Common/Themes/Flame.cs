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
            Color min = new(), max = new();

            switch(Temperature)
            {
                case FlameTemperature.Cool:
                    min.Hue = 0;
                    min.Saturation = 1;
                    min.Brightness = 0.25;
                    min.Kelvin = 1;

                    max.Hue = 45.0 / 360;
                    max.Saturation = 1;
                    max.Brightness = 1;
                    max.Kelvin = 1;

                    break;

                case FlameTemperature.Warm:
                    min.Hue = 30.0 / 360;
                    min.Saturation = 1;
                    min.Brightness = 0.25;
                    min.Kelvin = 1;

                    max.Hue = 60.0 / 360;
                    max.Saturation = 1;
                    max.Brightness = 1;
                    max.Kelvin = 1;

                    break;

                case FlameTemperature.Hot:
                    min.Hue = 240.0 / 360;
                    min.Saturation = 1;
                    min.Brightness = 0.5;
                    min.Kelvin = 1;

                    max.Hue = 180.0 / 360;
                    max.Saturation = 0;
                    max.Brightness = 1;
                    max.Kelvin = 1;

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            List<Color> colors = new();

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
