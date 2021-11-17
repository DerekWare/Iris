using System;
using System.Collections.Generic;
using DerekWare.HomeAutomation.Common.Colors;

namespace DerekWare.HomeAutomation.Common
{
    public interface IDevice : IDeviceProperties, IDeviceState, IEquatable<IDevice>
    {
        event EventHandler<DeviceEventArgs> PropertiesChanged;
        event EventHandler<DeviceEventArgs> StateChanged;

        IReadOnlyCollection<IDeviceGroup> Groups { get; }
    }

    // Properties generally don't change at runtime except when first connecting to the device
    public interface IDeviceProperties : IName, IFamily
    {
        bool IsColor { get; }
        bool IsMultiZone { get; }
        string Product { get; }
        string Uuid { get; }
        string Vendor { get; }
        int ZoneCount { get; }
    }

    // State changes based on user interaction
    public interface IDeviceState
    {
        Color Color { get; set; }
        IReadOnlyCollection<Color> MultiZoneColors { get; set; }
        PowerState Power { get; set; }

        void RefreshState();
        void SetColor(Color color, TimeSpan transitionDuration);
        void SetFirmwareEffect(object effect);
        void SetMultiZoneColors(IReadOnlyCollection<Color> colors, TimeSpan transitionDuration);
        void SetPower(PowerState power);
    }
}
