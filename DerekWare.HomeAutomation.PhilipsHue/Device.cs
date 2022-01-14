using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DerekWare.HomeAutomation.Common;
using DerekWare.HomeAutomation.Common.Colors;
using Q42.HueApi;

namespace DerekWare.HomeAutomation.PhilipsHue
{
    interface IHueDevice : IDevice
    {
        public void SendCommand(LightCommand cmd);
    }

    class Device : Common.Device, IHueDevice
    {
        protected Light HueDevice;

        readonly object SyncRoot = new();

        internal Device(Light hueDevice)
        {
            SetState(hueDevice);
        }

        [Browsable(false)]
        public override IClient Client => PhilipsHue.Client.Instance;

        [Browsable(false)]
        public override IReadOnlyCollection<IDeviceGroup> Groups => PhilipsHue.Client.Instance.Groups.Where(i => i.Children.Contains(this)).ToList();

        public string Id
        {
            get
            {
                lock(SyncRoot)
                {
                    return HueDevice.Id;
                }
            }
        }

        public override bool IsColor => Type.IndexOf("color", StringComparison.CurrentCultureIgnoreCase) >= 0; // TODO this is a little hacky

        public override bool IsMultiZone => false; // TODO can the gradient strip be addressed as more than one zone without the entertainment API?

        [Browsable(false)]
        public override bool IsValid => true;

        public string LuminaireUniqueId
        {
            get
            {
                lock(SyncRoot)
                {
                    return HueDevice.LuminaireUniqueId;
                }
            }
        }

        public string ModelId
        {
            get
            {
                lock(SyncRoot)
                {
                    return HueDevice.ModelId;
                }
            }
        }

        public override string Name
        {
            get
            {
                lock(SyncRoot)
                {
                    return HueDevice.Name;
                }
            }
        }

        public string Product
        {
            get
            {
                lock(SyncRoot)
                {
                    return HueDevice.ProductId;
                }
            }
        }

        public string SoftwareConfigId
        {
            get
            {
                lock(SyncRoot)
                {
                    return HueDevice.SwConfigId;
                }
            }
        }

        public string SoftwareVersion
        {
            get
            {
                lock(SyncRoot)
                {
                    return HueDevice.SoftwareVersion;
                }
            }
        }

        public string Type
        {
            get
            {
                lock(SyncRoot)
                {
                    return HueDevice.Type;
                }
            }
        }

        public override string Uuid
        {
            get
            {
                lock(SyncRoot)
                {
                    return HueDevice.UniqueId;
                }
            }
        }

        public string Vendor
        {
            get
            {
                lock(SyncRoot)
                {
                    return HueDevice.ManufacturerName;
                }
            }
        }

        public override int ZoneCount => 1;

        protected override void ApplyColor(IReadOnlyCollection<Color> colors, TimeSpan transitionDuration)
        {
            // TODO white + color temperature doesn't quite seem to work
            SendCommand(colors.First().ToLightCommand());
        }

        protected override void ApplyPower(PowerState power)
        {
            SendCommand(new LightCommand { On = power == PowerState.On });
        }

        protected override void OnPropertiesChanged()
        {
            base.OnPropertiesChanged();
            PhilipsHue.Client.Instance.OnPropertiesChanged(this);
        }

        protected override void OnStateChanged()
        {
            base.OnStateChanged();
            PhilipsHue.Client.Instance.OnStateChanged(this);
        }

        internal void SetState(Light hueDevice)
        {
            lock(SyncRoot)
            {
                HueDevice = hueDevice;

                SetPower(HueDevice.State.On ? PowerState.On : PowerState.Off, false);

                var color = Colors.FromState(HueDevice.State);

                if(color is not null)
                {
                    SetColor(new[] { Colors.FromState(HueDevice.State) }, TimeSpan.Zero, false);
                }
            }
        }

        #region IDeviceState

        public override async void RefreshState()
        {
            var hueDevice = await PhilipsHue.Client.Instance.GetLightById(Id);

            if(hueDevice is null)
            {
                return;
            }

            SetState(hueDevice);
        }

        public override void SetFirmwareEffect(object effect)
        {
            SendCommand(new LightCommand { Effect = Extensions.GetEffectType(effect) });
        }

        #endregion

        #region IHueDevice

        public void SendCommand(LightCommand cmd)
        {
            lock(SyncRoot)
            {
                cmd.SendCommand(HueDevice);
            }
        }

        #endregion
    }
}
