using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Xml.Serialization;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common.Audio;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.Reflection;

namespace DerekWare.HomeAutomation.Common.Effects
{
    [Description("The VU Meter effect hooks your sound device and responds to sounds made by your PC, including music."), Name("VU Meter")]
    public class VUMeter : MultiZoneColorEffectRenderer
    {
        AudioLoopbackFifo Recorder;

        public VUMeter()
        {
            RefreshRate = TimeSpan.FromMilliseconds(200);
        }

        [Description("The color of the unused portions of the device."), Browsable(false), XmlIgnore]
        public Color BackgroundColor => StandardColors.Black;

        // Unused
        [Browsable(false), XmlIgnore]
        public new TimeSpan Duration { get; set; }

        [Description("Shifts the center of the effect left or right.")]
        public int Offset { get; set; }

        [Description("Increases the sensitivity of the audio analyzer. Higher values mean the audio is treated as louder. 1 is zero amplification."),
         Range(typeof(float), "1", "10"), Browsable(false), XmlIgnore]
        public float Sensitivity { get; set; } = 1;

        [Description("Set all devices to the same color rather than treating them as a multizone device.")]
        public bool SingleColor { get; set; } = false;

        public override object Clone()
        {
            return MemberwiseClone();
        }

        protected override void StartEffect()
        {
            Recorder = new AudioLoopbackFifo { MaxDuration = RefreshRate.Min(TimeSpan.FromSeconds(1)) };
            Recorder.Start();
            base.StartEffect();
        }

        protected override void StopEffect(bool wait)
        {
            base.StopEffect(wait);
            Extensions.Dispose(ref Recorder);
        }

        protected override bool UpdateColors(RenderState renderState, ref Color[] colors)
        {
            if(Recorder.Count <= 0)
            {
                colors = BackgroundColor.Repeat(colors.Length).ToArray();
                return true;
            }

            // TODO too many hard-coded values
            var samples = Recorder.GetSamples().Select(i => Math.Abs(i) * Sensitivity).ToArray();
            var avg = samples.Average().Clamp(0, 1);
            var max = samples.Max().Clamp(0, 1);
            var color = new Color(1 - max, 1, (avg / 2) + 0.5, 1);

            if(SingleColor || (colors.Length < 5))
            {
                colors = color.Repeat(colors.Length).ToArray();
                return true;
            }

            var size = (int)(max * ((colors.Length + 1) / 2));
            var offset = (colors.Length / 2) + Offset;
            var start = offset - size;

            while(start < 0)
            {
                start += colors.Length;
            }

            colors = BackgroundColor.Repeat(colors.Length).ToArray();

            for(var i = 0; i < size * 2; ++i)
            {
                colors[(start + i) % colors.Length] = color;
            }

            return true;
        }
    }
}
