using System.IO;
using DerekWare.Diagnostics;
using DerekWare.HomeAutomation.Common;
using DerekWare.HomeAutomation.Lifx.Lan;

namespace DerekWare.HomeAutomation.Lifx.Lan.Messages
{
    class LightStateResponse : Response
    {
        public new const ushort MessageType = 107;

        public LightStateResponse()
            : base(MessageType)
        {
        }

        public Color Color { get; private set; }
        public byte[] Label { get; private set; }
        public double Power { get; private set; }

        #region Conversion

        public override bool Parse()
        {
            Debug.Assert(1 == Messages.Count);

            using var ms = new MemoryStream(Messages[0].Payload);
            using var b = new BinaryReader(ms);

            Color = new Color();
            Color.DeserializeBinary(b);

            b.ReadUInt16();
            Power = (double)b.ReadUInt16() / ushort.MaxValue;
            Label = b.ReadBytes(32);
            b.ReadBytes(8);

            return true;
        }

        #endregion
    }
}
