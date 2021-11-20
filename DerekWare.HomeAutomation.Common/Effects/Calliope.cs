using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DerekWare.HomeAutomation.Common.Colors;

namespace DerekWare.HomeAutomation.Common.Effects
{
    public class Calliope : MultiZoneColorEffectRenderer
    {
        readonly Scenes.Calliope Scene = new();

        [Range(typeof(double), "0", "1")]
        public double Brightness { get => Scene.Brightness; set => Scene.Brightness = value; }

        [Range(typeof(double), "0", "1")]
        public double Kelvin { get => Scene.Kelvin; set => Scene.Kelvin = value; }

        [Range(typeof(double), "0", "1")]
        public double MaxSaturation { get => Scene.MaxSaturation; set => Scene.MaxSaturation = value; }

        [Range(typeof(double), "0", "1")]
        public double MinSaturation { get => Scene.MinSaturation; set => Scene.MinSaturation = value; }

        public override object Clone()
        {
            return MemberwiseClone();
        }

        protected override bool UpdateColors(RenderState renderState, ref Color[] colors)
        {
            if(!renderState.CycleCountChanged)
            {
                return false;
            }

            colors = Scene.GetPalette(Device).ToArray();
            return true;
        }
    }
}
