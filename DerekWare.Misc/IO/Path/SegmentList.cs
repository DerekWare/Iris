using System.Collections;
using System.Collections.Generic;
using DerekWare.Strings;

namespace DerekWare.IO
{
    public partial class Path
    {
        public class SegmentList : IReadOnlyList<string>
        {
            readonly List<string> Items;

            internal SegmentList()
            {
                Items = new List<string>();
                Url = string.Empty;
            }

            internal SegmentList(IEnumerable<string> segments, char pathDelimiter)
            {
                Items = new List<string>(ResolvePathSegments(segments));
                Url = Items.Join(pathDelimiter);
            }

            public int Count => Items.Count;
            public string Url { get; }
            public string this[int index] => Items[index];

            #region IEnumerable

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion

            #region IEnumerable<string>

            public IEnumerator<string> GetEnumerator()
            {
                return Items.GetEnumerator();
            }

            #endregion
        }
    }
}
