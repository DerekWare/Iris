using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common;
using DerekWare.HomeAutomation.Lifx.Lan.Messages;

namespace DerekWare.HomeAutomation.Lifx.Lan.Devices
{
    public sealed class DeviceGroup : Common.DeviceGroup
    {
        internal readonly SynchronizedHashSet<Device> InternalDevices = new();

        internal DeviceGroup(GroupResponse response)
            : this()
        {
            Name = response.Label;
            Uuid = response.Uuid;
        }

        DeviceGroup()
        {
            InternalDevices.CollectionChanged += OnContentsChanged;
        }

        [Browsable(false)]
        public override IClient Client => Lan.Client.Instance;

        [Browsable(false)]
        public override IReadOnlyCollection<IDevice> Devices => InternalDevices;

        public override string Name { get; }

        public override string Uuid { get; }

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

        protected override void OnPropertiesChanged()
        {
            base.OnPropertiesChanged();
            Lan.Client.Instance.OnPropertiesChanged(this);
        }

        protected override void OnStateChanged()
        {
            base.OnStateChanged();
            Lan.Client.Instance.OnStateChanged(this);
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
