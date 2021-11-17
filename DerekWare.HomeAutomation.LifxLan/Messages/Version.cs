using System.Collections.Generic;
using System.IO;
using DerekWare.Diagnostics;

namespace DerekWare.HomeAutomation.Lifx.Lan.Messages
{
    class GetVersionRequest : Request
    {
        public new const ushort MessageType = 32;

        public GetVersionRequest()
            : base(MessageType)
        {
        }
    }

    class StateVersion : Response
    {
        public const ushort MessageType = 33;

        public uint ProductId { get; private set; }
        public uint VendorId { get; private set; }

        #region Conversion

        protected override void Parse(List<Message> messages)
        {
            Debug.Assert(1 == messages.Count);

            using var ms = new MemoryStream(messages[0].Payload);
            using var b = new BinaryReader(ms);

            VendorId = b.ReadUInt32();
            ProductId = b.ReadUInt32();
        }

        #endregion
    }
}
