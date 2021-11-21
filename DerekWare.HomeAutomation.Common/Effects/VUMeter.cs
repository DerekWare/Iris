// #define UseRms

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
        AudioRecorder Recorder;

        public VUMeter()
        {
            RefreshRate = TimeSpan.FromMilliseconds(200);
        }

        [Description("The color of the unused portions of the device."), Browsable(false), XmlIgnore]
        public Color BackgroundColor => StandardColors.Black;

        [Browsable(false), XmlIgnore]
        public double Kelvin => 1;

        // Unused
        [Browsable(false), XmlIgnore]
        public new TimeSpan Duration { get; set; }

        public double MaxBrightness { get; set; } = 1;
        public double MaxSaturation { get; set; } = 1;
        public double MinBrightness { get; set; } = 0.5;
        public double MinSaturation { get; set; } = 1;

        [Description("Shifts the center of the effect left or right.")]
        public int Offset { get; set; }

#if UseRms
        [Description("Increases or decreases the sensitivity of the audio analyzer. Higher values mean the audio is treated as louder. 1 is no change."),
         Range(typeof(float), "0.1", "10")]
        public float RmsSensitivity { get; set; } = 1;

        [Description("Increases or decreases the sensitivity of the audio analyzer. Higher values mean the audio is treated as louder. 1 is no change."),
         Range(typeof(float), "0.1", "10")]
        public float AmplitudeSensitivity { get; set; } = 1;
#else
        [Description("Increases or decreases the sensitivity of the audio analyzer. Higher values mean the audio is treated as louder. 1 is no change."),
         Range(typeof(float), "0.1", "10")]
        public float Sensitivity { get; set; } = 1;
#endif

        [Description("Set all devices to the same color rather than treating them as a multizone device.")]
        public bool SingleColor { get; set; } = false;

        public override object Clone()
        {
            return MemberwiseClone();
        }

        protected override void StartEffect()
        {
            Recorder = new AudioRecorder { MaxDuration = RefreshRate.Min(TimeSpan.FromSeconds(0.5)) };
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
            if(Recorder.CurrentDuration.TotalSeconds < (Recorder.MaxDuration.TotalSeconds / 2))
            {
                colors = BackgroundColor.Repeat(colors.Length).ToArray();
                return true;
            }

            // I've tested with both peak amplitude and peak RMS and when playing music, peak
            // RMS is pretty consistently ~amplitude/2 regardless of testing various window
            // sizes, so it's not worth the extra math.
            float amp, rms, irms;

#if UseRms
            Recorder.GetPeakRmsAndAmplitude(out rms, out amp);
            amp *= AmplitudeSensitivity;
            rms *= RmsSensitivity;
#else
            rms = amp = Recorder.GetPeakAmplitude() * Sensitivity;
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

            if(SingleColor || (colors.Length < 3))
            {
                colors = new[] { color };
                return true;
            }

            var size = (int)Math.Ceiling(amp * ((colors.Length + 1) / 2));
            var offset = ((colors.Length / 2) + Offset) - size;

            while(offset < 0)
            {
                offset += colors.Length;
            }

            colors = BackgroundColor.Repeat(colors.Length).ToArray();

            for(var i = 0; i < (size * 2); ++i)
            {
                colors[offset++ % colors.Length] = color;
            }

            return true;
        }
    }
}
