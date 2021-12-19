using System.Collections.Generic;
using System.Linq;
using DerekWare.Collections;

namespace DerekWare.HomeAutomation.Common
{
    public static class DeviceExtensions
    {
        public static IReadOnlyCollection<IDeviceGroup> GetDeviceGroups(this IDevice device)
        {
            return (device?.Client?.Groups?.Where(i => i.Devices.Contains(device))).SafeEmpty().ToArray();
        }

        public static IReadOnlyCollection<IDevice> GetDevices(this IDevice device)
        {
            return device is IDeviceGroup group ? group.Devices : new[] { device };
        }

        public static bool IsCompatible(this IFamily x, IFamily y)
        {
            return IsCompatible(x, y.Family);
        }

        public static bool IsCompatible(this IFamily x, string y)
        {
            return x.Family.IsNullOrEmpty() || y.IsNullOrEmpty() || x.Family.Equals(y);
        }
    }
}
