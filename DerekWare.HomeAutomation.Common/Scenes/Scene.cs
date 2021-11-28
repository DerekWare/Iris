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
    public interface IReadOnlySceneProperties : IName
    {
    }

    public interface ISceneProperties : IReadOnlySceneProperties
    {
        ICollection<SceneItem> Items { get; }
        new string Name { get; set; }
    }

    /// <summary>
    ///     A Scene is a collection of colors, themes and effects applied to any number
    ///     of devices that can be persisted in the settings.
    /// </summary>
    public class Scene : ISceneProperties, IEquatable<Scene>
    {
        public Scene()
        {
        }

        public Scene(string name)
        {
            Name = name;
        }

        public SynchronizedHashSet<SceneItem> Items { get; set; } = new();

        public string Name { get; set; }

        ICollection<SceneItem> ISceneProperties.Items => Items;

        public bool Add(IDevice device)
        {
            return Items.Add(new SceneItem(device));
        }

        public void Apply()
        {
            Items.ForEach(i => i.Apply());
        }

        public bool Contains(IDevice device)
        {
            return Items.Any(i => i.Equals(device));
        }

        public bool Remove(IDevice device)
        {
            return Items.RemoveWhere(i => i.Equals(device)) > 0;
        }

        #region Equality

        public bool Equals(Scene other)
        {
            if(ReferenceEquals(null, other))
            {
                return false;
            }

            if(ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj))
            {
                return false;
            }

            if(obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((Scene)obj);
        }

        public override int GetHashCode()
        {
            return Name.IsNullOrEmpty() ? 0 : Name.GetHashCode();
        }

        public static bool operator ==(Scene left, Scene right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Scene left, Scene right)
        {
            return !Equals(left, right);
        }

        #endregion
    }

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

            if(device.Effect is not null)
            {
                Effect = new PropertyBag(device.Effect) { [nameof(device.Effect.Name)] = device.Effect.Name };
            }
        }

        public List<Color> Colors { get; set; }
        public string DeviceFamily { get; set; }
        public string DeviceUuid { get; set; }
        public PropertyBag Effect { get; set; }
        public string Name { get; set; }
        public PowerState Power { get; set; }
        public PropertyBag Theme { get; set; }

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
                var theme = ThemeFactory.Instance.CreateInstance((string)Theme["Name"]);

                if(theme is not null)
                {
                    Theme.WriteToObject(theme);
                    theme.Apply(device);
                }
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

            if(Effect is not null)
            {
                var effect = EffectFactory.Instance.CreateInstance((string)Effect["Name"]);

                if(effect is not null)
                {
                    Effect.WriteToObject(effect);
                    device.Effect = effect;
                }
            }
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
