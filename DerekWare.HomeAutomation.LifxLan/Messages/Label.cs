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

    class LabelResponse : Response
    {
        public new const ushort MessageType = 25;

        public LabelResponse()
            : base(MessageType)
        {
        }

        public string Label { get; private set; }

        #region Conversion

        public override bool Parse()
        {
            Debug.Assert(1 == Messages.Count);

            using var ms = new MemoryStream(Messages[0].Payload);
            using var b = new BinaryReader(ms);

            Label = b.ReadBytes(32).DeserializeString();

            return true;
        }

        #endregion
    }
}
