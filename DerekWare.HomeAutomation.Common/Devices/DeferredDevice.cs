using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using DerekWare.Diagnostics;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.HomeAutomation.Common.Effects;
using DerekWare.HomeAutomation.Common.Themes;

namespace DerekWare.HomeAutomation.Common.Devices
{
    [Description("DeferredDevice allows for lazy loading of a device based on Family and Uuid.")]
    public interface IDeferredDevice : IDevice, ISerializable, IEquatable<DeferredDevice>, IMatch
    {
        public IDevice Device { get; }
    }

    [Serializable]
    public class DeferredDevice : IDeferredDevice
    {
        IClient _Client;
        IDevice _Device;

        public event EventHandler<DeviceEventArgs> DeviceDiscovered;
        public event EventHandler<DeviceEventArgs> PropertiesChanged;
        public event EventHandler<DeviceEventArgs> StateChanged;

        public DeferredDevice(IDevice device)
        {
            _Device = device;
            _Client = device.Client;

            Family = _Device.Family;
            Uuid = _Device.Uuid;

            _Client.DeviceDiscovered += OnDeviceDiscovered;
            _Device.PropertiesChanged += OnPropertiesChanged;
            _Device.StateChanged += OnStateChanged;
        }

        public DeferredDevice(SerializationInfo info, StreamingContext context)
        {
            Family = (string)info.GetValue(nameof(Family), typeof(string));
            Uuid = (string)info.GetValue(nameof(Uuid), typeof(string));

            FindDevice();
        }

        protected DeferredDevice()
        {
        }

        ~DeferredDevice()
        {
            Dispose();
        }

        public IClient Client
        {
            get
            {
                if(_Client is null)
                {
                    FindClient();
                }

                return _Client;
            }
        }

        public IDevice Device
        {
            get
            {
                if(_Device is null)
                {
                    FindDevice();
                }

                return _Device;
            }
        }

        public string Family { get; }
        public IReadOnlyCollection<IDeviceGroup> Groups => Device?.Groups;
        public bool IsColor => Device?.IsColor ?? false;
        public bool IsMultiZone => Device?.IsMultiZone ?? false;
        public bool IsValid => Device?.IsValid ?? false;
        public string Name => Device?.Name ?? Uuid;
        public string Uuid { get; }
        public int ZoneCount => Device?.ZoneCount ?? 0;

        public double Brightness
        {
            get => Device?.Brightness ?? 0;
            set
            {
                if(Device is not null)
                {
                    Device.Brightness = value;
                }
            }
        }

        public IReadOnlyCollection<Color> Color
        {
            get => Device?.Color;
            set
            {
                if(Device is not null)
                {
                    Device.Color = value;
                }
            }
        }

        public Effect Effect
        {
            get => Device?.Effect;
            set
            {
                if(Device is not null)
                {
                    Device.Effect = value;
                }
            }
        }

        public PowerState Power
        {
            get => Device?.Power ?? PowerState.Off;
            set
            {
                if(Device is not null)
                {
                    Device.Power = value;
                }
            }
        }

        public Theme Theme
        {
            get => Device?.Theme;
            set
            {
                if(Device is not null)
                {
                    Device.Theme = value;
                }
            }
        }

        protected bool FindClient()
        {
            _Client = ClientFactory.Instance.CreateInstance(Family);

            if(_Client is null)
            {
                Debug.Warning(this, $"Unable to find client {Family}");
                return false;
            }

            _Client.DeviceDiscovered += OnDeviceDiscovered;
            return true;
        }

        protected bool FindDevice()
        {
            _Device = Client?.Devices.FirstOrDefault(i => i.Uuid == Uuid) ?? Client?.Groups.FirstOrDefault(i => i.Uuid == Uuid);

            if(_Device is null)
            {
                Debug.Warning(this, $"Unable to find device {Uuid}");
                return false;
            }

            _Device.PropertiesChanged += OnPropertiesChanged;
            _Device.StateChanged += OnStateChanged;
            return true;
        }

        #region Equality

        public bool Equals(DeferredDevice other)
        {
            if(ReferenceEquals(null, other))
            {
                return false;
            }

            if(ReferenceEquals(this, other))
            {
                return true;
            }

            return Matches(other);
        }

        public bool Equals(IDevice other)
        {
            if(other is DeferredDevice deferredDevice)
            {
                return Equals(deferredDevice);
            }
            
            return Device?.Equals(other) ?? false;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DeferredDevice) || Equals(obj as IDevice);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Family != null ? Family.GetHashCode() : 0) * 397) ^ (Uuid != null ? Uuid.GetHashCode() : 0);
            }
        }

        public static bool operator ==(DeferredDevice left, DeferredDevice right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DeferredDevice left, DeferredDevice right)
        {
            return !Equals(left, right);
        }

        #endregion

        #region IDeviceState

        public void RefreshState()
        {
            Device?.RefreshState();
        }

        public void SetColor(IReadOnlyCollection<Color> colors, TimeSpan transitionDuration)
        {
            Device?.SetColor(colors, transitionDuration);
        }

        public void SetFirmwareEffect(object effect)
        {
            Device?.SetFirmwareEffect(effect);
        }

        public void SetPower(PowerState power)
        {
            Device?.SetPower(power);
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if(_Client is not null)
            {
                _Client.DeviceDiscovered -= OnDeviceDiscovered;
                _Client = null;
            }

            if(_Device is not null)
            {
                _Device.PropertiesChanged -= OnPropertiesChanged;
                _Device.StateChanged -= OnStateChanged;
                _Device = null;
            }
        }

        #endregion

        #region IMatch

        public virtual bool Matches(object obj)
        {
            return obj is IFamily family and IUuid uuid && Family.Equals(family.Family) && Uuid.Equals(uuid.Uuid);
        }

        #endregion

        #region ISerializable

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Family), Family, Family.GetType());
            info.AddValue(nameof(Uuid), Uuid, Uuid.GetType());
        }

        #endregion

        #region Event Handlers

        void OnDeviceDiscovered(object sender, DeviceEventArgs e)
        {
            if(!Matches(e.Device))
            {
                return;
            }

            _Device = e.Device;
            _Client = e.Device.Client;

            _Device.PropertiesChanged += OnPropertiesChanged;
            _Device.StateChanged += OnStateChanged;

            DeviceDiscovered?.Invoke(this, e);
        }

        void OnPropertiesChanged(object sender, DeviceEventArgs e)
        {
            PropertiesChanged?.Invoke(this, e);
        }

        void OnStateChanged(object sender, DeviceEventArgs e)
        {
            StateChanged?.Invoke(this, e);
        }

        #endregion
    }
}
