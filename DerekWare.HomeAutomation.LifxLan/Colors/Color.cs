using System;
using System.IO;
using DerekWare.HomeAutomation.Common;

namespace DerekWare.HomeAutomation.Lifx.Lan
{
    // These are ranges used by encoded/binary LIFX messages. The ranges of the runtime properties are all 0-1.
    static class EncodedColorRanges
    {
        public const ushort MaxBrightness = ushort.MaxValue;
        public const ushort MaxHue = ushort.MaxValue;
        public const ushort MaxKelvin = 9000;
        public const ushort MaxSaturation = ushort.MaxValue;
        public const ushort MinBrightness = 0;
        public const ushort MinHue = 0;
        public const ushort MinKelvin = 1500;
        public const ushort MinSaturation = 0;

        public static ushort FromDouble(double val, ushort min, ushort max)
        {
            return (ushort)Math.Max(Math.Min((val * (max - min)) + min, max), min);
        }

        public static double ToDouble(ushort val, ushort min, ushort max)
        {
            return Math.Max(val - min, 0) / (double)(max - min);
        }
    }

    public static partial class Extensions
    {
        internal static void DeserializeBinary(this Color c, BinaryReader b)
        {
            c.Hue = EncodedColorRanges.ToDouble(b.ReadUInt16(), EncodedColorRanges.MinHue, EncodedColorRanges.MaxHue);
            c.Saturation = EncodedColorRanges.ToDouble(b.ReadUInt16(), EncodedColorRanges.MinSaturation, EncodedColorRanges.MaxSaturation);
            c.Brightness = EncodedColorRanges.ToDouble(b.ReadUInt16(), EncodedColorRanges.MinBrightness, EncodedColorRanges.MaxBrightness);
            c.Kelvin = EncodedColorRanges.ToDouble(b.ReadUInt16(), EncodedColorRanges.MinKelvin, EncodedColorRanges.MaxKelvin);
        }

        internal static void SerializeBinary(this Color c, BinaryWriter writer)
        {
            writer.Write(EncodedColorRanges.FromDouble(c.Hue, EncodedColorRanges.MinHue, EncodedColorRanges.MaxHue));
            writer.Write(EncodedColorRanges.FromDouble(c.Saturation, EncodedColorRanges.MinSaturation, EncodedColorRanges.MaxSaturation));
            writer.Write(EncodedColorRanges.FromDouble(c.Brightness, EncodedColorRanges.MinBrightness, EncodedColorRanges.MaxBrightness));
            writer.Write(EncodedColorRanges.FromDouble(c.Kelvin, EncodedColorRanges.MinKelvin, EncodedColorRanges.MaxKelvin));
        }
    }
}
