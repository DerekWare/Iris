using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DerekWare.HomeAutomation.Common;
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

        internal Device(Light hueDevice)
        {
            SetState(hueDevice);
        }

        [Browsable(false)]
        public override IClient Client => PhilipsHue.Client.Instance;

        [Browsable(false)]
        public override IReadOnlyCollection<IDeviceGroup> Groups => PhilipsHue.Client.Instance.Groups.Where(i => i.Children.Contains(this)).ToList();

        public string Id => HueDevice.Id;

        public override bool IsColor => Type.IndexOf("color", StringComparison.CurrentCultureIgnoreCase) >= 0; // TODO this is a little hacky

        public override bool IsMultiZone => false; // TODO can the gradient strip be addressed as more than one zone without the entertainment API?

        [Browsable(false)]
        public override bool IsValid => true;

        public string LuminaireUniqueId => HueDevice.LuminaireUniqueId;

        public string ModelId => HueDevice.ModelId;

        public override string Name => HueDevice.Name;

        public string Product => HueDevice.ProductId;

        public string SoftwareConfigId => HueDevice.SwConfigId;

        public string SoftwareVersion => HueDevice.SoftwareVersion;

        public string Type => HueDevice.Type;

        public override string Uuid => HueDevice.UniqueId;

        public string Vendor => HueDevice.ManufacturerName;

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
            HueDevice = hueDevice;

            SetPower(HueDevice.State.On ? PowerState.On : PowerState.Off, false);

            var color = Colors.FromState(HueDevice.State);

            if(color is not null)
            {
                SetColor(new[] { Colors.FromState(HueDevice.State) }, TimeSpan.Zero, false);
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
            cmd.SendCommand(HueDevice);
        }

        #endregion
    }
}
