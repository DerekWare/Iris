using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common.Colors;

namespace DerekWare.HomeAutomation.Common.Themes
{
    public interface IReadOnlyThemeProperties : IDescription, IFamily, IName, IEquatable<IReadOnlyThemeProperties>
    {
        [Description("True if the theme chooses its own colors.")]
        public abstract bool IsDynamic { get; }

        [Description("True if the effect runs on the device as opposed to running in this application.")]
        public bool IsFirmware { get; }

        [Description("True if the theme is intended for multizone lights, such as the LIFX Z strip.")]
        public abstract bool IsMultiZone { get; }
    }

    public interface ITheme : IThemeProperties, ICloneable
    {
        public void Apply(IDevice device);
    }

    public interface IThemeProperties : IReadOnlyThemeProperties
    {
    }

    public abstract class Theme : ITheme
    {
        [Description("True if the theme chooses its own colors."), Browsable(false), XmlIgnore]
        public abstract bool IsDynamic { get; }

        [Description("True if the theme is intended for multizone lights, such as the LIFX Z strip."), Browsable(false), XmlIgnore]
        public abstract bool IsMultiZone { get; }

        public abstract IReadOnlyCollection<Color> GetPalette(IDevice targetDevice);

        #region ICloneable

        public abstract object Clone();

        #endregion

        protected Theme()
        {
            Name = this.GetName();
        }

        [XmlIgnore]
        public string Description => this.GetDescription();

        [Browsable(false), XmlIgnore]
        public virtual string Family => null;

        [Description("True if the effect runs on the device as opposed to running in this application."), Browsable(false), XmlIgnore]
        public bool IsFirmware => false;

        [Browsable(false), XmlIgnore]
        public virtual string Name { get; set; }

        public void Apply(IDevice device, TimeSpan duration)
        {
            // Retrieve the color palette
            var palette = GetPalette(device);

            // Turn on the device and apply the theme
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

        public bool Equals(IReadOnlyThemeProperties other)
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

            return Equals((IReadOnlyThemeProperties)obj);
        }

        public override int GetHashCode()
        {
            return Name != null ? Name.GetHashCode() : 0;
        }

        public static bool operator ==(Theme left, Theme right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Theme left, Theme right)
        {
            return !Equals(left, right);
        }

        #endregion

        #region ITheme

        public void Apply(IDevice device)
        {
            Apply(device, TimeSpan.Zero);
        }

        #endregion
    }
}
