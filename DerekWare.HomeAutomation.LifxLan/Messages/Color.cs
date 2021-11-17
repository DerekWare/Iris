using System;
using System.IO;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.HomeAutomation.Lifx.Lan.Colors;

namespace DerekWare.HomeAutomation.Lifx.Lan.Messages
{
    class GetColorRequest : Request
    {
        public new const ushort MessageType = 101;

        public GetColorRequest()
            : base(MessageType)
        {
        }
    }

    class SetColorRequest : Request
    {
        public new const ushort MessageType = 102;

        public SetColorRequest()
            : base(MessageType)
        {
        }

        public Color Color { get; set; }
        public TimeSpan Duration { get; set; }

        protected override void SerializePayload(BinaryWriter writer)
        {
            writer.Write((byte)0);
            Color.SerializeBinary(writer);
            writer.Write((uint)Duration.TotalMilliseconds);
        }
    }
}
