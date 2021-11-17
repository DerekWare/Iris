using DerekWare.HomeAutomation.Common.Colors;
using Q42.HueApi;

namespace DerekWare.HomeAutomation.PhilipsHue
{
    public static class Colors
    {
        public const double BrightnessMax = 254;
        public const double BrightnessMin = 0;

        public const double HueMax = 65535;

        /*
         * Found on a Tasker forum:
         *
         * So as an example suppose our incandescent bulb has a color temperature 0f 2700K and we need Hue to match it.
         * Looking through the Hue Developer's API it shows that Hue uses a 'Mired' ct value and can be set from 153 to
         * 500. Mired is new to me too! All we need to know is that M = 1000000 / T where M is Mired and T is the light
         * temperature in degrees Kelvin. So for our 2700K equivalent ct value we get:
         * M = 1000000 / 2700
         * M = 370.37037037
         *
         * We would round to 370 and post that value to the bulb in Tasker.
         *
         * TODO: it would be nice to treat Kelvin as a real value and not 0-1.
         */
        public const double HueMin = 0;
        public const double MiredMax = 500;
        public const double MiredMin = 153;
        public const double SaturationMax = 254;
        public const double SaturationMin = 0;

        public static Color FromHueColor(int? hue, int? saturation, byte brightness, int? colorTemperature)
        {
            var h = hue ?? HueMin;
            var s = saturation ?? SaturationMin;
            double b = brightness;
            var m = colorTemperature ?? MiredMin;

            h = (h - HueMin) / (HueMax - HueMin);
            s = (s - SaturationMin) / (SaturationMax - SaturationMin);
            b = (b - BrightnessMin) / (BrightnessMax - BrightnessMin);
            m = (m - MiredMin) / (MiredMax - MiredMin);

            return new Color(h, s, b, m);
        }

        public static Color FromHueColor(State state)
        {
            return FromHueColor(state.Hue, state.Saturation, state.Brightness, state.ColorTemperature);
        }

        public static double MiredFromKelvin(double k)
        {
            return 1000000.0 / k;
        }

        public static void ToHueColor(this Color src, out int h, out int s, out byte b, out int m)
        {
            h = (int)(HueMin + (src.Hue * (HueMax - HueMin)));
            s = (int)(SaturationMin + (src.Saturation * (SaturationMax - SaturationMin)));
            b = (byte)(BrightnessMin + (src.Brightness * (BrightnessMax - BrightnessMin)));
            m = (int)(MiredMin + (src.Kelvin * (MiredMax - MiredMin)));
        }

        public static void ToHueColor(this Color src, State state)
        {
            // For colors, set hue, saturation, brightness; for white, set brightness and mired/colortemp
            ToHueColor(src, out var h, out var s, out var b, out var m);

            if(src.IsWhite)
            {
                state.Hue = null;
                state.Saturation = null;
                state.Brightness = b;
                state.ColorTemperature = m;
            }
            else
            {
                state.Hue = h;
                state.Saturation = s;
                state.Brightness = b;
                state.ColorTemperature = null;
            }
        }
    }
}
