using System;
using System.ComponentModel;
using System.Xml.Serialization;

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

    public interface IReadOnlyEffectProperties : IDescription, IFamily, IName, IEquatable<IReadOnlyEffectProperties>
    {
        public IDevice Device { get; }
        public bool IsFirmware { get; }
        public bool IsMultiZone { get; }
    }

    public abstract class Effect : IEffect
    {
        [Description("True if the effect runs on the device as opposed to running in this application."), Browsable(false), XmlIgnore]
        public abstract bool IsFirmware { get; }

        [Description("True if the effect is intended for multizone lights or light groups."), Browsable(false), XmlIgnore]
        public abstract bool IsMultiZone { get; }

        protected abstract void StartEffect();
        protected abstract void StopEffect(bool wait);

        #region ICloneable

        public abstract object Clone();

        #endregion

        [XmlIgnore]
        public string Description => this.GetDescription();

        [Browsable(false), XmlIgnore]
        public virtual string Family => null;

        [Browsable(false), XmlIgnore]
        public bool IsRunning => Device is not null;

        [Browsable(false), XmlIgnore]
        public string Name => this.GetName();

        [Browsable(false), XmlIgnore]
        public IDevice Device { get; private set; }

        #region Equality

        public bool Equals(IReadOnlyEffectProperties other)
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

            return Equals((IReadOnlyEffectProperties)obj);
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
            EffectFactory.Instance.StopEffect(Device);

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
    }
}
