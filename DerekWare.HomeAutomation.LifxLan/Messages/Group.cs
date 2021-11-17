using System.Collections.Generic;
using System.IO;
using DerekWare.Diagnostics;

namespace DerekWare.HomeAutomation.Lifx.Lan.Messages
{
    class GetGroupRequest : Request
    {
        public new const ushort MessageType = 51;

        public GetGroupRequest()
            : base(MessageType)
        {
        }

        protected GetGroupRequest(ushort messageType)
            : base(messageType)
        {
        }
    }

    class StateGroup : Response
    {
        public const ushort MessageType = 53;

        public string Label { get; protected set; }
        public string Uuid { get; protected set; }

        #region Conversion

        protected override void Parse(List<Message> messages)
        {
            Debug.Assert(messages.Count == 1);

            using var ms = new MemoryStream(messages[0].Payload);
            using var b = new BinaryReader(ms);

            Uuid = b.ReadBytes(16).FormatByteArray(string.Empty);
            Label = b.ReadBytes(32).DeserializeString();
        }

        #endregion
    }
}
