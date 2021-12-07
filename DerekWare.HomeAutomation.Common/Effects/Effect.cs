using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using DerekWare.Collections;
using Newtonsoft.Json;

namespace DerekWare.HomeAutomation.Common.Effects
{
    public interface IEffectProperties : IReadOnlyEffectProperties
    {
    }

    public interface IReadOnlyEffectProperties : IName, IDescription, IFamily, ICloneable, IMatch
    {
        public IDevice Device { get; }
    }

    public abstract class Effect : IEffectProperties
    {
        [Description("True if the effect runs on the device as opposed to running in this application."), Browsable(false)]
        public abstract bool IsFirmware { get; }

        [Description("True if the effect is intended for multizone lights or light groups."), Browsable(false)]
        public abstract bool IsMultiZone { get; }

        protected abstract void StartEffect();
        protected abstract void StopEffect();

        #region ICloneable

        public abstract object Clone();

        #endregion

        readonly object DeviceStateLock = new();

        IDevice _Device;
        Task DeviceStateTask;

        [JsonIgnore]
        public string Description => this.GetDescription();

        [Browsable(false), JsonIgnore]
        public IDevice Device => _Device;

        [Browsable(false)]
        public virtual string Family => null;

        [Browsable(false)]
        public bool IsRunning => Device is not null;

        [Browsable(false)]
        public string Name => this.GetName();

        public virtual void Dispose()
        {
            Stop();
        }

        internal void Start(IDevice device)
        {
            // We shouldn't already be running, but just in case
            Stop();

            // Save the device
            _Device = device ?? throw new ArgumentNullException(nameof(device));

            // Register for device state changes. If the device turns off, we can stop
            // the effect. This will get called all the damned time when an effect is
            // modifying the device, but I don't currently have a better way.
            Device.StateChanged += OnDeviceStateChanged;

            // Register with the factory
            EffectFactory.Instance.OnEffectStarted(this);

            // Start doing the things
            StartEffect();
        }

        // Stop the effect for all devices
        internal void Stop()
        {
            // Stop doing the things
            StopEffect();

            // Unregister with the factory
            EffectFactory.Instance.OnEffectStopped(this);

            // Release the device
            var device = Interlocked.Exchange(ref _Device, null);

            if(device is not null)
            {
                device.StateChanged -= OnDeviceStateChanged;
            }
        }

        void OnDeviceStateChanged()
        {
            if(Device?.Power == PowerState.Off)
            {
                Stop();
            }

            lock(DeviceStateLock)
            {
                DeviceStateTask = null;
            }
        }

        #region IMatch

        // Provides a loose match based on name and family
        public bool Matches(object other)
        {
            if(other is not (IName name and IFamily family))
            {
                return false;
            }

            if(Name.IsNullOrEmpty() || name.Name.IsNullOrEmpty())
            {
                return false;
            }

            return Name.Equals(name.Name) && this.IsCompatible(family);
        }

        #endregion

        #region Event Handlers

        void OnDeviceStateChanged(object sender, DeviceEventArgs e)
        {
            // Schedule a task to prevent any deadlocks or recursion that could be caused by this
            // event handler being called in response to a change this effect made to the device.
            lock(DeviceStateLock)
            {
                if(DeviceStateTask is null)
                {
                    DeviceStateTask = Task.Run(OnDeviceStateChanged);
                }
            }
        }

        #endregion
    }
}
