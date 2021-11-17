using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DerekWare.Diagnostics;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.HomeAutomation.Lifx.Lan.Colors;

namespace DerekWare.HomeAutomation.Lifx.Lan.Messages
{
    /*
        Use the apply field to buffer all the changes before applying them with the last change you wish to make. 
        Send a message for each zone with the apply field set to NO_APPLY. This makes the device store the changes 
        in a buffer. Then send the final message with the apply field set to APPLY which will cause the device to 
        apply all the change in the message along with all the changes in the buffer at once.
    */
    public enum MultiZoneApplicationRequest
    {
        NoApply = 0,
        Apply = 1,
        ApplyOnly = 2
    }

    public class ColorZoneSettings
    {
        public MultiZoneApplicationRequest Apply { get; set; } = MultiZoneApplicationRequest.Apply;
        public Color Color { get; set; } = new();
        public TimeSpan Duration { get; set; }
        public byte EndIndex { get; set; }
        public byte StartIndex { get; set; }
    }

    class GetColorZonesRequest : Request
    {
        public new const ushort MessageType = 501;

        public GetColorZonesRequest()
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

    class SetColorZonesRequest : Request
    {
        public new const ushort MessageType = 501;

        public SetColorZonesRequest()
            : base(MessageType)
        {
        }

        public ColorZoneSettings Settings { get; set; } = new();

        protected override void SerializePayload(BinaryWriter writer)
        {
            writer.Write(Settings.StartIndex);
            writer.Write(Settings.EndIndex);
            Settings.Color.SerializeBinary(writer);
            writer.Write((uint)Settings.Duration.TotalMilliseconds);
            writer.Write((byte)Settings.Apply);
        }
    }

    class StateMultiZone : Response
    {
        public const ushort MessageType = 506;

        public Color[] Colors { get; private set; }
        public byte ZoneCount { get; private set; }

        #region Conversion

        protected override void Parse(List<Message> messages)
        {
            // Coalesce the various messages. They may arrive out of order since it's UDP.
            // Allocate the worst-case array size of 256.
            var colors = new Color[256];
            ZoneCount = 0;

            // Parse each message and store it in the array by start index. Each message
            // will contain exactly 8 colors. If the zone isn't valid, we'll ignore those
            // later.
            foreach(var message in messages)
            {
                using var ms = new MemoryStream(message.Payload);
                using var b = new BinaryReader(ms);

                var count = b.ReadByte();
                var index = b.ReadByte();

                Debug.Assert((ZoneCount == 0) || (ZoneCount == count));
                ZoneCount = Math.Max(ZoneCount, count);

                for(var i = 0; i < 8; ++i)
                {
                    Debug.Assert(colors[index + i] is null);
                    colors[index + i] = new Color();
                    colors[index + i].DeserializeBinary(b);
                }
            }

            // Now that we have the full array of colors, add them the final storage location.
#if false
            for(var i = 0; i < ZoneCount; ++i)
            {
                Debug.Assert(colors[i] is not null);
            }
#endif

            Colors = colors.Take(ZoneCount).ToArray();
        }

        #endregion
    }
}
