using System;
using System.Collections.Generic;

namespace DerekWare.HomeAutomation.Lifx.Lan.Messages
{
    abstract class Response
    {
        #region Conversion

        public abstract bool Parse();

        #endregion

        public List<Message> Messages { get; } = new();
        public ushort MessageType => GetMessageType(GetType());

        public static ushort GetMessageType(Type type)
        {
            return (ushort)type.GetField("MessageType").GetValue(null);
        }

        public static ushort GetMessageType<T>()
        {
            return GetMessageType(typeof(T));
        }
    }
}
