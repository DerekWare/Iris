using System;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common;
using DerekWare.HomeAutomation.Common.Effects;
using DerekWare.HomeAutomation.Lifx.Lan.Messages;
using DerekWare.Reflection;
using Device = DerekWare.HomeAutomation.Lifx.Lan.Device;

namespace DerekWare.HomeAutomation.Lifx.Lan.Effects
{
    [Name("Waveform (LIFX firmware)")]
    public class Waveform : Effect
    {
        protected WaveformSettings Settings = new();

        public Waveform()
        {
            WaveformType = WaveformType.Sine;
            Color = Common.Colors.Black;
            Duration = TimeSpan.FromSeconds(30);
            RefreshRate = TimeSpan.FromSeconds(30);
        }

        public override string Family => Client.Instance.Family;
        public override bool IsFirmware => true;
        public override bool IsMultiZone => false;
        public bool ApplyBrightness { get => Settings.ApplyBrightness; set => Settings.ApplyBrightness = value; }
        public bool ApplyHue { get => Settings.ApplyHue; set => Settings.ApplyHue = value; }
        public bool ApplyKelvin { get => Settings.ApplyKelvin; set => Settings.ApplyKelvin = value; }
        public bool ApplySaturation { get => Settings.ApplySaturation; set => Settings.ApplySaturation = value; }
        public double Brightness { get => Settings.Color.Brightness; set => Settings.Color.Brightness = value; }
        public Color Color { get => Settings.Color; set => Settings.Color = value; }
        public TimeSpan Duration { get => Settings.Cycle; set => Settings.Cycle = value; }
        public double Hue { get => Settings.Color.Hue; set => Settings.Color.Hue = value; }
        public double Kelvin { get => Settings.Color.Kelvin; set => Settings.Color.Kelvin = value; }
        public TimeSpan RefreshRate { get => Settings.Period; set => Settings.Period = value; }
        public double Saturation { get => Settings.Color.Saturation; set => Settings.Color.Saturation = value; }
        public double Skew { get => Settings.Skew; set => Settings.Skew = value; }
        public WaveformType WaveformType { get => Settings.WaveformType; set => Settings.WaveformType = value; }

        public override object Clone()
        {
            return Common.Reflection.Clone(this);
        }

        protected override void StartEffect()
        {
            Device.GetDevices().ForEach<Device>(i => i.SetWaveform(Settings));
        }

        protected override void StopEffect()
        {
            Device.GetDevices().ForEach<Device>(i => i.SetMultiZoneEffect(new MultiZoneEffectSettings()));
        }
    }
}
