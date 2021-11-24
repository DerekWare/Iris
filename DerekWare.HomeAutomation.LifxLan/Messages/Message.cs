using System;
using System.IO;

namespace DerekWare.HomeAutomation.Lifx.Lan.Messages
{
    /*
        typedef struct {
            // frame
            uint16_t size;
            uint16_t protocol:12;
            uint8_t addressable:1;
            uint8_t tagged:1;
            uint8_t origin:2;
            uint32_t source;

            // frame address
            uint8_t target[8];
            uint8_t reserved[6];
            uint8_t res_required:1;
            uint8_t ack_required:1;
            uint8_t  :6;
            uint8_t sequence;

            // protocol header
            uint64_t :64;
            uint16_t type;
            uint16_t :16;

            // variable length payload follows
        }
        lx_protocol_header_t;
    */

    class Message
    {
        public const int PacketHeaderSize = 36;
        public static readonly uint ExpectedSource = (uint)new Random().Next(2, int.MaxValue);

        static readonly object SequenceLock = new();
        static byte NextSequenceValue;

        // Packet header: https://lan.developer.lifx.com/docs/packet-contents
        public bool AcknowledgementRequired;
        public ushort MessageType;
        public byte[] Payload = new byte[0];
        public bool ResponseRequired;
        public byte Sequence;
        public uint Source = ExpectedSource;
        public byte[] Target = new byte[8];

        // Outgoing message
        protected Message(ushort messageType)
        {
            MessageType = messageType;
            Sequence = GetNextSequenceValue();
        }

        // Incoming message
        protected Message(Stream stream)
        {
            using var b = new BinaryReader(stream);
            var packetSize = b.ReadUInt16();

            if(packetSize < PacketHeaderSize)
            {
                throw new InvalidDataException($"Packet size: {packetSize} is less than the minimum size {PacketHeaderSize}");
            }

            var payloadSize = packetSize - PacketHeaderSize;
            var protocol = b.ReadUInt16();
            Source = b.ReadUInt32();
            Target = b.ReadBytes(8);
            var reserved0 = b.ReadBytes(6);
            var requirements = b.ReadByte();
            ResponseRequired = (requirements & 1) != 0;
            AcknowledgementRequired = (requirements & 2) != 0;
            Sequence = b.ReadByte();
            var reserved1 = b.ReadUInt64();
            MessageType = b.ReadUInt16();
            var reserved2 = b.ReadUInt16();
            Payload = b.ReadBytes(payloadSize);
        }

        public byte[] SerializeBinary()
        {
            using var stream = new MemoryStream();
            SerializeBinary(stream);
            return stream.ToArray();
        }

        public void SerializeBinary(Stream stream)
        {
            using var writer = new BinaryWriter(stream);
            byte requirements = 0;

            if(ResponseRequired)
            {
                requirements |= 1 << 0;
            }

            if(AcknowledgementRequired)
            {
                requirements |= 1 << 1;
            }

            writer.Write((ushort)(PacketHeaderSize + Payload.Length));
            writer.Write((ushort)0x3400);
            writer.Write(Source);
            writer.Write(Target);
            writer.Write(new byte[6]);
            writer.Write(requirements);
            writer.Write(Sequence);
            writer.Write((ulong)0);
            writer.Write(MessageType);
            writer.Write((ushort)0);

            SerializePayload(writer);

            writer.Flush();
        }

        public override string ToString()
        {
            return $"{{ size:{PacketHeaderSize + Payload.Length}" +
                   $", source:{Source}" +
                   $", target:{Target.FormatByteArray(string.Empty)}" +
                   $", res_required:{ResponseRequired}" +
                   $", ack_required:{AcknowledgementRequired}" +
                   $", sequence:{Sequence}" +
                   $", type:{MessageType}" +
                   $", payload:{Payload.FormatByteArray()} }}";
        }

        protected virtual void SerializePayload(BinaryWriter writer)
        {
            writer.Write(Payload);
        }

        public static Message DeserializeBinary(Stream stream)
        {
            return new Message(stream);
        }

        public static Message DeserializeBinary(byte[] data)
        {
            using var stream = new MemoryStream(data);
            return DeserializeBinary(stream);
        }

        static byte GetNextSequenceValue()
        {
            lock(SequenceLock)
            {
                return NextSequenceValue++;
            }
        }
    }
}
