using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.HomeAutomation.Common.Themes;
using DerekWare.Reflection;

namespace DerekWare.HomeAutomation.Common.Effects
{
    [Name("Spectrum Shift"),
     Description(
         "Combines the Spectrum theme and the Move effect, but with more mathematically correct color values during motion than the Move effect can provide. This is particularly evident on groups or devices with fewer color zones, or 1 (where the Move effect can't do anything).")]
    public class SpectrumShift : Move
    {
        protected readonly Spectrum Theme = new();

        public SpectrumShift()
        {
            Duration = TimeSpan.FromSeconds(10);
            RefreshRate = TimeSpan.FromSeconds(0.5);
            ClampRefreshRate = false;
        }

        [Range(0.0, 1.0)]
        public double Brightness { get => Theme.Brightness; set => Theme.Brightness = value; }

        [Range(0.0, 1.0)]
        public double Kelvin { get => Theme.Kelvin; set => Theme.Kelvin = value; }

        [Range(-1.0, 1.0), Description("Start the colors from a specific point in the spectrum.")]
        public double Offset { get; set; }

        [Range(0.0, 1.0)]
        public double Saturation { get => Theme.Saturation; set => Theme.Saturation = value; }

        [Description("Set all devices to the same color rather than treating them as a multizone device.")]
        public bool SingleColor { get; set; }

        [Description(
             "The window size is how much of the visible spectrum to show at once. A window size of 1 will show all of it. A window size of 0 will show a single color. A window size of 0.5 will show half of the spectrum at one time."),
         Range(0.0, 1.0)]
        public double Window { get => Theme.Window; set => Theme.Window = value; }

        public override object Clone()
        {
            return Reflection.Clone(this);
        }

        protected override bool UpdateColors(RenderState renderState, ref Color[] colors, ref TimeSpan transitionDuration)
        {
            Theme.Offset = GetColorOffset(renderState) + Offset;
            Theme.Direction = Behavior == EffectBehavior.Random ? Direction.Forward : Direction;
            colors = Theme.GetPalette(SingleColor ? colors.Length : 1);
            return true;
        }
    }
}
