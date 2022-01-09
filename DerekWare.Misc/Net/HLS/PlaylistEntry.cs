using System.Collections.Generic;
using DerekWare.Collections;
using DerekWare.IO;
using DerekWare.Strings;

namespace DerekWare.Net.HLS
{
    public abstract class PlaylistEntry : TreeNode<PlaylistEntry, PlaylistEntry>
    {
        protected PlaylistEntry(PlaylistEntry parent, IEnumerable<KeyValuePair<string, string>> metadata)
            : base(parent)
        {
            Metadata.AddRange(metadata);
        }

        public ObservableDictionary<string, string> Metadata { get; } = new();

        public override IEnumerable<KeyValuePair<object, object>> Properties
        {
            get
            {
                yield return new KeyValuePair<object, object>("Name", Path?.FileNameWithoutExtension);

                foreach(var i in Metadata)
                {
                    yield return new KeyValuePair<object, object>(i.Key, i.Value);
                }

                yield return new KeyValuePair<object, object>("URI", Path.SafeToString());
            }
        }

        public Path Path { get; internal set; }
        public string Tag { get; internal set; }

        public override string ToString()
        {
            return Path.IsNullOrEmpty() ? base.ToString() : Path;
        }
    }
}
