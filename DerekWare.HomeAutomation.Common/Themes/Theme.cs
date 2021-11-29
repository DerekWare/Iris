using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

    public abstract class Theme : IThemeProperties
    {
        [Description("True if the theme chooses its own colors."), Browsable(false)]
        public abstract bool IsDynamic { get; }

        [Description("True if the theme is intended for multizone lights, such as the LIFX Z strip."), Browsable(false)]
        public abstract bool IsMultiZone { get; }

        public abstract IReadOnlyCollection<Color> GetPalette(IDevice targetDevice);

        #region ICloneable

        public abstract object Clone();

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

        internal void Apply(IDevice device)
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
    }
}
