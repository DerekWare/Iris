using System.Collections.Generic;
using System.Linq;
using DerekWare.Collections;

namespace DerekWare.HomeAutomation.Common.Effects
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

        public IEnumerable<IEffect> GetRunningEffects(IDevice device)
        {
            // For the given device or device group, compare that list to the list of
            // running effects and their devices.
            // TODO there's got to be a better way.
            var a = device.GetDevices();

            foreach(var e in RunningEffects)
            {
                var b = e.Device.GetDevices();

                if(a.Intersect(b).Any())
                {
                    yield return e;
                }
            }
        }

        public void Stop(IDevice device)
        {
            // Stop any firmware effects that might be running
            device.SetFirmwareEffect(null);

            // Stop all effects we're aware of on the given device. RunningEffects returns
            // a copy of the list, so there's no worry about modifying the list during
            // enumeration.
            GetRunningEffects(device).ForEach(effect => effect.Stop());
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
