using System.Collections.Generic;
using System.Linq;
using DerekWare.Collections;

namespace DerekWare.HomeAutomation.Common
{
    public static partial class Extensions
    {
        /// <summary>
        ///     Finds all groups for which this device is a first-level member.
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public static IEnumerable<IDeviceGroup> GetDeviceGroups(this IDevice device)
        {
            return (device?.Client?.Groups).SafeEmpty().Where(i => i.Children.Contains(device));
        }

        /// <summary>
        ///     Returns all devices that are members of this device group, or just the given device if it's not a group. This
        ///     function behaves recursively, allowing for groups within groups.
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public static IEnumerable<IDevice> GetDevices(this IDevice device)
        {
            if(device is not IDeviceGroup group)
            {
                yield return device;
                yield break;
            }

            foreach(var i in group.Children)
            foreach(var j in GetDevices(i))
            {
                yield return j;
            }
        }

        /// <summary>
        ///     Compares to families for equality. If either family is null, they are considered compatible.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool IsCompatible(this IFamily x, IFamily y)
        {
            return IsCompatible(x, y.Family);
        }

        /// <summary>
        ///     Compares to families for equality. If either family is null, they are considered compatible.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool IsCompatible(this IFamily x, string y)
        {
            return x.Family.IsNullOrEmpty() || y.IsNullOrEmpty() || x.Family.Equals(y);
        }
    }
}
