using System;
using System.Collections.Generic;

namespace DerekWare.ffmpeg
{
    public enum CodecName
    {
        None,
        Copy,
        H264
    }

    public enum CodecType
    {
        None,
        Audio,
        Video,
        Other
    }

    public class Stream : Dictionary<string, dynamic>
    {
        public Stream()
        {
        }

        public Stream(IDictionary<string, dynamic> that)
            : base(that)
        {
        }

        public int BitRate => this["bit_rate"];
        public int ChannelCount => this["channels"];
        public string CodecName => this["codec_name"];
        public string CodecType => this["codec_type"];
        public double Duration => this["duration"];
        public int ProgramIndex => 0; // TODO support multiprogram
        public int StreamIndex => this["index"];

        public bool Is(CodecType v)
        {
            return Is("codec_type", v);
        }

        public bool Is(CodecName v)
        {
            return Is("codec_name", v);
        }

        public override string ToString()
        {
            return $"-map {ProgramIndex}:{StreamIndex}";
        }

        protected bool Is<T>(string t, T v)
        {
            return string.Equals(this[t], v.ToString(), StringComparison.OrdinalIgnoreCase);
        }
    }
}
