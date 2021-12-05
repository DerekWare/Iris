using System;
using System.IO;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.HomeAutomation.Lifx.Lan.Colors;
using DerekWare.Reflection;

namespace DerekWare.HomeAutomation.Lifx.Lan.Messages
{
    public enum WaveformType
    {
        Saw = 0,
        Sine = 1,
        HalfSine = 2,
        Triangle = 3,
        Pulse = 4
    }

    // Responds with LightState
    class SetWaveformRequest : Request
    {
        public new const ushort MessageType = 119;

        public SetWaveformRequest()
            : base(MessageType)
        {
        }

        public WaveformSettings Settings { get; set; } = new();

        protected override void SerializePayload(BinaryWriter writer)
        {
            short skew = 0;

            if(Settings.Skew > 0.5)
            {
                skew = (short)((Settings.Skew - 0.5) * short.MaxValue);
            }
            else if(Settings.Skew < 0.5)
            {
                skew = (short)((Settings.Skew + 0.5) * short.MinValue);
            }

            writer.Write((byte)0);
            writer.Write((byte)(Settings.Transient ? 1 : 0));
            Settings.Color.SerializeBinary(writer);
            writer.Write((uint)Settings.Period.TotalMilliseconds);
            writer.Write((float)Settings.Cycle.TotalMilliseconds);
            writer.Write(skew);
            writer.Write((byte)Settings.WaveformType);
            writer.Write((byte)(Settings.ApplyHue ? 1 : 0));
            writer.Write((byte)(Settings.ApplySaturation ? 1 : 0));
            writer.Write((byte)(Settings.ApplyBrightness ? 1 : 0));
            writer.Write((byte)(Settings.ApplyKelvin ? 1 : 0));
        }
    }

    public class WaveformSettings : ICloneable<WaveformSettings>
    {
        public bool ApplyBrightness { get; set; } = true;
        public bool ApplyHue { get; set; } = true;
        public bool ApplyKelvin { get; set; } = true;
        public bool ApplySaturation { get; set; } = true;
        public Color Color { get; set; }
        public TimeSpan Cycle { get; set; }
        public TimeSpan Period { get; set; }
        public double Skew { get; set; } = 0.5;
        public bool Transient { get; set; }
        public WaveformType WaveformType { get; set; }

        #region ICloneable

        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion

        #region ICloneable<WaveformSettings>

        public WaveformSettings Clone()
        {
            return Common.Reflection.Clone(this);
        }

        #endregion
    }
}
