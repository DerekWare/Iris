using System;
using System.Collections.Generic;
using System.Linq;

namespace DerekWare.HomeAutomation.Common.Audio
{
    public static class AudioProcessor
    {
        public static float GetPeakAmplitude(this IEnumerable<float> samples)
        {
            return samples.Max();
        }

        public static void GetPeakRmsAndAmplitude(this float[] samples, int windowSize, out float rms, out float amp)
        {
            windowSize = Math.Min(windowSize, samples.Length / 2);

            var windowCount = samples.Length / windowSize;
            var windowOffset = 0;

            rms = 0;
            amp = 0;

            for(var windowIndex = 0; windowIndex < windowCount; ++windowIndex)
            {
                float r = 0;

                for(var i = 0; i < windowSize; ++i)
                {
                    var s = samples[windowOffset + i];
                    r += s * s;
                    amp = Math.Max(amp, s);
                }

                r = (float)Math.Sqrt(r / windowSize);
                rms = Math.Max(rms, r);

                windowOffset += windowSize;
            }
        }
    }
}
