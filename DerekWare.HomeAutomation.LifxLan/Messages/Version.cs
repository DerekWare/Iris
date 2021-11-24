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

    class VersionResponse : Response
    {
        public new const ushort MessageType = 33;

        public uint ProductId { get; private set; }
        public uint VendorId { get; private set; }

        #region Conversion

        public override bool Parse()
        {
            Debug.Assert(1 == Messages.Count);

            using var ms = new MemoryStream(Messages[0].Payload);
            using var b = new BinaryReader(ms);

            VendorId = b.ReadUInt32();
            ProductId = b.ReadUInt32();

            return true;
        }

        #endregion
    }
}
