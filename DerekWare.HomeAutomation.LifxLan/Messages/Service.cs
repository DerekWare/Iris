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

    class ServiceResponse : Response
    {
        public new const ushort MessageType = 3;

        public uint Port { get; private set; }
        public byte Service { get; private set; }

        #region Conversion

        public override bool Parse()
        {
            Debug.Assert(1 == Messages.Count);

            using var ms = new MemoryStream(Messages[0].Payload);
            using var b = new BinaryReader(ms);

            Service = b.ReadByte();
            Port = b.ReadUInt32();

            return true;
        }

        #endregion
    }
}
