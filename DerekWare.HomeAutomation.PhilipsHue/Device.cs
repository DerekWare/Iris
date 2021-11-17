using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DerekWare.Diagnostics;
using DerekWare.HomeAutomation.Common;
using DerekWare.HomeAutomation.Common.Colors;
using Q42.HueApi;

namespace DerekWare.HomeAutomation.PhilipsHue
{
    public class Device : IDevice, IEquatable<Device>
    {
        internal Light HueDevice;

        public event EventHandler<DeviceEventArgs> PropertiesChanged;
        public event EventHandler<DeviceEventArgs> StateChanged;

        internal Device(Light hueDevice)
        {
            HueDevice = hueDevice;

            RefreshState();
        }

        public string Family => Client.Instance.Family;

        [Browsable(false)]
        public IReadOnlyCollection<IDeviceGroup> Groups { get; } // TODO

        public string Id => HueDevice.Id;
        public bool IsColor => Type.IndexOf("color", StringComparison.CurrentCultureIgnoreCase) >= 0; // TODO this is a little hacky
        public bool IsMultiZone => false; // TODO can the gradient strip be addressed as more than one zone without the entertainment API?
        public string LuminaireUniqueId => HueDevice.LuminaireUniqueId;
        public string ModelId => HueDevice.ModelId;
        public string Name => HueDevice.Name;
        public string Product => HueDevice.ProductId;
        public string SoftwareVersion => HueDevice.SoftwareVersion;
        public string SwConfigId => HueDevice.SwConfigId;
        public string Type => HueDevice.Type;
        public string Uuid => HueDevice.UniqueId;
        public string Vendor => HueDevice.ManufacturerName;
        public int ZoneCount => 1;

        [Browsable(false)]
        public Color Color { get => Colors.FromHueColor(HueDevice.State); set => SetColor(value, TimeSpan.Zero); }

        [Browsable(false)]
        public IReadOnlyCollection<Color> MultiZoneColors { get => new[] { Color }; set => SetMultiZoneColors(value, TimeSpan.Zero); }

        [Browsable(false)]
        public PowerState Power { get => HueDevice.State.On ? PowerState.On : PowerState.Off; set => SetPower(value); }

        public override string ToString()
        {
            return $"{Name} ({Family})";
        }

        void OnPropertiesChanged()
        {
            PropertiesChanged?.Invoke(this, new DeviceEventArgs { Device = this });
            Client.Instance.OnPropertiesChanged(this);
        }

        void OnStateChanged()
        {
            StateChanged?.Invoke(this, new DeviceEventArgs { Device = this });
            Client.Instance.OnStateChanged(this);
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

        public async void RefreshState()
        {
            HueDevice = await Client.Instance.HueClient.GetLightAsync(HueDevice.Id);
            OnStateChanged();
        }

        public void SetColor(Color color, TimeSpan transitionDuration)
        {
            // TODO white + color temperature doesn't quite seem to work
            color.ToHueColor(HueDevice.State);

            var cmd = new LightCommand
            {
                Hue = HueDevice.State.Hue,
                Saturation = HueDevice.State.Saturation,
                Brightness = HueDevice.State.Brightness,
                ColorTemperature = HueDevice.State.ColorTemperature,
                TransitionTime = transitionDuration,
                Effect = Effect.None
            };

            cmd.SendCommandAsync(new[] { HueDevice.Id });
            OnStateChanged();
        }

        public void SetFirmwareEffect(object effect)
        {
            if(effect is string effectName)
            {
                if(Enum.TryParse(effectName, true, out Effect value))
                {
                    var cmd = new LightCommand { Effect = value };

                    cmd.SendCommandAsync(new[] { HueDevice.Id });
                }
                else
                {
                    Debug.Warning(this, $"Unknown effect: {effectName}");
                }
            }
            else
            {
                Debug.Warning(this, "Invalid effect settings");
            }
        }

        public void SetMultiZoneColors(IReadOnlyCollection<Color> colors, TimeSpan transitionDuration)
        {
            SetColor(colors.First(), transitionDuration);
        }

        public void SetPower(PowerState power)
        {
            HueDevice.State.On = power == PowerState.On;

            var cmd = new LightCommand { On = HueDevice.State.On };

            cmd.SendCommandAsync(new[] { HueDevice.Id });
            OnStateChanged();
        }

        #endregion
    }
}
