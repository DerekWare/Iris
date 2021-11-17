using System.ComponentModel;
using DerekWare.Reflection;

namespace DerekWare.HomeAutomation.Common.Effects
{
    // A fake effect that just stops all other effects
    [Name("Stop")]
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

        public override void Start(IDevice device)
        {
            EffectFactory.Instance.Stop(device);
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
