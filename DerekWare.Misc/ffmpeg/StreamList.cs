using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using DerekWare.Collections;
using DerekWare.IO;

/*
{
"streams": [
{
    "index": 0,
    "codec_name": "mpeg4",
    "codec_long_name": "MPEG-4 part 2",
    "profile": "Advanced Simple Profile",
    "codec_type": "video",
    "codec_time_base": "417083/10000000",
    "codec_tag_string": "XVID",
    "codec_tag": "0x44495658",
    "width": 1280,
    "height": 544,
    "coded_width": 1280,
    "coded_height": 544,
    "has_b_frames": 1,
    "sample_aspect_ratio": "1:1",
    "display_aspect_ratio": "40:17",
    "pix_fmt": "yuv420p",
    "level": 5,
    "chroma_location": "left",
    "refs": 1,
    "quarter_sample": "false",
    "divx_packed": "false",
    "r_frame_rate": "10000000/417083",
    "avg_frame_rate": "10000000/417083",
    "time_base": "417083/10000000",
    "start_pts": 0,
    "start_time": "0.000000",
    "duration_ts": 165211,
    "duration": "6890.669951",
    "bit_rate": "3314590",
    "nb_frames": "165211",
    "disposition": {
        "default": 0,
        "dub": 0,
        "original": 0,
        "comment": 0,
        "lyrics": 0,
        "karaoke": 0,
        "forced": 0,
        "hearing_impaired": 0,
        "visual_impaired": 0,
        "clean_effects": 0,
        "attached_pic": 0,
        "timed_thumbnails": 0
    }
},
{
    "index": 1,
    "codec_name": "ac3",
    "codec_long_name": "ATSC A/52A (AC-3)",
    "codec_type": "audio",
    "codec_time_base": "1/48000",
    "codec_tag_string": "[0] [0][0]",
    "codec_tag": "0x2000",
    "sample_fmt": "fltp",
    "sample_rate": "48000",
    "channels": 6,
    "channel_layout": "5.1(side)",
    "bits_per_sample": 0,
    "dmix_mode": "-1",
    "ltrt_cmixlev": "-1.000000",
    "ltrt_surmixlev": "-1.000000",
    "loro_cmixlev": "-1.000000",
    "loro_surmixlev": "-1.000000",
    "r_frame_rate": "0/0",
    "avg_frame_rate": "0/0",
    "time_base": "1/56000",
    "start_pts": 0,
    "start_time": "0.000000",
    "bit_rate": "448000",
    "nb_frames": "385877517",
    "disposition": {
        "default": 0,
        "dub": 0,
        "original": 0,
        "comment": 0,
        "lyrics": 0,
        "karaoke": 0,
        "forced": 0,
        "hearing_impaired": 0,
        "visual_impaired": 0,
        "clean_effects": 0,
        "attached_pic": 0,
        "timed_thumbnails": 0
    }
}
],
"format": {
"filename": "N:\\Videos\\Backup\\Thor.avi",
"nbthis": 2,
"nb_programs": 0,
"format_name": "avi",
"format_long_name": "AVI (Audio Video Interleaved)",
"start_time": "0.000000",
"duration": "6890.669951",
"size": "3250034688",
"bit_rate": "3773258",
"probe_score": 100,
"tags": {
    "encoder": "VirtualDubMod 1.5.10.2 (build 2542/release)"
}
}
}
 */

namespace DerekWare.ffmpeg
{
    public class StreamList : FilterList<Stream>
    {
        public StreamList()
        {
        }

        public StreamList(IEnumerable<Stream> items)
            : base(items.SafeEmpty())
        {
        }

        public IEnumerable<Stream> Audio =>
            from s in this
            where s.Is(CodecType.Audio)
            orderby s.ChannelCount descending, s.BitRate descending
            select s;

        public IEnumerable<Stream> Video =>
            from s in this
            where s.Is(CodecType.Video)
            orderby s.BitRate descending
            select s;

        public static StreamList FromJson(string json)
        {
            var d = new JavaScriptSerializer().Deserialize<dynamic>(json);
            return new StreamList(((IEnumerable<dynamic>)d["streams"]).Select(i => new Stream(i)));
        }

        public static StreamList Load(Path file)
        {
            object[] param = { "-v quiet", "-print_format json", "-show_format", "-show_streams", file };
            var json = new Path("ffprobe.exe").ConsoleExecute(param);
            return FromJson(json);
        }
    }
}
