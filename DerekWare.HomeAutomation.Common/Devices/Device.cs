using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.HomeAutomation.Common.Effects;
using DerekWare.HomeAutomation.Common.Themes;

namespace DerekWare.HomeAutomation.Common
{
    public interface IDevice : IDeviceProperties, IDeviceState, IEquatable<IDevice>, IDisposable
    {
        event EventHandler<DeviceEventArgs> PropertiesChanged;
        event EventHandler<DeviceEventArgs> StateChanged;
    }

    // Properties generally don't change at runtime except when first connecting to the device
    public interface IDeviceProperties : IName, IFamily
    {
        IClient Client { get; }
        IReadOnlyCollection<IDeviceGroup> Groups { get; }
        bool IsColor { get; }
        bool IsMultiZone { get; }
        bool IsValid { get; }
        string Product { get; }
        string Uuid { get; }
        string Vendor { get; }
        int ZoneCount { get; }
    }

    // State changes based on user interaction
    public interface IDeviceState
    {
        double Brightness { get; set; }
        Color Color { get; set; }
        Effect Effect { get; set; }
        IReadOnlyCollection<Color> MultiZoneColors { get; set; }
        PowerState Power { get; set; }
        Theme Theme { get; set; }

        void RefreshState();
        void SetColor(Color color, TimeSpan transitionDuration);
        void SetFirmwareEffect(object effect);
        void SetMultiZoneColors(IReadOnlyCollection<Color> colors, TimeSpan transitionDuration);
        void SetPower(PowerState power);
    }

    // Optional base class for devices
    public abstract class Device : IDevice, IEquatable<Device>
    {
        [Browsable(false)]
        public abstract IClient Client { get; }

        [Browsable(false)]
        public abstract IReadOnlyCollection<IDeviceGroup> Groups { get; }

        public abstract bool IsColor { get; }
        public abstract bool IsMultiZone { get; }
        public abstract bool IsValid { get; }
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

        public virtual event EventHandler<DeviceEventArgs> PropertiesChanged;
        public virtual event EventHandler<DeviceEventArgs> StateChanged;

        public virtual string Family => Client.Family;

        [Browsable(false)]
        public double Brightness
        {
            get => Color.Brightness;
            set => MultiZoneColors = MultiZoneColors.Select(color => new Color(color) { Brightness = value }).ToList();
        }

        [Browsable(false)]
        public virtual Color Color { get => _Color; set => SetColor(value, TimeSpan.Zero); }

        [Browsable(false)]
        public virtual Effect Effect
        {
            get => EffectFactory.Instance.GetRunningEffects(this).FirstOrDefault();
            set
            {
                EffectFactory.Instance.Stop(this);
                value?.Start(this);
                OnStateChanged();
            }
        }

        [Browsable(false)]
        public virtual IReadOnlyCollection<Color> MultiZoneColors { get => _MultiZoneColors; set => SetMultiZoneColors(value, TimeSpan.Zero); }

        [Browsable(false)]
        public virtual PowerState Power { get => _Power; set => SetPower(value); }

        [Browsable(false)]
        public virtual Theme Theme
        {
            get => null; // TODO
            set
            {
                value?.Apply(this);
                OnStateChanged();
            }
        }

        public override string ToString()
        {
            return $"{Name} ({Family})";
        }

        protected virtual void OnPropertiesChanged()
        {
            PropertiesChanged?.Invoke(this, new DeviceEventArgs { Device = this });
        }

        protected virtual void OnStateChanged()
        {
            StateChanged?.Invoke(this, new DeviceEventArgs { Device = this });
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

        public bool Equals(IDevice other)
        {
            if(ReferenceEquals(null, other))
            {
                return false;
            }

            if(ReferenceEquals(this, other))
            {
                return true;
            }

            if(other.GetType() != GetType())
            {
                return false;
            }

            return (Family == other.Family) && (Uuid == other.Uuid);
        }

        public bool Equals(Device other)
        {
            return Equals(other as IDevice);
        }

        public override bool Equals(object other)
        {
            return Equals(other as IDevice);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Family != null ? Family.GetHashCode() : 0) * 397) ^ (Uuid != null ? Uuid.GetHashCode() : 0);
            }
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
            if(Equals(color, _Color))
            {
                return;
            }

            _Color = color.Clone();
            _MultiZoneColors = _Color.Repeat(ZoneCount).ToList();
            OnStateChanged();
        }

        public virtual void SetMultiZoneColors(IReadOnlyCollection<Color> colors, TimeSpan transitionDuration)
        {
            colors = colors.Take(Math.Min(ZoneCount, colors.Count)).ToList();

            if(colors.SafeEmpty().SequenceEqual(_MultiZoneColors))
            {
                return;
            }

            _MultiZoneColors = colors.Select(i => i.Clone()).ToList();
            _Color = _MultiZoneColors.Average();
            OnStateChanged();
        }

        public virtual void SetPower(PowerState power)
        {
            if(Equals(power, _Power))
            {
                return;
            }

            _Power = power;

            if(PowerState.Off == _Power)
            {
                Effect = null;
            }

            OnStateChanged();
        }

        #endregion

        #region IDisposable

        public virtual void Dispose()
        {
            Extensions.Dispose(ref _RefreshTask);
        }

        #endregion
    }
}
