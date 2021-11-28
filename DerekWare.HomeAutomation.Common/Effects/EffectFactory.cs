using System.Collections.Generic;
using System.Linq;
using DerekWare.Collections;

namespace DerekWare.HomeAutomation.Common.Effects
{
    public class EffectFactory : Factory<Effect, IReadOnlyEffectProperties>
    {
        public static readonly EffectFactory Instance = new();

        readonly SynchronizedList<Effect> _RunningEffects = new();

        EffectFactory()
        {
        }

        public IReadOnlyCollection<Effect> RunningEffects => _RunningEffects;

        public IReadOnlyCollection<Effect> GetRunningEffects(IDevice device)
        {
            // Find the intersection of the which devices have running effects and
            // the given device/device group. This is a little interesting because
            // an effect may be running on a device group, which includes child
            // devices and the given device may or may not be a device group.
            var running = new HashSet<Effect>();
            var a = device.GetDevices().Append(device).ToDistinctList();

            foreach(var effect in RunningEffects)
            {
                var b = effect.Device.GetDevices();

                if(a.Intersect(b).Any())
                {
                    running.Add(effect);
                }
            }

            return running;
        }

        public void Stop(IDevice device)
        {
            device.SetFirmwareEffect(null);
            GetRunningEffects(device).ForEach(effect => effect.Stop());
        }

        public void StopAllEffects()
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
