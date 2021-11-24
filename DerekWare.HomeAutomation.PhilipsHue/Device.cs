﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using DerekWare.Diagnostics;
using DerekWare.HomeAutomation.Common;
using DerekWare.HomeAutomation.Common.Colors;
using Q42.HueApi;

namespace DerekWare.HomeAutomation.PhilipsHue
{
    public class Device : Common.Device
    {
        internal Light HueDevice;

        internal Device(Light hueDevice)
        {
            HueDevice = hueDevice;

            RefreshState();
        }

        [Browsable(false), XmlIgnore]
        public override IClient Client => PhilipsHue.Client.Instance;

        [Browsable(false), XmlIgnore]
        public override IReadOnlyCollection<IDeviceGroup> Groups => PhilipsHue.Client.Instance.Groups.Where(i => i.Devices.Contains(this)).ToList();

        public string Id => HueDevice.Id;

        public override bool IsColor => Type.IndexOf("color", StringComparison.CurrentCultureIgnoreCase) >= 0; // TODO this is a little hacky

        public override bool IsMultiZone => false; // TODO can the gradient strip be addressed as more than one zone without the entertainment API?

        [Browsable(false), XmlIgnore]
        public override bool IsValid => true;

        public string LuminaireUniqueId => HueDevice.LuminaireUniqueId;

        public string ModelId => HueDevice.ModelId;

        public override string Name => HueDevice.Name;

        public override string Product => HueDevice.ProductId;

        public string SoftwareVersion => HueDevice.SoftwareVersion;

        public string SwConfigId => HueDevice.SwConfigId;

        public string Type => HueDevice.Type;

        public override string Uuid => HueDevice.UniqueId;

        public override string Vendor => HueDevice.ManufacturerName;

        public override int ZoneCount => 1;

        public override async void RefreshState()
        {
            HueDevice = await PhilipsHue.Client.Instance.HueClient.GetLightAsync(HueDevice.Id);

            base.SetColor(Colors.FromState(HueDevice.State), TimeSpan.Zero);
            base.SetPower(HueDevice.State.On ? PowerState.On : PowerState.Off);

            StartRefreshTask();
        }

        public override void SetColor(Color color, TimeSpan transitionDuration)
        {
            // TODO white + color temperature doesn't quite seem to work
            base.SetColor(color, transitionDuration);
            Color.ToLightCommand().SendCommandAsync(new[] { HueDevice.Id });
        }

        public override void SetFirmwareEffect(object effect)
        {
            switch(effect)
            {
                case null:
                    new LightCommand { Effect = Effect.None }.SendCommandAsync(new[] { HueDevice.Id });
                    break;
                case string effectName:
                    new LightCommand { Effect = (Effect)Enum.Parse(typeof(Effect), effectName, true) }.SendCommandAsync(new[] { HueDevice.Id });
                    break;
                default:
                    Debug.Warning(this, "Invalid effect settings");
                    break;
            }
        }

        public override void SetMultiZoneColors(IReadOnlyCollection<Color> colors, TimeSpan transitionDuration)
        {
            SetColor(colors.First(), transitionDuration);
        }

        public override void SetPower(PowerState power)
        {
            base.SetPower(power);
            new LightCommand { On = Power == PowerState.On }.SendCommandAsync(new[] { HueDevice.Id });
        }

        protected override void OnPropertiesChanged(DeviceEventArgs e)
        {
            base.OnPropertiesChanged(e);
            PhilipsHue.Client.Instance.OnPropertiesChanged(this);
        }

        protected override void OnStateChanged(DeviceEventArgs e)
        {
            base.OnStateChanged(e);
            PhilipsHue.Client.Instance.OnStateChanged(this);
        }
    }
}
