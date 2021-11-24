namespace DerekWare.HomeAutomation.Lifx.Lan.Messages
{
    class GetLocationRequest : GetGroupRequest
    {
        public new const ushort MessageType = 48;

        public GetLocationRequest()
            : base(MessageType)
        {
        }
    }

    class LocationResponse : GroupResponse
    {
        public new const ushort MessageType = 50;
    }
}
