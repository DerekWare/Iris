using System.Collections.Generic;
using System.IO;
using DerekWare.Diagnostics;

namespace DerekWare.HomeAutomation.Lifx.Lan.Messages
{
    class GetServiceRequest : Request
    {
        public new const ushort MessageType = 2;

        public GetServiceRequest()
            : base(MessageType)
        {
            ResponseRequired = true;
            Sequence = 0;
        }
    }

    class StateService : Response
    {
        public const ushort MessageType = 3;

        public uint Port { get; private set; }
        public byte Service { get; private set; }

        #region Conversion

        protected override void Parse(List<Message> messages)
        {
            Debug.Assert(1 == messages.Count);

            using var ms = new MemoryStream(messages[0].Payload);
            using var b = new BinaryReader(ms);

            Service = b.ReadByte();
            Port = b.ReadUInt32();
        }

        #endregion
    }
}
