using System.Collections.Generic;
using DerekWare.Collections;

namespace DerekWare.HomeAutomation.Common
{
    public static class DeviceExtensions
    {
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
