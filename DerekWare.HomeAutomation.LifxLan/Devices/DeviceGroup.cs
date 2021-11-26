using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common;
using DerekWare.HomeAutomation.Lifx.Lan.Messages;

namespace DerekWare.HomeAutomation.Lifx.Lan.Devices
{
    public class DeviceGroup : Common.DeviceGroup
    {
        internal readonly SynchronizedHashSet<Device> InternalDevices = new();

        public override event EventHandler<DeviceEventArgs> PropertiesChanged;
        public override event EventHandler<DeviceEventArgs> StateChanged;

        internal DeviceGroup(GroupResponse response)
            : this()
        {
            Name = response.Label;
            Uuid = response.Uuid;
        }

        internal DeviceGroup(LocationResponse response)
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

        public override string ToString()
        {
            return $"{Name} ({Family})";
        }

        #region Event Handlers

        void OnContentsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach(Device device in e.OldItems.SafeEmpty())
            {
                device.StateChanged -= OnStateChanged;
            }

            foreach(Device device in e.NewItems.SafeEmpty())
            {
                device.StateChanged += OnStateChanged;
            }

            OnStateChanged(null, null);
            OnPropertiesChanged(null, null);
        }

        void OnPropertiesChanged(object sender, DeviceEventArgs e)
        {
            PropertiesChanged?.Invoke(this, new DeviceEventArgs { Device = this });
            Lan.Client.Instance.OnPropertiesChanged(this);
        }

        void OnStateChanged(object sender, DeviceEventArgs e)
        {
            StateChanged?.Invoke(this, new DeviceEventArgs { Device = this });
            Lan.Client.Instance.OnStateChanged(this);
        }

        #endregion
    }
}
