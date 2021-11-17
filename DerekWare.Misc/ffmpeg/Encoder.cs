namespace DerekWare.ffmpeg
{
    public class Encoder : FilterProperties
    {
        public static readonly Encoder AudioCopy = new Encoder("audio", "copy");
        public static readonly Encoder H264 = new Encoder("video", "libx264") { { "preset", "slow" }, { "crf", 23 } };
        public static readonly Encoder VideoCopy = new Encoder("video", "copy");

        public readonly string Name;
        public readonly string Type;

        public Encoder(CodecType type, CodecName name)
            : this(type.ToString(), name.ToString())
        {
        }

        public Encoder(string type, string name)
        {
            Type = type.ToLower();
            Name = name.ToLower();
            KeyPrefix = "-";
            KeyValueSeparator = " ";
            ParameterSeparator = " ";

            Add($"c:{Type[0]}", Name);
        }
    }
}
