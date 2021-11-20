using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common;
using DerekWare.HomeAutomation.Lifx.Lan.Messages;

namespace DerekWare.HomeAutomation.Lifx.Lan.Devices
{
    public class DeviceGroup : Common.DeviceGroup
    {
        internal readonly SynchronizedList<Device> InternalDevices = new();

        public override event EventHandler<DeviceEventArgs> PropertiesChanged;
        public override event EventHandler<DeviceEventArgs> StateChanged;

        internal DeviceGroup(StateGroup response)
            : this()
        {
            Name = response.Label;
            Uuid = response.Uuid;
        }

        internal DeviceGroup(StateLocation response)
            : this()
        {
            Name = response.Label;
            Uuid = response.Uuid;
        }

        DeviceGroup()
        {
            InternalDevices.CollectionChanged += OnContentsChanged;
        }

        [Browsable(false), XmlIgnore]
        public override IClient Client => Lan.Client.Instance;

        [Browsable(false), XmlIgnore]
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
            switch(e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach(Device device in e.NewItems)
                    {
                        device.StateChanged += OnStateChanged;
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Reset:
                    foreach(Device device in e.NewItems)
                    {
                        device.StateChanged -= OnStateChanged;
                    }

                    break;
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
