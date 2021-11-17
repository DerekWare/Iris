using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common.Colors;

namespace DerekWare.HomeAutomation.Common.Scenes
{
    public interface IReadOnlySceneProperties : IName, IFamily
    {
        [Description("True if the scene chooses its own colors.")]
        public abstract bool IsDynamic { get; }

        [Description("True if the effect runs on the device as opposed to running in this application.")]
        public bool IsFirmware { get; }

        [Description("True if the scene is intended for multizone lights, such as the LIFX Z strip.")]
        public abstract bool IsMultiZone { get; }
    }

    public interface IScene : ISceneProperties, ICloneable
    {
        public void Apply(IDevice device);
    }

    public interface ISceneProperties : IReadOnlySceneProperties
    {
    }

    public abstract class Scene : IScene
    {
        [Description("True if the scene chooses its own colors."), Browsable(false)]
        public abstract bool IsDynamic { get; }

        [Description("True if the scene is intended for multizone lights, such as the LIFX Z strip."), Browsable(false)]
        public abstract bool IsMultiZone { get; }

        public abstract IEnumerable<Color> GetPalette(IDevice targetDevice);

        #region ICloneable

        public abstract object Clone();

        #endregion

        public virtual string Family => null;

        [Description("True if the effect runs on the device as opposed to running in this application.")]
        public bool IsFirmware => false;

        [Browsable(false)]
        public string Name => GetType().GetTypeName();

        public void Apply(IDevice device, TimeSpan duration)
        {
            // Retrieve the color palette
            var palette = GetPalette(device).ToList();

            // Turn on the device and apply the scene
            device.Power = PowerState.On;
            device.MultiZoneColors = palette;
        }

        public void Apply(IEnumerable<IDevice> devices, TimeSpan duration)
        {
            devices.ForEach(device => Apply(device, duration));
        }

        public void Apply(IReadOnlyCollection<IDevice> devices)
        {
            devices.ForEach(Apply);
        }

        #region IScene

        public void Apply(IDevice device)
        {
            Apply(device, TimeSpan.Zero);
        }

        #endregion
    }
}
