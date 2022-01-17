using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DerekWare.HomeAutomation.Common.Colors;

namespace DerekWare.HomeAutomation.Common.Effects
{
    [Description("Repeatedly applies the Calliope effect with new colors.")]
    public class Calliope : MultiZoneColorEffectRenderer
    {
        readonly Themes.Calliope Theme = new();

        [Range(typeof(double), "0", "1")]
        public double Brightness { get => Theme.Brightness; set => Theme.Brightness = value; }

        [Browsable(false)]
        public override TimeSpan Duration { get => RefreshRate; set => RefreshRate = value; }

        [Range(typeof(double), "0", "1")]
        public double Kelvin { get => Theme.Kelvin; set => Theme.Kelvin = value; }

        [Range(typeof(double), "0", "1")]
        public double MaxSaturation { get => Theme.MaxSaturation; set => Theme.MaxSaturation = value; }

        [Range(typeof(double), "0", "1")]
        public double MinSaturation { get => Theme.MinSaturation; set => Theme.MinSaturation = value; }

        public override object Clone()
        {
            return Reflection.Clone(this);
        }

        protected override bool UpdateColors(RenderState renderState, ref Color[] colors, ref TimeSpan transitionDuration)
        {
            colors = Theme.GetPalette(Device).ToArray();
            return true;
        }
    }
}
