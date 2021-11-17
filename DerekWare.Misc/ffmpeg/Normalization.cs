using System;

namespace DerekWare.ffmpeg
{
    [Serializable]
    public class LoudNormAnalysis
    {
        public double input_i;
        public double input_lra;
        public double input_thresh;
        public double input_tp;
        public string normalization_type;
        public double output_i;
        public double output_lra;
        public double output_thresh;
        public double output_tp;
        public double target_offset;
    }

    public class Normalization : Filter
    {
        public Normalization()
            : base("loudnorm")
        {
        }

#if false
        public void Analyze()
        {
            // Set filter parameters
            Clear();

            this["I"] = TARGET_I;
            this["LRA"] = TARGET_LRA;
            this["tp"] = TARGET_TP;
            this["print_format"] = "json";

            // Run ffmpeg to get existing normalization levels
            // TODO wrapper class for running ffmpeg
            object[] param =
            {
                "-i \"{Source}\"",
                Streams.AudioStream,
                this,
                "-f null",
                "-"
            };

            var result = new Path("ffmpeg.exe").ConsoleExecute(param);

            // Strip everything off but the JSON
            var j = result.Remove(0, result.LastIndexOf('{'));

            // Deserialize
            Analysis = new JavaScriptSerializer().Deserialize<LoudNormAnalysis>(j);
        }

        public void Normalize()
        {
            // Select the video stream
            var video = Streams.VideoStream;

            // Set transcode parameters
            var param = new List<object>
            {
                "-i \"{Source}\"",
                Streams.AudioStream,
                video,
                "-c:a ac3",
                "-ac 6",
                "-ar 48000",
                "-ab 384k",
                "-channel_layout 63",
            };

            // The target is always H.264, so if the source is as well, we'll just copy. If it's not, we'll transcode.
            if(video.IsH264)
            {
                param.Add("-c:v copy");
            }
            else
            {
                param.Add("-c:v libx264");
                param.Add("-preset slow");
                param.Add($"-crf {(VideoQuality > 0 ? VideoQuality : DefaultVideoQuality)}");
            }

            // Select 
        }


        public string NormalizationParams
        {
            get
            {
                var audio = Streams.AudioStream;
                var video = Streams.VideoStream;
                var param = $"-i \"{Source}\" {ffmpeg.StreamList.ToMapping(video)} {ffmpeg.StreamList.ToMapping(audio)}";

                // The target is always H.264, so if the source is as well, we'll just copy. If it's not, we'll transcode.
                if(string.Equals(video["codec_name"], "h264", StringComparison.OrdinalIgnoreCase))
                {
                    param += " -c:v copy";
                }
                else
                {
                    param += $" -c:v libx264 -preset slow -crf {(VideoQuality > 0 ? VideoQuality : DefaultVideoQuality)}";
                }

                param += " -c:a ac3 -ac 6 -ar 48000 -ab 384k -channel_layout 63";
                param += $" -af loudnorm=I={TARGET_I}:LRA={TARGET_LRA}:tp={TARGET_TP}";
                param += $":measured_I={Analysis.input_i}";
                param += $":measured_LRA={Analysis.input_lra}";
                param += $":measured_tp={Analysis.input_tp}";
                param += $":measured_thresh={Analysis.input_thresh}";
                param += $":offset={Analysis.target_offset}";
                param += $" -f matroska \"{Target}\"";

                return param;
            }
        }














        public const int DefaultVideoQuality = 22;

        public const double TARGET_I = -24.0;
        public const double TARGET_LRA = 11.0;
        public const double TARGET_TP = -2.0;

        public const string TestJson =
            "{\"input_i\" : \"-16.42\",\"input_tp\" : \"1.83\",\"input_lra\" : \"21.80\",\"input_thresh\" : \"-28.58\",\"output_i\" : \"-24.69\",\"output_tp\" : \"-2.00\",\"output_lra\" : \"17.60\",\"output_thresh\" : \"-35.79\",\"normalization_type\" : \"dynamic\",\"target_offset\" : \"0.69\"}";

        public FileIOOptions FileIOOptions;

        public int VideoQuality;

        Path _Source;
        StreamList _Streams;
        Path _Target;

        public LoudNormAnalysis Analysis { get; private set; }

        public Path Source
        {
            get => _Source;
            set
            {
                if(value == _Source)
                {
                    return;
                }

                _Source = value;
                _Streams = new StreamList(_Source);
                Analysis = null;
            }
        }

        public StreamList Streams
        {
            get => _Streams;
            set
            {
                if(value == _Streams)
                {
                    return;
                }

                _Streams = value;
                Analysis = null;
            }
        }

        public Path Target
        {
            get => _Target;
            set
            {
                if(value == _Target)
                {
                    return;
                }

                _Target = value.ChangeExtension("mkv");
            }
        }

        public void Normalize()
        {
            if(Source.Equals(Target))
            {
                throw new IOException("Source and target paths match");
            }

            if(Target.FileExists && !FileIOOptions.HasFlag(FileIOOptions.Overwrite))
            {
                throw new IOException("The target path already exists");
            }

            if(FileIOOptions.HasFlag(FileIOOptions.Test))
            {
                return;
            }

            if(Target.FileExists)
            {
                Target.Delete(FileIOOptions);
            }
            else
            {
                Target.Directory.CreateDirectory();
            }

            if(Analysis is null)
            {
                Analyze();
            }

            new Path("ffmpeg.exe").ConsoleExecute(NormalizationParams);
        }
#endif
    }
}
