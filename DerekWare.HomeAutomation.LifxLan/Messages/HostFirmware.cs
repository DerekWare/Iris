using System;
using System.IO;
using DerekWare.Diagnostics;

namespace DerekWare.HomeAutomation.Lifx.Lan.Messages
{
    class HostFirmwareResponse : Response
    {
        public new const ushort MessageType = 15;

        public HostFirmwareResponse()
            : base(MessageType)
        {
        }

        public ulong Build { get; set; }
        public Version Version { get; set; }

        #region Conversion

        public override bool Parse()
        {
            Debug.Assert(1 == Messages.Count);

            using var ms = new MemoryStream(Messages[0].Payload);
            using var b = new BinaryReader(ms);

            Build = b.ReadUInt64();
            b.ReadBytes(8);
            var minor = b.ReadUInt16();
            var major = b.ReadUInt16();
            Version = new Version(major, minor);

            return true;
        }

        #endregion
    }

    class GetHostFirmwareRequest : Request
    {
        public new const ushort MessageType = 14;

        public GetHostFirmwareRequest()
            : base(MessageType)
        {
        }
    }
}
