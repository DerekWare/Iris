using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.HomeAutomation.Common.Effects;

namespace DerekWare.HomeAutomation.Common
{
    public interface IDeviceGroup : IDevice
    {
        int DeviceCount { get; }
        IReadOnlyCollection<IDevice> Devices { get; }
    }

    // Helper class that can be used to implement a Group for a device family. When setting
    // properties on the group, this class propagates the changes to all devices within
    // the group. Lights are split into collections of multizone and single zone, with all
    // single zone lights treated as a single multizone light, providing a better experience
    // with multizone themes and effects.
    public abstract class DeviceGroup : Device, IDeviceGroup
    {
        public abstract IReadOnlyCollection<IDevice> Devices { get; }

        public virtual int DeviceCount => Devices.Count;

        [Browsable(false)]
        public override IReadOnlyCollection<IDeviceGroup> Groups => Array.Empty<IDeviceGroup>();

        public override bool IsColor { get { return Devices.Any(i => i.IsColor); } }

        public override bool IsMultiZone => true;

        [Browsable(false)]
        public override bool IsValid => Devices.All(i => i.IsValid);

        [Browsable(false)]
        public override string Product => null;

        public override int ZoneCount
        {
            get
            {
                SplitDeviceTypes(Devices, out var multizone, out var singles);

                var multizoneCount = multizone.IsNullOrEmpty() ? 0 : multizone.Max(i => i.ZoneCount);
                var singleCount = singles.Count;

                return Math.Max(1, Math.Max(multizoneCount, singleCount));
            }
        }

        [Browsable(false)]
        public override Color Color { get => Devices.FirstOrDefault()?.Color ?? new Color(); set => base.Color = value; }

        public override Effect Effect
        {
            get => base.Effect;
            set
            {
                Devices.ForEach(i => i.Effect = null);
                base.Effect = value;
            }
        }

        [Browsable(false)]
        public override IReadOnlyCollection<Color> MultiZoneColors
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
            set => base.MultiZoneColors = value;
        }

        [Browsable(false)]
        public override PowerState Power
        {
            get { return Devices.Any(i => i.Power == PowerState.On) ? PowerState.On : PowerState.Off; }
            set => base.Power = value;
        }

        #region Equality

        public bool Equals(DeviceGroup other)
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

        #endregion

        #region IDeviceState

        public override void RefreshState()
        {
            Devices.ForEach(i => i.RefreshState());
        }

        public override void SetColor(Color color, TimeSpan transitionDuration)
        {
            Devices.ForEach(i => i.SetColor(color, transitionDuration));
        }

        public override void SetFirmwareEffect(object effect)
        {
            Devices.ForEach(i => i.SetFirmwareEffect(effect));
        }

        public override void SetMultiZoneColors(IReadOnlyCollection<Color> _colors, TimeSpan transitionDuration)
        {
            SplitDeviceTypes(Devices, out var multizone, out var singles);

            var colors = _colors.ToArray();

            foreach(var device in multizone)
            {
                var c = new Color[device.ZoneCount];

                for(var i = 0; i < c.Length; ++i)
                {
                    var j = (i * colors.Length) / c.Length;
                    c[i] = colors[j];
                }

                device.SetMultiZoneColors(c, transitionDuration);
            }

            {
                var c = new Color[singles.Count];

                for(var i = 0; i < c.Length; ++i)
                {
                    var j = (i * colors.Length) / c.Length;
                    c[i] = colors[j];
                }

                for(var i = 0; i < c.Length; ++i)
                {
                    singles[i].SetColor(c[i], transitionDuration);
                }
            }
        }

        public override void SetPower(PowerState power)
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
