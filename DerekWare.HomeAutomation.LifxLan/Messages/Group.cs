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

    class GroupResponse : Response
    {
        public new const ushort MessageType = 53;

        public string Label { get; protected set; }
        public string Uuid { get; protected set; }

        #region Conversion

        public override bool Parse()
        {
            Debug.Assert(Messages.Count == 1);

            using var ms = new MemoryStream(Messages[0].Payload);
            using var b = new BinaryReader(ms);

            Uuid = b.ReadBytes(16).FormatByteArray(string.Empty);
            Label = b.ReadBytes(32).DeserializeString();

            return true;
        }

        #endregion
    }
}
