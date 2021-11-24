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

    class EchoResponse : Response
    {
        public new const ushort MessageType = 59;

        #region Conversion

        public override bool Parse()
        {
            Debug.Assert(1 == Messages.Count);

            using var ms = new MemoryStream(Messages[0].Payload);
            using var b = new BinaryReader(ms);

            var content = b.ReadUInt64();

            if(EchoRequest.Content != content)
            {
                Debug.Warning(this, "Echo contents not as expected");
            }

            return true;
        }

        #endregion
    }
}
