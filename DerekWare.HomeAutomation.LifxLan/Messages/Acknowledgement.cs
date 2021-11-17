using System.Collections.Generic;

namespace DerekWare.HomeAutomation.Lifx.Lan.Messages
{
    class Acknowledgement : Response
    {
        public const ushort MessageType = 45;

        #region Conversion

        protected override void Parse(List<Message> messages)
        {
        }

        #endregion
    }
}
