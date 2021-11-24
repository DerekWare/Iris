using System;
using System.Collections.Generic;
using System.IO;
using DerekWare.Collections;
using DerekWare.Diagnostics;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.HomeAutomation.Lifx.Lan.Colors;

namespace DerekWare.HomeAutomation.Lifx.Lan.Messages
{
    class ExtendedColorZoneSettings
    {
        public MultiZoneApplicationRequest Apply { get; set; } = MultiZoneApplicationRequest.Apply;
        public IReadOnlyCollection<Color> Colors { get; set; }
        public TimeSpan Duration { get; set; }
        public byte ZoneIndex { get; set; }
    }

    class ExtendedMultiZoneColorsResponse : Response
    {
        public new const ushort MessageType = 512;

        public ExtendedMultiZoneColorsResponse()
            : base(MessageType)
        {
        }

        public byte ColorCount { get; private set; }
        public Color[] Colors { get; private set; }
        public byte ZoneCount { get; private set; }
        public byte ZoneIndex { get; private set; }

        #region Conversion

        public override bool Parse()
        {
            Debug.Assert(1 == Messages.Count);

            using var ms = new MemoryStream(Messages[0].Payload);
            using var b = new BinaryReader(ms);

            ZoneCount = b.ReadByte();
            ZoneIndex = b.ReadByte();
            ColorCount = b.ReadByte();
            Colors = new Color[ColorCount];

            for(var i = 0; i < ColorCount; ++i)
            {
                Colors[i] = new Color();
                Colors[i].DeserializeBinary(b);
            }

            return true;
        }

        #endregion
    }

    class GetExtendedMultiZoneColorsRequest : Request
    {
        public new const ushort MessageType = 511;

        public GetExtendedMultiZoneColorsRequest()
            : base(MessageType)
        {
        }

        public byte EndIndex { get; set; } = 255;
        public byte StartIndex { get; set; } = 0;

        protected override void SerializePayload(BinaryWriter writer)
        {
            writer.Write(StartIndex);
            writer.Write(EndIndex);
        }
    }

    class SetExtendedMultiZoneColorsRequest : Request
    {
        public const int MaxColorValueCount = 82; // Limited to 82 colors per request
        public new const ushort MessageType = 510;

        public SetExtendedMultiZoneColorsRequest()
            : base(MessageType)
        {
        }

        public ExtendedColorZoneSettings Settings { get; set; } = new();

        protected override void SerializePayload(BinaryWriter writer)
        {
            if(Settings.Colors.Count > MaxColorValueCount)
            {
                Debug.Error(null, "Too many colors");
            }

            writer.Write((uint)Settings.Duration.TotalMilliseconds);
            writer.Write((byte)Settings.Apply);
            writer.Write((ushort)Settings.ZoneIndex);
            writer.Write((byte)Settings.Colors.Count);
            Settings.Colors.ForEach(i => i.SerializeBinary(writer));
        }
    }
}
