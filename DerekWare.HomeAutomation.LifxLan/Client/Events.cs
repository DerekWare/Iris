using System;
using DerekWare.Collections;
using DerekWare.Diagnostics;
using DerekWare.HomeAutomation.Common;

namespace DerekWare.HomeAutomation.Lifx.Lan
{
    public partial class Client
    {
        public event EventHandler<DeviceEventArgs> DeviceDiscovered
        {
            add
            {
                _DeviceDiscovered += value;
                InternalDevices.ForEach(i => OnDeviceDiscovered(i.Value));
            }
            remove => _DeviceDiscovered -= value;
        }

        event EventHandler<DeviceEventArgs> _DeviceDiscovered;

        internal void OnDeviceDiscovered(IDevice device)
        {
            OnDeviceDiscovered(new DeviceEventArgs { Device = device });
        }

        internal void OnDeviceDiscovered(DeviceEventArgs e)
        {
            Debug.Trace(null, $"Discovered device {e.Device}");
            _DeviceDiscovered?.Invoke(this, e);
        }
    }
}
