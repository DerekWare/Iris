using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.HomeAutomation.Common.Effects;

namespace DerekWare.HomeAutomation.Common
{
    public interface IDevice : IDeviceProperties, IDeviceState, IEquatable<IDevice>, IDisposable
    {
        event EventHandler<DeviceEventArgs> PropertiesChanged;
        event EventHandler<DeviceEventArgs> StateChanged;

        IReadOnlyCollection<IDeviceGroup> Groups { get; }
    }

    // Properties generally don't change at runtime except when first connecting to the device
    public interface IDeviceProperties : IName, IFamily
    {
        bool IsColor { get; }
        bool IsMultiZone { get; }
        string Product { get; }
        string Uuid { get; }
        string Vendor { get; }
        int ZoneCount { get; }
    }

    // State changes based on user interaction
    public interface IDeviceState
    {
        IReadOnlyCollection<IEffect> Effects { get; }
        Color Color { get; set; }
        IReadOnlyCollection<Color> MultiZoneColors { get; set; }
        PowerState Power { get; set; }

        void RefreshState();
        void SetColor(Color color, TimeSpan transitionDuration);
        void SetFirmwareEffect(object effect);
        void SetMultiZoneColors(IReadOnlyCollection<Color> colors, TimeSpan transitionDuration);
        void SetPower(PowerState power);
    }

    // Optional base class for devices
    public abstract class Device : IDevice, IEquatable<Device>
    {
        public abstract string Family { get; }
        public abstract IReadOnlyCollection<IDeviceGroup> Groups { get; }
        public abstract bool IsColor { get; }
        public abstract bool IsMultiZone { get; }
        public abstract string Name { get; }
        public abstract string Product { get; }
        public abstract string Uuid { get; }
        public abstract string Vendor { get; }
        public abstract int ZoneCount { get; }

        #region IDeviceState

        public abstract void RefreshState();
        public abstract void SetFirmwareEffect(object effect);

        #endregion

        protected Color _Color = new();
        protected IReadOnlyCollection<Color> _MultiZoneColors = Array.Empty<Color>();
        protected PowerState _Power;
        protected DeviceStateRefreshTask _RefreshTask;

        public event EventHandler<DeviceEventArgs> PropertiesChanged;
        public event EventHandler<DeviceEventArgs> StateChanged;

        [Browsable(false), XmlIgnore]
        public IReadOnlyCollection<IEffect> Effects => EffectFactory.Instance.GetRunningEffects(this).ToList();

        [Browsable(false), XmlIgnore]
        public virtual Color Color { get => _Color; set => SetColor(value, TimeSpan.Zero); }

        [Browsable(false), XmlIgnore]
        public virtual IReadOnlyCollection<Color> MultiZoneColors { get => _MultiZoneColors; set => SetMultiZoneColors(value, TimeSpan.Zero); }

        [Browsable(false), XmlIgnore]
        public virtual PowerState Power { get => _Power; set => SetPower(value); }

        public override string ToString()
        {
            return $"{Name} ({Family})";
        }

        protected void OnPropertiesChanged()
        {
            OnPropertiesChanged(new DeviceEventArgs { Device = this });
        }

        protected virtual void OnPropertiesChanged(DeviceEventArgs e)
        {
            PropertiesChanged?.Invoke(this, e);
        }

        protected void OnStateChanged()
        {
            OnStateChanged(new DeviceEventArgs { Device = this });
        }

        protected virtual void OnStateChanged(DeviceEventArgs e)
        {
            StateChanged?.Invoke(this, e);
        }

        protected virtual void StartRefreshTask()
        {
            StartRefreshTask(TimeSpan.FromSeconds(30));
        }

        protected virtual void StartRefreshTask(TimeSpan timeout)
        {
            _RefreshTask ??= new DeviceStateRefreshTask(this, timeout);
        }

        #region Equality

        public bool Equals(Device other)
        {
            if(ReferenceEquals(null, other))
            {
                return false;
            }

            if(ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(Uuid, other.Uuid);
        }

        public bool Equals(IDevice other)
        {
            if(ReferenceEquals(null, other))
            {
                return false;
            }

            if(other.GetType() != GetType())
            {
                return false;
            }

            return Equals((Device)other);
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

            return Equals((Device)obj);
        }

        public override int GetHashCode()
        {
            return Uuid != null ? Uuid.GetHashCode() : 0;
        }

        public static bool operator ==(Device left, Device right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Device left, Device right)
        {
            return !Equals(left, right);
        }

        #endregion

        #region IDeviceState

        public virtual void SetColor(Color color, TimeSpan transitionDuration)
        {
            _Color = color.Clone();
            _MultiZoneColors = _Color.Repeat(ZoneCount).ToList();
            OnStateChanged();
        }

        public virtual void SetMultiZoneColors(IReadOnlyCollection<Color> colors, TimeSpan transitionDuration)
        {
            _MultiZoneColors = colors.Take(Math.Min(ZoneCount, colors.Count)).Select(i => i.Clone()).ToList();
            _Color = _MultiZoneColors.First();
            OnStateChanged();
        }

        public virtual void SetPower(PowerState power)
        {
            _Power = power;
            OnStateChanged();
        }

        #endregion

        #region IDisposable

        public virtual void Dispose()
        {
            DerekWare.Extensions.Dispose(ref _RefreshTask);
        }

        #endregion
    }
}
