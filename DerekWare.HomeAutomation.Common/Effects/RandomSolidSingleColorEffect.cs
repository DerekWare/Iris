using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DerekWare.Reflection;

namespace DerekWare.HomeAutomation.Common.Effects
{
    [Name("Random Solid"), Description("Selects a random color to apply to the device.")]
    public class RandomSolidSingleColorEffect : SingleColorEffectRenderer
    {
        [Range(typeof(double), "0", "1")]
        public double Brightness { get; set; } = 1;

        [Browsable(false)]
        public override TimeSpan Duration { get => RefreshRate; set => RefreshRate = value; }

        [Range(typeof(double), "0", "1")]
        public double Kelvin { get; set; } = 1;

        [Range(typeof(double), "0", "1")]
        public double MaxSaturation { get; set; } = 1;

        [Range(typeof(double), "0", "1")]
        public double MinSaturation { get; set; } = 0.25;

        public override object Clone()
        {
            return Reflection.Clone(this);
        }

        protected override bool GetColor(RenderState state, out Color color)
        {
            color = new Color
            {
                Hue = Random.GetDouble(), Brightness = Brightness, Saturation = Random.GetDouble(MinSaturation, MaxSaturation), Kelvin = Kelvin
            };

            return true;
        }
    }
}
