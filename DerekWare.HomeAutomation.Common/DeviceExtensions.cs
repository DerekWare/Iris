using System.Collections.Generic;

namespace DerekWare.HomeAutomation.Common
{
    public static class DeviceExtensions
    {
        public static IReadOnlyCollection<IDevice> GetDevices(this IDevice device)
        {
            return device is IDeviceGroup group ? group.Devices : new[] { device };
        }
    }
}
