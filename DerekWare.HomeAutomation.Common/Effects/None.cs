using System.ComponentModel;

namespace DerekWare.HomeAutomation.Common.Effects
{
    [Description("Stops all other effects.")]
    public class None : Effect
    {
        [Browsable(false)]
        public override bool IsFirmware => false;

        [Browsable(false)]
        public override bool IsMultiZone => false;

        public override object Clone()
        {
            return Reflection.Clone(this);
        }

        protected override void StartEffect()
        {
            Stop();
        }

        protected override void StopEffect()
        {
        }
    }
}
