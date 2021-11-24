using System;
using System.IO;
using DerekWare.Diagnostics;
using DerekWare.Reflection;
using Enum = System.Enum;

namespace DerekWare.HomeAutomation.Lifx.Lan.Messages
{
    public enum MultiZoneEffectDirection
    {
        Right = 0,
        Left = 1
    }

    public enum MultiZoneEffectType
    {
        Off = 0,
        Move = 1
    }

    class GetMultiZoneEffectRequest : Request
    {
        public new const ushort MessageType = 507;

        public GetMultiZoneEffectRequest()
            : base(MessageType)
        {
        }
    }

    class MultiZoneEffectResponse : Response
    {
        public new const ushort MessageType = 509;

        public MultiZoneEffectSettings Settings { get; } = new();

        #region Conversion

        public override bool Parse()
        {
            Debug.Assert(1 == Messages.Count);

            using var ms = new MemoryStream(Messages[0].Payload);
            using var b = new BinaryReader(ms);

            Settings.InstanceId = b.ReadUInt32();
            int effectType = b.ReadByte();
            b.ReadUInt16(); // Reserved
            Settings.Cycle = TimeSpan.FromMilliseconds(b.ReadUInt32());
            Settings.Duration = TimeSpan.FromMilliseconds(b.ReadUInt64() / 1000 / 1000);
            b.ReadUInt32(); // Reserved
            b.ReadUInt32(); // Reserved
            b.ReadUInt32(); // Parameter 0
            Settings.Direction = (MultiZoneEffectDirection)b.ReadUInt32(); // Parameter 1
            b.ReadUInt32(); // Parameter 2
            b.ReadUInt32(); // Parameter 3
            b.ReadUInt32(); // Parameter 4
            b.ReadUInt32(); // Parameter 5
            b.ReadUInt32(); // Parameter 6
            b.ReadUInt32(); // Parameter 7

            Settings.EffectType = Enum.IsDefined(typeof(MultiZoneEffectType), effectType) ? (MultiZoneEffectType)effectType : MultiZoneEffectType.Off;

            return true;
        }

        #endregion
    }

    public class MultiZoneEffectSettings : ICloneable<MultiZoneEffectSettings>
    {
        public TimeSpan Cycle { get; set; } = TimeSpan.FromSeconds(30);
        public MultiZoneEffectDirection Direction { get; set; } = MultiZoneEffectDirection.Left;
        public TimeSpan Duration { get; set; } = TimeSpan.MaxValue;
        public MultiZoneEffectType EffectType { get; set; }
        public uint InstanceId { get; set; } = (uint)new Random().Next(0x10000);

        #region ICloneable

        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion

        #region ICloneable<MultiZoneEffectSettings>

        public MultiZoneEffectSettings Clone()
        {
            return (MultiZoneEffectSettings)MemberwiseClone();
        }

        #endregion
    }

    class SetMultiZoneEffectRequest : Request
    {
        public new const ushort MessageType = 508;

        // Let's just say anything more than 24 hours is infinity
        public static readonly TimeSpan MaxDuration = TimeSpan.FromDays(1);

        public SetMultiZoneEffectRequest()
            : base(MessageType)
        {
        }

        public MultiZoneEffectSettings Settings { get; set; } = new();

        protected override void SerializePayload(BinaryWriter writer)
        {
            // Note that the max duration that seems to work on the device is *long*.MaxValue, not ulong.MaxValue.
            var duration = Settings.Duration >= MaxDuration ? long.MaxValue : (ulong)Settings.Duration.TotalMilliseconds * 1000 * 1000;

            writer.Write(Settings.InstanceId);
            writer.Write((byte)Settings.EffectType);
            writer.Write((ushort)0); // Reserved
            writer.Write((uint)Settings.Cycle.TotalMilliseconds);
            writer.Write(duration);
            writer.Write((uint)0); // Reserved
            writer.Write((uint)0); // Reserved
            writer.Write((uint)0); // Parameter 0
            writer.Write((uint)Settings.Direction); // Parameter 1
            writer.Write((uint)0); // Parameter 2
            writer.Write((uint)0); // Parameter 3
            writer.Write((uint)0); // Parameter 4
            writer.Write((uint)0); // Parameter 5
            writer.Write((uint)0); // Parameter 6
            writer.Write((uint)0); // Parameter 7
        }
    }
}
