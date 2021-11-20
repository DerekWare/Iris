using System;
using System.ComponentModel.DataAnnotations;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.Reflection;

namespace DerekWare.HomeAutomation.Common.Effects
{
    [Name("Random Solid")]
    public class RandomSolidSingleColorEffect : SingleColorEffectRenderer
    {
        readonly Random Random = new();

        [Range(typeof(double), "0", "1")]
        public double Brightness { get; set; } = 1;

        [Range(typeof(double), "0", "1")]
        public double Kelvin { get; set; } = 1;

        [Range(typeof(double), "0", "1")]
        public double MaxSaturation { get; set; } = 1;

        [Range(typeof(double), "0", "1")]
        public double MinSaturation { get; set; } = 0.25;

        public override object Clone()
        {
            return MemberwiseClone();
        }

        protected override bool GetColor(RenderState state, out Color color)
        {
            if(!state.CycleCountChanged)
            {
                color = null;
                return false;
            }

            color = new Color
            {
                Hue = Random.NextDouble(),
                Brightness = Brightness,
                Saturation = (Random.NextDouble() * (MaxSaturation - MinSaturation)) + MinSaturation,
                Kelvin = Kelvin
            };

            return true;
        }
    }
}
