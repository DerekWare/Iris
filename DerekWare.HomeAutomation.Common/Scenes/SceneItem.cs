using System;
using System.Collections.Generic;
using System.Linq;
using DerekWare.Collections;
using DerekWare.Diagnostics;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.HomeAutomation.Common.Effects;
using DerekWare.HomeAutomation.Common.Themes;

namespace DerekWare.HomeAutomation.Common.Scenes
{
    public class SceneItem : IEquatable<SceneItem>, IEquatable<IDevice>
    {
        public SceneItem()
        {
        }

        public SceneItem(IDevice device)
        {
            // TODO currently no way to query the active theme
            Name = device.Name;
            DeviceFamily = device.Family;
            DeviceUuid = device.Uuid;
            Power = PowerState.On;
            Colors = device.MultiZoneColors.ToList();
            Effect = device.Effect;
        }

        public List<Color> Colors { get; set; }
        public string DeviceFamily { get; set; }
        public string DeviceUuid { get; set; }
        public Effect Effect { get; set; }
        public string Name { get; set; }
        public PowerState Power { get; set; }
        public Theme Theme { get; set; }

        public void Apply()
        {
            // Find the device by family and UUID
            var client = ClientFactory.Instance.CreateInstance(DeviceFamily);

            if(client is null)
            {
                Debug.Error(this, $"Unable to find client for family {DeviceFamily}");
                return;
            }

            var device = client.Devices.FirstOrDefault(i => i.Uuid.Equals(DeviceUuid)) ?? client.Groups.FirstOrDefault(i => i.Uuid.Equals(DeviceUuid));

            if(device is null)
            {
                Debug.Error(this, $"Unable to find device or group for UUID {DeviceUuid}");
                return;
            }

            device.SetPower(Power);

            if(Theme is not null)
            {
                Theme.Apply(device);
            }
            else if(!Colors.IsNullOrEmpty())
            {
                if(Colors.Count == 1)
                {
                    device.SetColor(Colors[0], TimeSpan.Zero);
                }
                else
                {
                    device.SetMultiZoneColors(Colors, TimeSpan.Zero);
                }
            }

            device.Effect = Effect;
        }

        #region Equality

        public bool Equals(IDevice other)
        {
            if(ReferenceEquals(null, other))
            {
                return false;
            }

            return Equals(DeviceFamily, other.Family) && Equals(DeviceUuid, other.Uuid);
        }

        public bool Equals(SceneItem other)
        {
            if(ReferenceEquals(null, other))
            {
                return false;
            }

            if(ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(DeviceFamily, other.DeviceFamily) && Equals(DeviceUuid, other.DeviceUuid);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj))
            {
                return false;
            }

            if(ReferenceEquals(this, obj))
            {
                return true;
            }

            if(obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((SceneItem)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((DeviceFamily.IsNullOrEmpty() ? 0 : DeviceFamily.GetHashCode()) * 397) ^ (DeviceUuid.IsNullOrEmpty() ? 0 : DeviceUuid.GetHashCode());
            }
        }

        public static bool operator ==(SceneItem left, SceneItem right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SceneItem left, SceneItem right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}
