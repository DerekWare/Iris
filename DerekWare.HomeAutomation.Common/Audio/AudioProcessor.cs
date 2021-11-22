using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DerekWare.Collections;
using NAudio.Dsp;

namespace DerekWare.HomeAutomation.Common.Audio
{
    public class AudioProcessor : IDisposable
    {
        float[] Samples;
        AudioRecorder Source;

        public AudioProcessor(AudioRecorder source)
        {
            Source = source;
        }

        public void Capture(float[] samples)
        {
            Samples = samples;
        }

        public void Capture()
        {
            Capture(Source.GetSamples());
        }

        public void Capture(TimeSpan range)
        {
            Capture(Source.GetSamples(range));
        }

        /// <summary>
        ///     Applies a filter to the captured samples.
        /// </summary>
        /// <param name="filter">The filter to apply to the sample buffer.</param>
        /// <param name="sampleFrequency">Filter every Nth sample. A value of 1 will filter all samples.</param>
        public void Filter(BiQuadFilter filter, int sampleFrequency = 1)
        {
            Samples = Samples.TakeNth(sampleFrequency).Select(filter.Transform).ToArray();
        }

        public float GetPeakAmplitude()
        {
            return GetPeakAmplitude(Samples);
        }

        public float GetPeakRms()
        {
            // The voiced speech of a typical adult male will have a fundamental frequency from 85 to
            // 155 Hz, and that of a typical adult female from 165 to 255 Hz. I'll use as a
            // somewhat arbitrary window size of 120 Hz.
            return GetPeakRms(Source.Format.SampleRate / 120);
        }

        public float GetPeakRms(int windowSize)
        {
            return GetPeakRms(Samples, windowSize);
        }

        #region IDisposable

        public void Dispose()
        {
            Interlocked.Exchange(ref Source, null);
        }

        #endregion

        public static float GetPeakAmplitude(IEnumerable<float> samples)
        {
            return samples.Max();
        }

        public static float GetPeakRms(float[] samples, int windowSize)
        {
            windowSize = Math.Min(windowSize, samples.Length / 2);

            var windowCount = samples.Length / windowSize;
            var windowOffset = 0;

            float rms = 0;

            for(var windowIndex = 0; windowIndex < windowCount; ++windowIndex)
            {
                float r = 0;

                for(var i = 0; i < windowSize; ++i)
                {
                    var s = samples[windowOffset + i];
                    r += s * s;
                }

                r = (float)Math.Sqrt(r / windowSize);
                rms = Math.Max(rms, r);

                windowOffset += windowSize;
            }

            return rms;
        }
    }
}
