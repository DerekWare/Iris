using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using DerekWare.HomeAutomation.Common.Colors;
using Newtonsoft.Json;

namespace DerekWare.HomeAutomation.Common.Effects
{
    [Description("Repeatedly applies the Calliope effect with new colors."), Serializable, JsonObject]
    public class Calliope : MultiZoneColorEffectRenderer
    {
        readonly Themes.Calliope Theme = new();

        public Calliope()
        {
        }

        public Calliope(SerializationInfo info, StreamingContext context)
        {
            this.Deserialize(info, context);
        }

        [Range(typeof(double), "0", "1")]
        public double Brightness { get => Theme.Brightness; set => Theme.Brightness = value; }

        [Range(typeof(double), "0", "1")]
        public double Kelvin { get => Theme.Kelvin; set => Theme.Kelvin = value; }

        [Range(typeof(double), "0", "1")]
        public double MaxSaturation { get => Theme.MaxSaturation; set => Theme.MaxSaturation = value; }

        [Range(typeof(double), "0", "1")]
        public double MinSaturation { get => Theme.MinSaturation; set => Theme.MinSaturation = value; }

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

            colors = Theme.GetPalette(Device).ToArray();
            return true;
        }

        #region ISerializable

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            this.Serialize(info, context);
        }

        #endregion
    }
}
