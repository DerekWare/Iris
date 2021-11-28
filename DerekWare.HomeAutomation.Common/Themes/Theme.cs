using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using DerekWare.HomeAutomation.Common.Colors;
using Newtonsoft.Json;

namespace DerekWare.HomeAutomation.Common.Themes
{
    public interface IReadOnlyThemeProperties : ICloneable, IName, IDescription, IFamily
    {
    }

    public interface IThemeProperties : IReadOnlyThemeProperties
    {
        public new string Name { get; set; }
    }

    public abstract class Theme : IThemeProperties, ISerializable
    {
        [Description("True if the theme chooses its own colors."), Browsable(false)]
        public abstract bool IsDynamic { get; }

        [Description("True if the theme is intended for multizone lights, such as the LIFX Z strip."), Browsable(false)]
        public abstract bool IsMultiZone { get; }

        public abstract IReadOnlyCollection<Color> GetPalette(IDevice targetDevice);

        #region ICloneable

        public abstract object Clone();

        #endregion

        #region ISerializable

        public abstract void GetObjectData(SerializationInfo info, StreamingContext context);

        #endregion

        protected Theme()
        {
            Name = this.GetName();
        }

        [JsonIgnore]
        public string Description => this.GetDescription();

        [Browsable(false)]
        public virtual string Family => null;

        [Description("True if the effect runs on the device as opposed to running in this application."), Browsable(false)]
        public bool IsFirmware => false;

        [Browsable(false)]
        public virtual string Name { get; set; }

        public void Apply(IDevice device)
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

        #region Equality

        public bool Equals(Theme other)
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

            return Equals((Theme)obj);
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
    }
}
