using System;
using System.IO;
using System.Linq;
using DerekWare.Diagnostics;
using DerekWare.HomeAutomation.Common;
using DerekWare.HomeAutomation.Lifx.Lan;

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

    class GetMultiZoneColorsRequest : Request
    {
        public new const ushort MessageType = 501;

        public GetMultiZoneColorsRequest()
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

    public class MultiZoneColorSettings
    {
        public MultiZoneApplicationRequest Apply { get; set; } = MultiZoneApplicationRequest.Apply;
        public Color Color { get; set; } = new();
        public TimeSpan Duration { get; set; }
        public byte EndIndex { get; set; }
        public byte StartIndex { get; set; }
    }

    class MultiZoneColorsResponse : Response
    {
        public new const ushort MessageType = 506;

        public MultiZoneColorsResponse()
            : base(MessageType)
        {
        }

        public Color[] Colors { get; private set; }
        public byte ZoneCount { get; private set; }

        #region Conversion

        public override bool Parse()
        {
            // This response is made up of multiple messages, each with exactly 8 colors.
            // The total number of messages expected is therefore ZoneCount / 8. Parse the
            // first message just to get the zone count.
            if(ZoneCount <= 0)
            {
                using var ms = new MemoryStream(Messages[0].Payload);
                using var b = new BinaryReader(ms);
                ZoneCount = b.ReadByte();
            }

            if(Messages.Count < ((ZoneCount + 7) / 8))
            {
                return false;
            }

            // Coalesce the various messages. They may arrive out of order since it's UDP.
            // Allocate the worst-case array size of 256.
            var colors = new Color[256];

            // Parse each message and store it in the array by start index
            foreach(var message in Messages)
            {
                using var ms = new MemoryStream(message.Payload);
                using var b = new BinaryReader(ms);

                var count = b.ReadByte();
                Debug.Assert(ZoneCount == count);

                var index = b.ReadByte();
                Debug.Assert(index < count);

                for(var i = 0; i < 8; ++i)
                {
                    Debug.Assert(colors[index + i] is null);
                    colors[index + i] = new Color();
                    colors[index + i].DeserializeBinary(b);
                }
            }

            for(var i = 0; i < ZoneCount; ++i)
            {
                Debug.Assert(!ReferenceEquals(colors[i], null));
            }

            // Now that we have the full array of colors, add them the final storage location.
            Colors = colors.Take(ZoneCount).ToArray();
            return true;
        }

        #endregion
    }

    class SetMultiZoneColorsRequest : Request
    {
        public new const ushort MessageType = 501;

        public SetMultiZoneColorsRequest()
            : base(MessageType)
        {
        }

        public MultiZoneColorSettings Settings { get; set; } = new();

        protected override void SerializePayload(BinaryWriter writer)
        {
            writer.Write(Settings.StartIndex);
            writer.Write(Settings.EndIndex);
            Settings.Color.SerializeBinary(writer);
            writer.Write((uint)Settings.Duration.TotalMilliseconds);
            writer.Write((byte)Settings.Apply);
        }
    }
}
