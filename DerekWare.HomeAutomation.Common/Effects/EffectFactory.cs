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
            var effects = new ObservableHashSet<Effect>();

            lock(_RunningEffects.SyncRoot)
            {
                // Because an effect can run on either a group or an individual device,
                // this gets a little complicated. First, find any effects running on
                // the object given.
                effects.AddRange(_RunningEffects.Where(effect => effect.Device.Equals(device)));

                // Add any effects that may be running on the device's groups
                foreach(var i in device.Groups)
                {
                    effects.AddRange(_RunningEffects.Where(effect => effect.Device.Equals(i)));
                }

                if(device is IDeviceGroup group)
                {
                    // Add any effects that may be running on this group's children
                    foreach(var i in group.Devices)
                    {
                        effects.AddRange(_RunningEffects.Where(effect => effect.Device.Equals(i)));
                    }

                    // Find any effects that may be running on OTHER groups with which
                    // this group may have common members. Ugh.
                    foreach(var i in group.Devices)
                    foreach(var j in i.Groups)
                    {
                        effects.AddRange(_RunningEffects.Where(effect => effect.Device.Equals(j)));
                    }
                }
            }

            return effects;
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
