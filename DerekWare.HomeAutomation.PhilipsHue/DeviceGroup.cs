using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using DerekWare.Collections;
using DerekWare.Diagnostics;
using DerekWare.HomeAutomation.Common;
using Q42.HueApi;
using Q42.HueApi.Models.Groups;

namespace DerekWare.HomeAutomation.PhilipsHue
{
    public class DeviceGroup : Common.DeviceGroup
    {
        internal readonly Group HueDevice;
        internal readonly SynchronizedList<Device> InternalDevices = new();

        public override event EventHandler<DeviceEventArgs> PropertiesChanged;
        public override event EventHandler<DeviceEventArgs> StateChanged;

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

        [Browsable(false), XmlIgnore]
        public override IReadOnlyCollection<IDevice> Devices => InternalDevices;

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

        public override void SetFirmwareEffect(object effect)
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
            PhilipsHue.Client.Instance.OnPropertiesChanged(this);
        }

        void OnStateChanged(object sender, DeviceEventArgs e)
        {
            StateChanged?.Invoke(this, new DeviceEventArgs { Device = this });
            PhilipsHue.Client.Instance.OnStateChanged(this);
        }

        #endregion
    }
}
