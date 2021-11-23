// #define UseRms
// #define UseBandPassFilter

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
    [Name("VU Meter"), Description("Hooks your sound device and responds to sounds made by your PC, including music.")]
    public class VUMeter : MultiZoneColorEffectRenderer
    {
        static readonly TimeSpan MaxRecorderBufferDuration = TimeSpan.FromSeconds(0.25);

        AudioProcessor AudioProcessor;
        AudioRecorder AudioRecorder;
#if UseBandPassFilter
        BiQuadFilter Filter;
#endif

        public VUMeter()
        {
            RefreshRate = TimeSpan.FromMilliseconds(200);
        }

        [Description("The color of the unused portions of the device."), Browsable(false), XmlIgnore]
        public Color BackgroundColor => StandardColors.Black;

        [Browsable(false), XmlIgnore]
        public double Kelvin => 1;

#if UseRms || UseBandPassFilter
        [Description("Increases or decreases the sensitivity of the audio analyzer. Higher values mean the audio is treated as louder. 1 is no change."),
         Range(typeof(float), "0.1", "10")]
        public float AmplitudeSensitivity { get; set; } = 1;

        [Description("Increases or decreases the sensitivity of the audio analyzer. Higher values mean the audio is treated as louder. 1 is no change."),
         Range(typeof(float), "0.1", "10")]
        public float RmsSensitivity { get; set; } = 1;
#else
        [Description("Increases or decreases the sensitivity of the audio analyzer. Higher values mean the audio is treated as louder. 1 is no change."),
         Range(typeof(float), "0.1", "10")]
        public float Sensitivity { get; set; } = 1;
#endif

        // Unused
        [Browsable(false), XmlIgnore]
        public new TimeSpan Duration { get; set; }

        public double MaxBrightness { get; set; } = 1;
        public double MaxSaturation { get; set; } = 1;
        public double MinBrightness { get; set; } = 0.25;
        public double MinSaturation { get; set; } = 0.25;

        [Description("Shifts the center of the effect left or right.")]
        public int Offset { get; set; }

        [Description("Set all devices to the same color rather than treating them as a multizone device.")]
        public bool SingleColor { get; set; } = false;

        public override object Clone()
        {
            return MemberwiseClone();
        }

        protected override void StartEffect()
        {
            AudioRecorder = new AudioRecorder { MaxDuration = RefreshRate.Min(MaxRecorderBufferDuration) };
            AudioProcessor = new AudioProcessor(AudioRecorder);
#if UseBandPassFilter
            Filter = BiQuadFilter.PeakingEQ(AudioRecorder.Format.SampleRate, 120, 1, 0);
#endif
            AudioRecorder.Start();
            base.StartEffect();
        }

        protected override void StopEffect(bool wait)
        {
            base.StopEffect(wait);
            Extensions.Dispose(ref AudioProcessor);
            Extensions.Dispose(ref AudioRecorder);
        }

        protected override bool UpdateColors(RenderState renderState, ref Color[] colors)
        {
            if(AudioRecorder.CurrentDuration.TotalSeconds < (AudioRecorder.MaxDuration.TotalSeconds / 2))
            {
                colors = new[] { BackgroundColor };
                return true;
            }

            float amp, rms, irms;

            // After playing around with a bunch of different music, the RMS values are lower than
            // I'd like and the band-pass filter doesn't produce significantly different results than
            // just using peak amplitude, so I'm going the cheap route.
            AudioProcessor.Update();
#if UseRms || UseBandPassFilter
            amp = AudioProcessor.GetPeakAmplitude() * AmplitudeSensitivity;
#if UseBandPassFilter
            AudioProcessor.Filter(Filter);
#endif
            rms = AudioProcessor.GetPeakRms() * RmsSensitivity;
#else
            rms = amp = AudioProcessor.GetPeakAmplitude() * Sensitivity;
#endif
            irms = 1 - rms;

            if(rms < 0.05f)
            {
                colors = new[] { BackgroundColor };
                return true;
            }

            var color = new Color(irms,
                                  (rms * (MaxSaturation - MinSaturation)) + MinSaturation,
                                  (amp * (MaxBrightness - MinBrightness)) + MinBrightness,
                                  Kelvin);

            if(SingleColor || (ZoneCount < 5))
            {
                colors = new[] { color };
                return true;
            }

            var size = (int)Math.Ceiling(amp * (ZoneCount + 1));
            var offset = ((ZoneCount / 2) + Offset) - (size / 2);

            while(offset < 0)
            {
                offset += ZoneCount;
            }

            colors = BackgroundColor.Repeat(ZoneCount).ToArray();

            for(var i = 0; i < size; ++i)
            {
                colors[offset++ % ZoneCount] = color;
            }

            return true;
        }
    }
}
