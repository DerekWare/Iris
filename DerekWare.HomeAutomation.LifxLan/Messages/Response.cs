using System;
using System.Collections.Generic;

namespace DerekWare.HomeAutomation.Lifx.Lan.Messages
{
    abstract class Response
    {
        #region Conversion

        public abstract bool Parse();

        #endregion

        protected Response(ushort messageType)
        {
            MessageType = messageType;
        }

        public List<Message> Messages { get; } = new();
        public ushort MessageType { get; }
    }
}
