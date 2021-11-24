using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.HomeAutomation.Common.Effects;

namespace DerekWare.HomeAutomation.Common
{
    public interface IDeviceGroup : IDevice, IEquatable<IDeviceGroup>
    {
        int DeviceCount { get; }
        IReadOnlyCollection<IDevice> Devices { get; }
    }

    // Helper class that can be used to implement a Group for a device family. When setting
    // properties on the group, this class propagates the changes to all devices within
    // the group. Lights are split into collections of multizone and single zone, with all
    // single zone lights treated as a single multizone light, providing a better experience
    // with multizone scenes and effects.
    public abstract class DeviceGroup : IDeviceGroup
    {
        public abstract event EventHandler<DeviceEventArgs> PropertiesChanged;
        public abstract event EventHandler<DeviceEventArgs> StateChanged;

        public abstract IClient Client { get; }
        public abstract IReadOnlyCollection<IDevice> Devices { get; }
        public abstract string Name { get; }
        public abstract string Uuid { get; }
        public abstract string Vendor { get; }

        #region IDisposable

        public abstract void Dispose();

        #endregion

        public virtual int DeviceCount => Devices.Count;

        [Browsable(false), XmlIgnore]
        public IReadOnlyCollection<IEffect> Effects => EffectFactory.Instance.GetRunningEffects(this).ToList();

        public virtual string Family => Client.Family;

        [Browsable(false), XmlIgnore]
        public virtual IReadOnlyCollection<IDeviceGroup> Groups => Array.Empty<IDeviceGroup>();

        public virtual bool IsColor { get { return Devices.Any(i => i.IsColor); } }
        public virtual bool IsMultiZone => true;

        [Browsable(false), XmlIgnore]
        public bool IsValid => Devices.Any(i => i.IsValid);

        [Browsable(false), XmlIgnore]
        public virtual string Product => null;

        public virtual int ZoneCount
        {
            get
            {
                SplitDeviceTypes(Devices, out var multizone, out var singles);

                var multizoneCount = multizone.IsNullOrEmpty() ? 0 : multizone.Max(i => i.ZoneCount);
                var singleCount = singles.Count;

                return Math.Max(multizoneCount, singleCount);
            }
        }

        [Browsable(false), XmlIgnore]
        public virtual Color Color { get => Devices.FirstOrDefault()?.Color ?? new Color(); set => SetColor(value, TimeSpan.Zero); }

        [Browsable(false), XmlIgnore]
        public virtual IReadOnlyCollection<Color> MultiZoneColors
        {
            get
            {
                SplitDeviceTypes(Devices, out var multizone, out var singles);

                multizone = multizone.OrderByDescending(i => i.ZoneCount).ToList();

                if(multizone.Count > 0)
                {
                    if(multizone[0].ZoneCount >= singles.Count)
                    {
                        return multizone[0].MultiZoneColors;
                    }
                }

                return singles.Select(i => i.Color).ToList();
            }
            set => SetMultiZoneColors(value, TimeSpan.Zero);
        }

        [Browsable(false), XmlIgnore]
        public PowerState Power { get { return Devices.Any(i => i.Power == PowerState.On) ? PowerState.On : PowerState.Off; } set => SetPower(value); }

        public override string ToString()
        {
            return $"{Name} ({Family})";
        }

        #region Equality

        public virtual bool Equals(IDevice other)
        {
            return Equals(other as IDeviceGroup);
        }

        public virtual bool Equals(IDeviceGroup other)
        {
            if(ReferenceEquals(null, other))
            {
                return false;
            }

            if(ReferenceEquals(this, other))
            {
                return true;
            }

            if(other.GetType() != GetType())
            {
                return false;
            }

            return Uuid.Equals(other.Uuid);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IDeviceGroup);
        }

        public override int GetHashCode()
        {
            return Uuid != null ? Uuid.GetHashCode() : 0;
        }

        #endregion

        #region IDeviceState

        public virtual void RefreshState()
        {
            Devices.ForEach(i => i.RefreshState());
        }

        public virtual void SetColor(Color color, TimeSpan transitionDuration)
        {
            Devices.ForEach(i => i.SetColor(color, transitionDuration));
        }

        public virtual void SetFirmwareEffect(object effect)
        {
            Devices.ForEach(i => i.SetFirmwareEffect(effect));
        }

        public virtual void SetMultiZoneColors(IReadOnlyCollection<Color> _colors, TimeSpan transitionDuration)
        {
            SplitDeviceTypes(Devices, out var multizone, out var singles);

            var colors = _colors.ToArray();

            foreach(var device in multizone)
            {
                var c = new Color[device.ZoneCount];

                for(var i = 0; i < c.Length; ++i)
                {
                    var j = (i * ZoneCount) / c.Length;
                    c[i] = colors[j];
                }

                device.SetMultiZoneColors(c, transitionDuration);
            }

            {
                var c = new Color[singles.Count];

                for(var i = 0; i < c.Length; ++i)
                {
                    var j = (i * ZoneCount) / c.Length;
                    c[i] = colors[j];
                }

                for(var i = 0; i < c.Length; ++i)
                {
                    singles[i].SetColor(c[i], transitionDuration);
                }
            }
        }

        public virtual void SetPower(PowerState power)
        {
            Devices.ForEach(i => i.SetPower(power));
        }

        #endregion

        protected static void SplitDeviceTypes(IReadOnlyCollection<IDevice> devices, out List<IDevice> multizone, out List<IDevice> singles)
        {
            multizone = new List<IDevice>();
            singles = new List<IDevice>();

            foreach(var i in devices)
            {
                if(i.IsMultiZone)
                {
                    multizone.Add(i);
                }
                else
                {
                    singles.Add(i);
                }
            }
        }
    }
}
