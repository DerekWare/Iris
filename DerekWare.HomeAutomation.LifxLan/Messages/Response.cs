using System;
using System.Collections.Generic;

namespace DerekWare.HomeAutomation.Lifx.Lan.Messages
{
    abstract class Response
    {
        #region Conversion

        protected abstract void Parse(List<Message> message);

        #endregion

        static readonly Dictionary<ushort, Func<Response>> TypeMap = new()
        {
            { Acknowledgement.MessageType, () => new Acknowledgement() },
            { LightState.MessageType, () => new LightState() },
            { StateEcho.MessageType, () => new StateEcho() },
            { StateExtendedColorZones.MessageType, () => new StateExtendedColorZones() },
            { StateGroup.MessageType, () => new StateGroup() },
            { StateLabel.MessageType, () => new StateLabel() },
            { StateLocation.MessageType, () => new StateLocation() },
            { StateMultiZone.MessageType, () => new StateMultiZone() },
            { StateMultiZoneEffect.MessageType, () => new StateMultiZoneEffect() },
            { StatePower.MessageType, () => new StatePower() },
            { StateService.MessageType, () => new StateService() },
            { StateVersion.MessageType, () => new StateVersion() }
        };

        List<Message> _Messages = new();

        public List<Message> Messages
        {
            get => _Messages;
            set
            {
                _Messages = value;
                Parse(value);
            }
        }

        public static Response Create(ushort messageType)
        {
            if(!TypeMap.TryGetValue(messageType, out var func))
            {
                return null;
            }

            return func();
        }
    }
}
