using DerekWare.Collections;
using DerekWare.HomeAutomation.Common;
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

        public override string Vendor => null;

        internal SynchronizedHashSet<IDevice> InternalChildren => Children;

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
