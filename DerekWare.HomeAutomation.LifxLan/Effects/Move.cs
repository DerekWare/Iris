using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common;
using DerekWare.HomeAutomation.Common.Effects;
using DerekWare.HomeAutomation.Lifx.Lan.Messages;
using DerekWare.Reflection;
using Newtonsoft.Json;
using Device = DerekWare.HomeAutomation.Lifx.Lan.Devices.Device;

namespace DerekWare.HomeAutomation.Lifx.Lan.Effects
{
    [Name("Move (LIFX firmware)"), Serializable, JsonObject]
    public class Move : Effect
    {
        protected MultiZoneEffectSettings Settings = new() { EffectType = MultiZoneEffectType.Move };

        public Move()
        {
            Direction = MultiZoneEffectDirection.Left;
        }

        public Move(SerializationInfo info, StreamingContext context)
        {
            this.Deserialize(info, context);
        }

        public override string Family => Client.Instance.Family;
        public override bool IsFirmware => true;
        public override bool IsMultiZone => true;

        [Description("The direction in which the effect moves.")]
        public MultiZoneEffectDirection Direction { get => Settings.Direction; set => Settings.Direction = value; }

        [Description("The time it takes for the effect to complete a full cycle and start over."), Range(typeof(TimeSpan), "00:00:02", "24:00:00")]
        public TimeSpan Duration { get => Settings.Cycle; set => Settings.Cycle = value; }

        public override object Clone()
        {
            return MemberwiseClone();
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            this.Serialize(info, context);
        }

        protected override void StartEffect()
        {
            Device.GetDevices().ForEach<Device>(i => i.SetMultiZoneEffect(Settings));
        }

        protected override void StopEffect(bool wait)
        {
            Device.GetDevices().ForEach<Device>(i => i.SetMultiZoneEffect(new MultiZoneEffectSettings()));
        }
    }
}
