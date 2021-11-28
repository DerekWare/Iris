using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using DerekWare.Reflection;

namespace DerekWare.HomeAutomation.Common.Effects
{
    [Name("Stop"), Description("Stops all other effects.")]
    public class NullEffect : Effect
    {
        [Browsable(false)]
        public override bool IsFirmware => false;

        [Browsable(false)]
        public override bool IsMultiZone => false;

        public override object Clone()
        {
            return MemberwiseClone();
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        protected override void StartEffect()
        {
            Stop();
        }

        protected override void StopEffect(bool wait)
        {
        }
    }
}
