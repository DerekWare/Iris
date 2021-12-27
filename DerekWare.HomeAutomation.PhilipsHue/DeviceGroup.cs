using System;
using System.Collections.Generic;
using System.Linq;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common;
using DerekWare.HomeAutomation.Common.Colors;
using Q42.HueApi;
using Q42.HueApi.Models.Groups;

namespace DerekWare.HomeAutomation.PhilipsHue
{
    public class DeviceGroup : Common.DeviceGroup
    {
        internal readonly Group HueDevice;

        internal DeviceGroup(Group hueDevice)
        {
            HueDevice = hueDevice;

            foreach(var i in HueDevice.Lights)
            {
                if(PhilipsHue.Client.Instance.InternalDevices.TryGetValue(i, out var device))
                {
                    Children.Add(device);
                }
            }
        }

        public RoomClass? Class => HueDevice.Class;

        public override IClient Client => PhilipsHue.Client.Instance;

        public string ModelId => HueDevice.ModelId;

        public override string Name => HueDevice.Name;

        public GroupType? Type => HueDevice.Type;

        public override string Uuid => HueDevice.Id;

        internal SynchronizedHashSet<IDevice> InternalChildren => Children;

        protected override void ApplyColor(IReadOnlyCollection<Color> color, TimeSpan transitionDuration)
        {
            if(color.Count > 1)
            {
                base.ApplyColor(color, transitionDuration);
            }
            else
            {
                color.First().ToLightCommand().SendCommand(HueDevice);
                Children.ForEach<Device>(i => i.SetColor(color, TimeSpan.Zero, false));
            }
        }

        protected override void ApplyPower(PowerState power)
        {
            new LightCommand { On = power == PowerState.On }.SendCommand(HueDevice);
            Children.ForEach<Device>(i => i.SetPower(power, false));
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
    }
}
