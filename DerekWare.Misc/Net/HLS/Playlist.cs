using System;
using System.Collections.Generic;
using System.Linq;
using DerekWare.Collections;
using DerekWare.Diagnostics;
using DerekWare.IO;
using DerekWare.Strings;
using Enumerable = DerekWare.Collections.Enumerable;
using StringSplitOptions = DerekWare.Strings.StringSplitOptions;

namespace DerekWare.Net.HLS
{
    public class Playlist : PlaylistEntry
    {
        public Playlist(Path path)
            : this(null, null)
        {
            Path = path;
        }

        internal Playlist(Playlist parent, IEnumerable<KeyValuePair<string, string>> metadata)
            : base(parent, metadata)
        {
            TreeNodeType = TreeNodeTypes.Branch;
        }

        public IEnumerable<Segment> Segments => this.OfType<Segment>();
        public IEnumerable<Playlist> Streams => this.OfType<Playlist>();

        protected void ParseLines(IEnumerable<string> content)
        {
            Debug.Trace(this, content.JoinLines());
            content.ForEach(ParseNextLine);
        }

        protected virtual void ParseNextLine(string content)
        {
            if(content.IsNullOrEmpty())
            {
                return;
            }

            // Check for a tag start
            var tags = SplitTag(content).ToArray();

            if(tags.Length == 0)
            {
                // Not a tag -- assume it's a URI. If the previous entry was a stream or segment, append the path to that.
                var entry = Count > 0 ? this[Count - 1] : null;

                if(!(entry?.Path).IsNullOrEmpty())
                {
                    entry = null;
                }

                switch(entry)
                {
                    case Playlist stream:
                        stream.Path = Path.ToPath(content);
                        break;
                    case Segment segment:
                        segment.Path = Path.ToPath(content);
                        break;
                    case null:
                        Add(entry = new Segment(this) { Path = Path.ToPath(content) });
                        break;
                    default:
                        throw new InvalidOperationException("Unexpected playlist entry");
                }

                entry.Path = Path.Parent.GetAbsolutePath(entry.Path);
            }
            else if(tags.Length == 2)
            {
                // Based on the tag type, this is either playlist metadata or metadata for the next child entry, such as a stream or segment.
                var metadata = SplitTagValues(tags[1]);

                switch(tags[0].ToUpper())
                {
                    case TagNames.Stream:
                        Add(new Playlist(this, metadata));
                        break;

                    case TagNames.Segment:
                        Add(new Segment(this));
                        break;

                    default:
                        foreach(var i in metadata)
                        {
                            Metadata[i.Key] = i.Value;
                        }

                        break;
                }
            }
        }

        protected override void Populate()
        {
            Debug.Trace(this, "Loading");
            Parse(Path.ReadText());
        }

        #region Conversion

        protected void Parse(string content)
        {
            ParseLines(content.SplitLines());
        }

        #endregion

        public static IEnumerable<string> SplitTag(string content)
        {
            return !content.StartsWith("#") ? Enumerable.Empty<string>() : content.Remove(0, 1).Split(new[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);
        }

        public static IEnumerable<KeyValuePair<string, string>> SplitTagValues(string content)
        {
            foreach(var kvp in content.Split(new[] { ',' }, StringSplitOptions.RespectQuotes))
            {
                var split = kvp.Split(new[] { '=' }, 2, StringSplitOptions.None);

                switch(split.Count)
                {
                    case 1:
                        yield return new KeyValuePair<string, string>(string.Empty, split[0].Trim('\"'));
                        break;

                    case 2:
                        yield return new KeyValuePair<string, string>(split[0], split[1].Trim('\"'));
                        break;

                    default:
                        throw new InvalidOperationException();
                }
            }
        }
    }
}
