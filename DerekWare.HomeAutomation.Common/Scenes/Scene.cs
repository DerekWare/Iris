using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common.Colors;

namespace DerekWare.HomeAutomation.Common.Scenes
{
    public interface IReadOnlySceneProperties : IName, IFamily, IEquatable<IReadOnlySceneProperties>
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
        [Description("True if the scene chooses its own colors."), Browsable(false), XmlIgnore]
        public abstract bool IsDynamic { get; }

        [Description("True if the scene is intended for multizone lights, such as the LIFX Z strip."), Browsable(false), XmlIgnore]
        public abstract bool IsMultiZone { get; }

        public abstract IReadOnlyCollection<Color> GetPalette(IDevice targetDevice);

        #region ICloneable

        public abstract object Clone();

        #endregion

        protected Scene()
        {
            Name = GetType().GetTypeName();
        }

        [XmlIgnore]
        public virtual string Family => null;

        [XmlIgnore, Description("True if the effect runs on the device as opposed to running in this application.")]
        public bool IsFirmware => false;

        [Browsable(false), XmlIgnore]
        public string Name { get; protected set; }

        public void Apply(IDevice device, TimeSpan duration)
        {
            // Retrieve the color palette
            var palette = GetPalette(device);

            // Turn on the device and apply the scene
            device.Power = PowerState.On;

            if(palette.Count > 1)
            {
                device.MultiZoneColors = palette;
            }
            else
            {
                device.Color = palette.First();
            }
        }

        public void Apply(IEnumerable<IDevice> devices, TimeSpan duration)
        {
            devices.ForEach(device => Apply(device, duration));
        }

        public void Apply(IReadOnlyCollection<IDevice> devices)
        {
            devices.ForEach(Apply);
        }

        #region Equality

        public bool Equals(IReadOnlySceneProperties other)
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

            if(ReferenceEquals(this, obj))
            {
                return true;
            }

            if(obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((IReadOnlySceneProperties)obj);
        }

        public override int GetHashCode()
        {
            return Name != null ? Name.GetHashCode() : 0;
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

        #region IScene

        public void Apply(IDevice device)
        {
            Apply(device, TimeSpan.Zero);
        }

        #endregion
    }
}
