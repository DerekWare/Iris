using System;
using System.ComponentModel;

namespace DerekWare.HomeAutomation.Common.Effects
{
    public interface IEffect : IEffectProperties, ICloneable, IDisposable
    {
        public void Start(IDevice device);
        public void Stop(bool wait = true);
    }

    public interface IEffectProperties : IReadOnlyEffectProperties
    {
    }

    public interface IReadOnlyEffectProperties : IName, IFamily
    {
        [Browsable(false)]
        public IDevice Device { get; }

        [Description("True if the effect runs on the device as opposed to running in this application.")]
        public bool IsFirmware { get; }

        [Description("True if the effect is intended for multizone lights or light groups.")]
        public bool IsMultiZone { get; }
    }

    public abstract class Effect : IEffect
    {
        [Description("True if the effect runs on the device as opposed to running in this application.")]
        public abstract bool IsFirmware { get; }

        [Description("True if the effect is intended for multizone lights or light groups.")]
        public abstract bool IsMultiZone { get; }

        protected abstract void StartEffect();
        protected abstract void StopEffect(bool wait);

        #region ICloneable

        public abstract object Clone();

        #endregion

        [Browsable(false)]
        public bool IsRunning => Device is not null;

        [Browsable(false)]
        public string Name => GetType().GetTypeName();

        [Browsable(false)]
        public IDevice Device { get; private set; }

        #region IDisposable

        public virtual void Dispose()
        {
            Stop();
        }

        #endregion

        #region IEffect

        public virtual void Start(IDevice device)
        {
            // Save the device
            Device = device;

            // Stop all other effects on the given devices
            EffectFactory.Instance.Stop(Device);

            // Register with the factory
            EffectFactory.Instance.OnEffectStarted(this);

            // Notify the devices
            // TODO DeviceList.ForEach(i => i.OnActiveEffectChanged(this));

            // Start doing the things
            StartEffect();
        }

        // Stop the effect for all devices
        public virtual void Stop(bool wait = true)
        {
            // Notify the devices
            // TODO DeviceList.ForEach(i => i.OnActiveEffectChanged(null));

            // Unregister with the factory
            EffectFactory.Instance.OnEffectStopped(this);

            // Stop doing the things
            StopEffect(wait);

            Device = null;
        }

        #endregion

        public virtual string Family => null;
    }
}
