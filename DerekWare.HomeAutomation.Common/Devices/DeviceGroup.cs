using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.HomeAutomation.Common.Effects;
using DerekWare.Strings;

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

        public int DeviceCount => Devices.Count;

        public string DeviceNames => Devices.Select(i => i.Name).Join(", ");

        [Browsable(false)]
        public override IReadOnlyCollection<IDeviceGroup> Groups => Array.Empty<IDeviceGroup>();

        public override bool IsColor { get { return Devices.Any(i => i.IsColor); } }

        public override bool IsMultiZone => true;

        [Browsable(false)]
        public override bool IsValid => Devices.All(i => i.IsValid);

        [Browsable(false)]
        public override string Product => null;

        public override int ZoneCount { get { return Devices.Sum(i => i.ZoneCount); } }

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
            get { return Devices.SelectMany(i => i.MultiZoneColors).ToArray(); }
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

        public override void SetMultiZoneColors(IReadOnlyCollection<Color> colors, TimeSpan transitionDuration)
        {
            var count = ZoneCount - colors.Count;
            var index = 0;

            if(count > 0)
            {
                colors = colors.Append(colors.Last().Repeat(count)).ToArray();
            }

            foreach(var device in Devices)
            {
                count = device.ZoneCount;
                device.SetMultiZoneColors(colors.Skip(index).Take(count).ToArray(), transitionDuration);
                index += count;
            }
        }

        public override void SetPower(PowerState power)
        {
            Devices.ForEach(i => i.SetPower(power));
        }

        #endregion
    }
}
