using System.Collections.Generic;
using System.Linq;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common.Effects;

namespace DerekWare.HomeAutomation.Common
{
    public interface IEffectFactory : IFactory<IEffect, IReadOnlyEffectProperties>
    {
        public IReadOnlyCollection<Effect> RunningEffects { get; }
    }

    public class EffectFactory : Factory<IEffect, IReadOnlyEffectProperties>, IEffectFactory
    {
        public static readonly EffectFactory Instance = new();

        readonly SynchronizedList<Effect> _RunningEffects = new();

        EffectFactory()
        {
        }

        public IReadOnlyCollection<Effect> RunningEffects => _RunningEffects.ToList();

        public void Stop(IDevice device)
        {
            // Stop any firmware effects that might be running
            device.SetFirmwareEffect(null);

            // Stop all effects we're aware of on the given device. RunningEffects returns
            // a copy of the list, so there's no worry about modifying the list during
            // enumeration.
            var effects = from effect in RunningEffects
                          where Equals(effect.Device, device)
                          select effect;

            effects.ForEach(effect => effect.Stop());
        }

        public void Stop(IEnumerable<IDevice> devices)
        {
            devices.ForEach(Stop);
        }

        public void StopAll()
        {
            RunningEffects.ForEach(i => i.Stop());
        }

        internal void OnEffectStarted(Effect effect)
        {
            if(effect is NullEffect)
            {
                return;
            }

            _RunningEffects.Add(effect);
        }

        internal void OnEffectStopped(Effect effect)
        {
            _RunningEffects.Remove(effect);
        }
    }
}
