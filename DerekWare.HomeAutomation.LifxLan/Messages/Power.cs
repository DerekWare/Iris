using System.IO;
using DerekWare.Diagnostics;
using DerekWare.HomeAutomation.Common;

namespace DerekWare.HomeAutomation.Lifx.Lan.Messages
{
    class GetPowerRequest : Request
    {
        public new const ushort MessageType = 20;

        public GetPowerRequest()
            : base(MessageType)
        {
        }
    }

    class PowerResponse : Response
    {
        public new const ushort MessageType = 22;

        public PowerState Power { get; private set; }

        #region Conversion

        public override bool Parse()
        {
            Debug.Assert(1 == Messages.Count);

            using var ms = new MemoryStream(Messages[0].Payload);
            using var b = new BinaryReader(ms);

            Power = b.ReadUInt16() == ushort.MinValue ? PowerState.Off : PowerState.On;

            return true;
        }

        #endregion
    }

    class SetPowerRequest : Request
    {
        public new const ushort MessageType = 21;

        public SetPowerRequest()
            : base(MessageType)
        {
        }

        public PowerState Power { get; set; }

        protected override void SerializePayload(BinaryWriter writer)
        {
            writer.Write(Power == PowerState.On ? ushort.MaxValue : ushort.MinValue);
        }
    }
}
