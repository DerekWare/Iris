using System;
using System.Collections.Generic;
using System.Linq;
using NAudio.Wave;
using Enumerable = DerekWare.Collections.Enumerable;

namespace DerekWare.HomeAutomation.Common.Audio
{
    public class AudioFrame
    {
        public float Average => Enumerable.SafeEmpty(Samples).Average();
        public float Max => Enumerable.SafeEmpty(Samples).Max();
        public float Min => Enumerable.SafeEmpty(Samples).Min();
        public int SampleCount => Samples?.Length ?? 0;
        public float[] Samples { get; internal set; }
    }

    // Records audio using the WASAPI loopback, storing PCM samples in a buffer of 
    // a finite size, ejecting samples from the FIFO as needed.
    public class AudioRecorder : IDisposable
    {
        readonly Queue<float> Queue = new();

        WasapiLoopbackCapture CaptureInstance = new();

        public AudioRecorder()
        {
            CaptureInstance.DataAvailable += OnDataAvailable;
        }

        public TimeSpan CurrentDuration => TimeSpan.FromSeconds(Queue.Count / (float)Format.SampleRate);
        public WaveFormat Format => CaptureInstance.WaveFormat;
        public bool IsSupportedFormat => null != SampleConverter;
        public TimeSpan MaxDuration { get; set; } = TimeSpan.FromSeconds(1);

        protected Func<byte[], int, IEnumerable<float>> SampleConverter
        {
            get
            {
                if(Format.Encoding == WaveFormatEncoding.Pcm)
                {
                    switch(Format.BitsPerSample)
                    {
                        case 8:
                            return ConvertPcm8;

                        case 16:
                            return ConvertPcm16;

                        case 32:
                            return ConvertPcm32;
                    }
                }
                else if(Format.Encoding == WaveFormatEncoding.IeeeFloat)
                {
                    return ConvertIeeeFloat;
                }

                return null;
            }
        }

        public AudioFrame GetSamples()
        {
            lock(Queue)
            {
                return new AudioFrame { Samples = Queue.ToArray() };
            }
        }

        public void Start()
        {
            CaptureInstance.StartRecording();
        }

        public void Stop()
        {
            CaptureInstance.StopRecording();
        }

        #region IDisposable

        public void Dispose()
        {
            CaptureInstance?.Dispose();
            CaptureInstance = null;
        }

        #endregion

        #region Event Handlers

        protected void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            lock(Queue)
            {
                // Add mono samples to the queue
                var samples = SampleConverter(e.Buffer, e.BytesRecorded).ToArray();

                for(var i = 0; i < samples.Length; i += Format.Channels)
                {
                    float value = 0;

                    for(var j = 0; j < Format.Channels; ++j)
                    {
                        value += samples[i + j];
                    }

                    value /= Format.Channels;
                    Queue.Enqueue(value);
                }

                // Trim the queue
                while(CurrentDuration > MaxDuration)
                {
                    Queue.Dequeue();
                }
            }
        }

        #endregion

        protected static IEnumerable<float> ConvertIeeeFloat(byte[] bytes, int byteCount)
        {
            for(var i = 0; i < byteCount; i += sizeof(float))
            {
                yield return BitConverter.ToSingle(bytes, i);
            }
        }

        protected static IEnumerable<float> ConvertPcm16(byte[] bytes, int byteCount)
        {
            for(var i = 0; i < byteCount; i += sizeof(short))
            {
                float value = BitConverter.ToInt16(bytes, i);
                yield return value / 32768;
            }
        }

        protected static IEnumerable<float> ConvertPcm32(byte[] bytes, int byteCount)
        {
            for(var i = 0; i < byteCount; i += sizeof(int))
            {
                float value = BitConverter.ToInt32(bytes, i);
                yield return value / 4294967296;
            }
        }

        protected static IEnumerable<float> ConvertPcm8(byte[] bytes, int byteCount)
        {
            for(var i = 0; i < byteCount; ++i)
            {
                float value = bytes[i];
                yield return (value - 128) / 128;
            }
        }
    }
}
