using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common;
using Q42.HueApi.Models.Groups;

namespace DerekWare.HomeAutomation.PhilipsHue
{
    public class DeviceGroup : Common.DeviceGroup
    {
        internal readonly Group HueDevice;
        internal readonly SynchronizedList<Device> InternalDevices = new();

        SynchronizedList<Device> SortedDevices = new();

        internal DeviceGroup(Group hueDevice)
        {
            HueDevice = hueDevice;
            InternalDevices.CollectionChanged += OnContentsChanged;

            foreach(var i in HueDevice.Lights)
            {
                if(PhilipsHue.Client.Instance.InternalDevices.TryGetValue(i, out var device))
                {
                    InternalDevices.Add(device);
                }
            }
        }

        public RoomClass? Class => HueDevice.Class;

        public override IClient Client => PhilipsHue.Client.Instance;

        [Browsable(false)]
        public override IReadOnlyCollection<IDevice> Devices => SortedDevices;

        public string ModelId => HueDevice.ModelId;

        public override string Name => HueDevice.Name;

        public GroupType? Type => HueDevice.Type;

        public override string Uuid => HueDevice.Id;

        public override string Vendor => null;

        public override void Dispose()
        {
            List<Device> devices;

            lock(InternalDevices.SyncRoot)
            {
                devices = InternalDevices.ToList();
                InternalDevices.Clear();
            }

            foreach(var device in devices)
            {
                device.Dispose();
            }
        }

        public override string ToString()
        {
            return $"{Name} ({Family})";
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

        #region Event Handlers

        void OnContentsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach(Device device in e.OldItems.SafeEmpty())
            {
                device.StateChanged -= OnDeviceStateChanged;
            }

            foreach(Device device in e.NewItems.SafeEmpty())
            {
                device.StateChanged += OnDeviceStateChanged;
            }

            SortedDevices = new SynchronizedList<Device>(InternalDevices.OrderBy(i => i.Name));

            OnStateChanged();
            OnPropertiesChanged();
        }

        void OnDeviceStateChanged(object sender, DeviceEventArgs e)
        {
            OnStateChanged();
        }

        #endregion
    }
}
