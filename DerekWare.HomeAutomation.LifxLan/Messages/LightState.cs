using System.Collections.Generic;
using System.IO;
using DerekWare.Diagnostics;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.HomeAutomation.Lifx.Lan.Colors;

namespace DerekWare.HomeAutomation.Lifx.Lan.Messages
{
    class LightState : Response
    {
        public const ushort MessageType = 107;

        public Color Color { get; private set; }
        public byte[] Label { get; private set; }
        public double Power { get; private set; }

        #region Conversion

        protected override void Parse(List<Message> messages)
        {
            Debug.Assert(1 == messages.Count);

            using var ms = new MemoryStream(messages[0].Payload);
            using var b = new BinaryReader(ms);

            Color = new Color();
            Color.DeserializeBinary(b);

            b.ReadUInt16();
            Power = (double)b.ReadUInt16() / ushort.MaxValue;
            Label = b.ReadBytes(32);
            b.ReadBytes(8);
        }

        #endregion
    }
}
