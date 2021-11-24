namespace DerekWare.HomeAutomation.Lifx.Lan.Messages
{
    abstract class Request : Message
    {
        protected Request(ushort messageType)
            : base(messageType)
        {
        }
    }
}
