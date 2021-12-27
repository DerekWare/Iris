﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        int ChildCount { get; }
        string ChildNames { get; }
        IReadOnlyCollection<IDevice> Children { get; }
    }

    // Helper class that can be used to implement a Group for a device family. When setting
    // properties on the group, this class propagates the changes to all devices within
    // the group. Lights are split into collections of multizone and single zone, with all
    // single zone lights treated as a single multizone light, providing a better experience
    // with multizone themes and effects.
    public abstract class DeviceGroup : Device, IDeviceGroup
    {
        protected SynchronizedHashSet<IDevice> Children = new();
        protected SynchronizedList<IDevice> SortedChildren = new();

        protected DeviceGroup()
        {
            Children.CollectionChanged += OnChildCollectionChanged;
        }

        public int ChildCount => SortedChildren.Count;

        public string ChildNames => SortedChildren.Select(i => i.Name).Join(", ");

        [Browsable(false)]
        public override IReadOnlyCollection<IDeviceGroup> Groups => Array.Empty<IDeviceGroup>(); // TODO allow groups within groups

        public override bool IsColor { get { return Children.Any(i => i.IsColor); } }

        public override bool IsMultiZone => true;

        public override bool IsValid => Children.All(i => i.IsValid);

        [Browsable(false)]
        public override string Product => null;

        public override int ZoneCount { get { return Children.Sum(i => i.ZoneCount); } }

        [Browsable(false)]
        public override Color Color { get => Children.FirstOrDefault()?.Color ?? new Color(); set => base.Color = value; }

        public override Effect Effect
        {
            get => base.Effect;
            set
            {
                Children.ForEach(i => i.Effect = null);
                base.Effect = value;
            }
        }

        [Browsable(false)]
        public override IReadOnlyCollection<Color> MultiZoneColors
        {
            get { return Children.SelectMany(i => i.MultiZoneColors).ToArray(); }
            set => base.MultiZoneColors = value;
        }

        [Browsable(false)]
        public override PowerState Power
        {
            get { return Children.Any(i => i.Power == PowerState.On) ? PowerState.On : PowerState.Off; }
            set => base.Power = value;
        }

        IReadOnlyCollection<IDevice> IDeviceGroup.Children => Children;

        #region IDeviceState

        public override void RefreshState()
        {
            Children.ForEach(i => i.RefreshState());
        }

        public override void SetColor(Color color, TimeSpan transitionDuration)
        {
            Children.ForEach(i => i.SetColor(color, transitionDuration));
        }

        public override void SetFirmwareEffect(object effect)
        {
            Children.ForEach(i => i.SetFirmwareEffect(effect));
        }

        public override void SetMultiZoneColors(IReadOnlyCollection<Color> colors, TimeSpan transitionDuration)
        {
            var count = ZoneCount - colors.Count;
            var index = 0;

            if(count > 0)
            {
                colors = colors.Append(colors.Last().Repeat(count)).ToArray();
            }

            foreach(var device in Children)
            {
                count = device.ZoneCount;
                device.SetMultiZoneColors(colors.Skip(index).Take(count).ToArray(), transitionDuration);
                index += count;
            }
        }

        public override void SetPower(PowerState power)
        {
            Children.ForEach(i => i.SetPower(power));
        }

        #endregion

        #region Event Handlers

        protected virtual void OnDevicePropertiesChanged(object sender, DeviceEventArgs e)
        {
            SortedChildren = new SynchronizedList<IDevice>(Children.OrderBy(i => i.Name));
            OnPropertiesChanged();
        }

        protected virtual void OnDeviceStateChanged(object sender, DeviceEventArgs e)
        {
            OnStateChanged();
        }

        void OnChildCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach(Device device in e.OldItems.SafeEmpty())
            {
                device.PropertiesChanged -= OnDevicePropertiesChanged;
                device.StateChanged -= OnDeviceStateChanged;
            }

            foreach(Device device in e.NewItems.SafeEmpty())
            {
                device.PropertiesChanged += OnDevicePropertiesChanged;
                device.StateChanged += OnDeviceStateChanged;
            }

            SortedChildren = new SynchronizedList<IDevice>(Children.OrderBy(i => i.Name));
            OnPropertiesChanged();
        }

        #endregion
    }
}
