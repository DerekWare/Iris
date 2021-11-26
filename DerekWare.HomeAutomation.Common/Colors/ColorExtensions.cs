using System;
using System.Collections.Generic;

namespace DerekWare.HomeAutomation.Common.Colors
{
    public static partial class Colors
    {
        public static Color Average(this IEnumerable<Color> colors)
        {
            double h = 0, s = 0, b = 0, k = 0;
            var count = 0;

            foreach(var i in colors)
            {
                h += i.Hue;
                s += i.Saturation;
                b += i.Brightness;
                k += i.Kelvin;
                ++count;
            }

            if(count > 0)
            {
                h /= count;
                s /= count;
                b /= count;
                k /= count;
            }

            return new Color(h, s, b, k);
        }

        public static System.Drawing.Color HsvToRgb(double hue, double saturation, double value)
        {
            var hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            var f = (hue / 60) - Math.Floor(hue / 60);

            value *= 255;
            var v = Convert.ToInt32(value);
            var p = Convert.ToInt32(value * (1 - saturation));
            var q = Convert.ToInt32(value * (1 - (f * saturation)));
            var t = Convert.ToInt32(value * (1 - ((1 - f) * saturation)));

            return hi switch
            {
                0 => System.Drawing.Color.FromArgb(255, v, t, p),
                1 => System.Drawing.Color.FromArgb(255, q, v, p),
                2 => System.Drawing.Color.FromArgb(255, p, v, t),
                3 => System.Drawing.Color.FromArgb(255, p, q, v),
                4 => System.Drawing.Color.FromArgb(255, t, p, v),
                _ => System.Drawing.Color.FromArgb(255, v, p, q)
            };
        }

        public static Color Interpolate(Color x, Color y, double position)
        {
            return new Color((x.Hue * (1 - position)) + (y.Hue * position),
                             (x.Saturation * (1 - position)) + (y.Saturation * position),
                             (x.Brightness * (1 - position)) + (y.Brightness * position),
                             (x.Kelvin * (1 - position)) + (y.Kelvin * position));
        }

        public static void RgbToHsv(this System.Drawing.Color color, out double hue, out double saturation, out double value)
        {
            hue = color.GetHue();
            saturation = color.GetSaturation();
            value = color.GetBrightness();
        }
    }
}
