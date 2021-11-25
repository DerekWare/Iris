using System;
using System.Collections.Generic;

namespace DerekWare.HomeAutomation.Common
{
    public interface IClient : IFamily, IDisposable
    {
        // Called when a new device or device group is found
        event EventHandler<DeviceEventArgs> DeviceDiscovered;

        // Called when the device or group properties have changed. Device properties
        // normally only change during device discovery and include things like the
        // device name, product identifier and group membership.
        event EventHandler<DeviceEventArgs> PropertiesChanged;

        // Call when the device or group state has changed. Device state includes
        // power state, color and other values that can be changed at runtime.
        event EventHandler<DeviceEventArgs> StateChanged;

        IReadOnlyCollection<IDevice> Devices { get; }
        IReadOnlyCollection<IDeviceGroup> Groups { get; }

        // The minimum amount of time between messages sent to this client to prevent overrun.
        TimeSpan MinMessageInterval { get; }
    }
}
