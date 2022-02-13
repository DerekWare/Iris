using System;
using System.ComponentModel;
using System.Linq;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common.Audio;
using DerekWare.HomeAutomation.Common;
using DerekWare.Reflection;
using NAudio.Dsp;

namespace DerekWare.HomeAutomation.Common.Effects
{
    [Name("Graphic Equalizer"), Description("Hooks your sound device and responds to sounds made by your PC, including music."), Browsable(false)]
    public class GraphicEQ : MultiZoneColorEffectRenderer
    {
        AudioRecorder AudioRecorder;
        BiQuadFilter[] Filters;

        public GraphicEQ()
        {
            RefreshRate = TimeSpan.FromMilliseconds(200);
        }

        [Description("The color of the unused portions of the device."), Browsable(false)]
        public Color BackgroundColor => Common.Colors.Black;

        [Browsable(false)]
        public double Kelvin => 1;

        // Unused
        [Browsable(false)]
        public new TimeSpan Duration { get; set; }

        public double MaxBrightness { get; set; } = 1;
        public double MaxSaturation { get; set; } = 1;
        public double MinBrightness { get; set; } = 0.25;
        public double MinSaturation { get; set; } = 0.25;

        public override object Clone()
        {
            return Reflection.Clone(this);
        }

        protected override void StartEffect()
        {
            AudioRecorder = new AudioRecorder { MaxDuration = RefreshRate.Min(TimeSpan.FromSeconds(0.25)) };

            Filters = new[]
            {
                BiQuadFilter.PeakingEQ(AudioRecorder.Format.SampleRate, 100, 0.8f, 0),
                BiQuadFilter.PeakingEQ(AudioRecorder.Format.SampleRate, 200, 0.8f, 0),
                BiQuadFilter.PeakingEQ(AudioRecorder.Format.SampleRate, 400, 0.8f, 0),
                BiQuadFilter.PeakingEQ(AudioRecorder.Format.SampleRate, 800, 0.8f, 0),
                BiQuadFilter.PeakingEQ(AudioRecorder.Format.SampleRate, 1200, 0.8f, 0),
                BiQuadFilter.PeakingEQ(AudioRecorder.Format.SampleRate, 2400, 0.8f, 0),
                BiQuadFilter.PeakingEQ(AudioRecorder.Format.SampleRate, 4800, 0.8f, 0),
                BiQuadFilter.PeakingEQ(AudioRecorder.Format.SampleRate, 9600, 0.8f, 0)
            };

            AudioRecorder.Start();
            base.StartEffect();
        }

        protected override void StopEffect()
        {
            base.StopEffect();
            DerekWare.Extensions.Dispose(ref AudioRecorder);
        }

        protected override bool UpdateColors(RenderState renderState, ref Color[] colors, ref TimeSpan transitionDuration)
        {
            colors = BackgroundColor.Repeat(ZoneCount).ToArray();

            if(AudioRecorder.CurrentDuration.TotalSeconds < (AudioRecorder.MaxDuration.TotalSeconds / 2))
            {
                return true;
            }

            var sourceSamples = AudioRecorder.GetSamples();

            for(var i = 0; i < Filters.Length; ++i)
            {
                var bandSamples = sourceSamples.Select(s => Filters[i].Transform(s)).ToArray();
                var peak = bandSamples.Max();
                colors[i] = new Color(peak, 1, 1, 1);
            }

            transitionDuration = TimeSpan.Zero;

            return true;
        }
    }
}
