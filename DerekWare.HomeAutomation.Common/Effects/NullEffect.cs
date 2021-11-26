using System.ComponentModel;
using DerekWare.Reflection;
using Newtonsoft.Json;

namespace DerekWare.HomeAutomation.Common.Effects
{
    [Name("Stop"), Description("Stops all other effects.")]
    public class NullEffect : Effect
    {
        [Browsable(false), JsonIgnore]
        public override bool IsFirmware => false;

        [Browsable(false), JsonIgnore]
        public override bool IsMultiZone => false;

        public override object Clone()
        {
            return MemberwiseClone();
        }

        public override void Start(IDevice device)
        {
            EffectFactory.Instance.StopEffect(device);
        }

        public override void Stop(bool wait = true)
        {
        }

        protected override void StartEffect()
        {
        }

        protected override void StopEffect(bool wait)
        {
        }
    }
}
