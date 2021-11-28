using System;
using System.ComponentModel;
using Newtonsoft.Json;

namespace DerekWare.HomeAutomation.Common.Effects
{
    public interface IEffectProperties : IReadOnlyEffectProperties
    {
    }

    public interface IReadOnlyEffectProperties : IName, IDescription, IFamily, ICloneable
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
        protected abstract void StopEffect(bool wait);

        #region ICloneable

        public abstract object Clone();

        #endregion

        [JsonIgnore]
        public string Description => this.GetDescription();

        [Browsable(false)]
        public virtual string Family => null;

        [Browsable(false)]
        public bool IsRunning => Device is not null;

        [Browsable(false)]
        public string Name => this.GetName();

        [Browsable(false)]
        public IDevice Device { get; private set; }

        public virtual void Dispose()
        {
            Stop();
        }

        internal void Start(IDevice device)
        {
            // We shouldn't already be running, but just in case
            Stop();

            // Save the device
            Device = device ?? throw new ArgumentNullException(nameof(device));

            // Register with the factory
            EffectFactory.Instance.OnEffectStarted(this);

            // Start doing the things
            StartEffect();
        }

        // Stop the effect for all devices
        internal void Stop()
        {
            // Stop doing the things
            StopEffect(true);

            // Unregister with the factory
            EffectFactory.Instance.OnEffectStopped(this);

            // Release the device
            Device = null;
        }

        #region Equality

        public bool Equals(Effect other)
        {
            if(ReferenceEquals(null, other))
            {
                return false;
            }

            if(ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj))
            {
                return false;
            }

            if(ReferenceEquals(this, obj))
            {
                return true;
            }

            if(obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((Effect)obj);
        }

        public override int GetHashCode()
        {
            return Name != null ? Name.GetHashCode() : 0;
        }

        public static bool operator ==(Effect left, Effect right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Effect left, Effect right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}
