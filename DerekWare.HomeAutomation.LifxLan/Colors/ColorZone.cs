using System.IO;
using DerekWare.HomeAutomation.Common.Colors;

namespace DerekWare.HomeAutomation.Lifx.Lan.Colors
{
    public static partial class Extensions
    {
        public static void DeserializeBinary(this ColorZone c, BinaryReader b)
        {
            c.StartIndex = b.ReadByte();
            c.EndIndex = b.ReadByte();
            c.Color.DeserializeBinary(b);
        }

        public static void SerializeBinary(this ColorZone c, BinaryWriter writer)
        {
            writer.Write((byte)c.StartIndex);
            writer.Write((byte)c.EndIndex);
            c.Color.SerializeBinary(writer);
        }
    }
}
