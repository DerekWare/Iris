using System.ComponentModel.DataAnnotations;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.HomeAutomation.Common.Effects;
using DerekWare.Reflection;

namespace DerekWare.HomeAutomation.PhilipsHue
{
    [Name("ColorLoop (Hue firmware)")]
    public class ColorLoop : Effect
    {
        public override string Family => Client.Instance.Family;
        public override bool IsFirmware => true;
        public override bool IsMultiZone => false;

        [Range(0.0, 1.0)]
        public double Brightness { get; set; } = 1;

        [Range(0.0, 1.0)]
        public double Saturation { get; set; } = 1;

        public override object Clone()
        {
            return Common.Reflection.Clone(this);
        }

        protected override void StartEffect()
        {
            // Batch up the commands rather than calling individual APIs
            var color = new Color(1, Saturation, Brightness, 1);
            var cmd = color.ToLightCommand();
            cmd.On = true;
            cmd.Effect = Q42.HueApi.Effect.ColorLoop;
            ((IHueDevice)Device).SendCommand(cmd);
        }

        protected override void StopEffect()
        {
            Device.SetFirmwareEffect(Q42.HueApi.Effect.None);
        }
    }
}
