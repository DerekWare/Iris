using System.Collections.Generic;
using System.IO;
using DerekWare.Diagnostics;

namespace DerekWare.HomeAutomation.Lifx.Lan.Messages
{
    class EchoRequest : Request
    {
        public const ulong Content = 0x0BADF00DUL;
        public new const ushort MessageType = 58;

        public EchoRequest()
            : base(MessageType)
        {
        }

        protected override void SerializePayload(BinaryWriter writer)
        {
            writer.Write(Content);
        }
    }

    class StateEcho : Response
    {
        public const ushort MessageType = 59;

        #region Conversion

        protected override void Parse(List<Message> messages)
        {
            Debug.Assert(1 == messages.Count);

            using var ms = new MemoryStream(messages[0].Payload);
            using var b = new BinaryReader(ms);

            var content = b.ReadUInt64();

            if(EchoRequest.Content != content)
            {
                Debug.Warning(this, "Echo contents not as expected");
            }
        }

        #endregion
    }
}
