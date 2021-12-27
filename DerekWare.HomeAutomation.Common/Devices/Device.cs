using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.HomeAutomation.Common.Effects;
using DerekWare.HomeAutomation.Common.Scenes;
using DerekWare.HomeAutomation.Common.Themes;

namespace DerekWare.HomeAutomation.Common
{
    public interface IDevice : IDeviceProperties, IDeviceState, IEquatable<IDevice>, IDisposable
    {
        event EventHandler<DeviceEventArgs> PropertiesChanged;
        event EventHandler<DeviceEventArgs> StateChanged;
    }

    // Properties generally don't change at runtime except when first connecting to the device
    public interface IDeviceProperties : IName, IFamily, IUuid
    {
        IClient Client { get; }
        IReadOnlyCollection<IDeviceGroup> Groups { get; }
        bool IsColor { get; }
        bool IsMultiZone { get; }
        bool IsValid { get; }
        int ZoneCount { get; }
    }

    // State changes based on user interaction
    // TODO combine Color/Color
    public interface IDeviceState
    {
        double Brightness { get; set; }
        IReadOnlyCollection<Color> Color { get; set; }
        Effect Effect { get; set; }
        PowerState Power { get; set; }
        Theme Theme { get; set; }

        void RefreshState();
        void SetColor(IReadOnlyCollection<Color> colors, TimeSpan transitionDuration);
        void SetFirmwareEffect(object effect);
        void SetPower(PowerState power);
    }

    // Optional base class for devices
    public abstract class Device : IDevice
    {
        [Browsable(false)]
        public abstract IClient Client { get; }

        [Browsable(false)]
        public abstract IReadOnlyCollection<IDeviceGroup> Groups { get; }

        public abstract bool IsColor { get; }
        public abstract bool IsValid { get; }
        public abstract string Name { get; }
        public abstract string Uuid { get; }
        public abstract int ZoneCount { get; }

        protected abstract void ApplyColor(IReadOnlyCollection<Color> colors, TimeSpan transitionDuration);
        protected abstract void ApplyPower(PowerState power);

        #region IDeviceState

        public abstract void RefreshState();
        public abstract void SetFirmwareEffect(object effect);

        #endregion

        protected IReadOnlyCollection<Color> _Color = Array.Empty<Color>();
        protected PowerState _Power;
        protected DeviceStateRefreshTask _RefreshTask;

        public virtual event EventHandler<DeviceEventArgs> PropertiesChanged;
        public virtual event EventHandler<DeviceEventArgs> StateChanged;

        public virtual string Family => Client.Family;
        public virtual bool IsMultiZone => ZoneCount > 1;

        [Browsable(false)]
        public double Brightness { get => Color.First().Brightness; set => Color = Color.Select(color => new Color(color) { Brightness = value }).ToList(); }

        [Browsable(false)]
        public virtual IReadOnlyCollection<Color> Color { get => _Color; set => SetColor(value, TimeSpan.Zero); }

        [Browsable(false)]
        public virtual Effect Effect
        {
            get => EffectFactory.Instance.GetRunningEffects(this).FirstOrDefault();
            set
            {
                // TODO we don't currently have a way to see if any of the properties have
                // changed, so just assume they have and always apply the new effect.
#if false
                if(Equals(Effect, value))
                {
                    return;
                }
#endif

                EffectFactory.Instance.Stop(this);
                value?.Start(this);
                OnStateChanged();
            }
        }

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

        public virtual void SetColor(IReadOnlyCollection<Color> colors, TimeSpan transitionDuration, bool apply)
        {
            colors = colors.Take(Math.Min(ZoneCount, colors.Count)).ToArray();

            if(colors.Count < ZoneCount)
            {
                colors = colors.Append(colors.Last().Repeat(ZoneCount - colors.Count)).ToArray();
            }

            if(colors.SafeEmpty().SequenceEqual(Color))
            {
                return;
            }

            _Color = colors.Select(i => i.Clone()).ToArray();

            if(apply)
            {
                ApplyColor(_Color, transitionDuration);
            }

            OnStateChanged();
        }

        public virtual void SetPower(PowerState power, bool apply)
        {
            if(Equals(power, Power))
            {
                return;
            }

            _Power = power;

            if(apply)
            {
                ApplyPower(_Power);
            }

            OnStateChanged();

            // If a scene is configured to start automatically and this device was just powered
            // on, start the scene.
            // TODO this should really be in the Scene, but this is simpler.
            if(power == PowerState.On)
            {
                var scenes = from s in SceneFactory.Instance
                             where s.AutoApply
                             from d in this.GetDeviceGroups().Cast<IDevice>().Append(this)
                             where s.Contains(d)
                             select s;

                scenes.FirstOrDefault()?.Apply();
            }
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

        #endregion

        #region IDeviceState

        public void SetColor(IReadOnlyCollection<Color> colors, TimeSpan transitionDuration)
        {
            SetColor(colors, transitionDuration, true);
        }

        public void SetPower(PowerState power)
        {
            SetPower(power, true);
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
