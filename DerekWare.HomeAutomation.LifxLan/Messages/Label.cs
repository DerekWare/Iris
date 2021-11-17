using System.Collections.Generic;
using System.IO;
using DerekWare.Diagnostics;

namespace DerekWare.HomeAutomation.Lifx.Lan.Messages
{
    class GetLabelRequest : Request
    {
        public new const ushort MessageType = 23;

        public GetLabelRequest()
            : base(MessageType)
        {
        }
    }

    class StateLabel : Response
    {
        public const ushort MessageType = 25;

        public string Label { get; private set; }

        #region Conversion

        protected override void Parse(List<Message> messages)
        {
            Debug.Assert(1 == messages.Count);

            using var ms = new MemoryStream(messages[0].Payload);
            using var b = new BinaryReader(ms);

            Label = b.ReadBytes(32).DeserializeString();
        }

        #endregion
    }
}
