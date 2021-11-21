using System;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common.Audio;
using DerekWare.HomeAutomation.Common.Colors;

namespace DerekWare.HomeAutomation.Common.Effects
{
    [Description("The Visualizer effect hooks your sound device and responds to sounds made by your PC, including music.")]
    public class Visualizer : MultiZoneColorEffectRenderer
    {
        AudioLoopbackFifo Recorder;

        public Visualizer()
        {
            RefreshRate = TimeSpan.FromMilliseconds(200);
        }

        // Unused
        [Browsable(false), XmlIgnore]
        public new TimeSpan Duration { get; set; }

        // TODO need to be able to serialize colors
        [Description("The color of the unused portions of the device.")]
        public Color BackgroundColor { get; set; } = StandardColors.Black;

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

            // TODO why do I need to amplify the sample value so much?
            var samples = Recorder.GetSamples().Select(i => Math.Abs(i) * 8).ToArray();
            var avg = samples.Average().Clamp(0, 1);
            var max = samples.Max().Clamp(0, 1);
            var size = (int)(max * (colors.Length / 2));
            var color = new Color(avg, 1, 1, 1);
            var centerIndex = colors.Length / 2;
            var startIndex = centerIndex - size;
            var i = 0;

            for(; i < startIndex; ++i)
            {
                colors[i] = BackgroundColor;
            }

            for(; i < (startIndex + (size * 2)); ++i)
            {
                colors[i] = color;
            }

            for(; i < colors.Length; ++i)
            {
                colors[i] = BackgroundColor;
            }

            return true;
        }
    }
}
