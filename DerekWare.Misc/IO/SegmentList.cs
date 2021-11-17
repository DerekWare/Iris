using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DerekWare.Strings;

namespace DerekWare.IO
{
    public partial class Path
    {
        public class SegmentList : IReadOnlyList<string>
        {
            readonly IReadOnlyList<string> Items;

            internal SegmentList()
            {
                Items = new List<string>();
                Url = string.Empty;
            }

            internal SegmentList(IEnumerable<string> segments, char pathDelimiter)
            {
                Items = ResolvePathSegments(segments).ToList();
                Url = Items.Join(pathDelimiter);
            }

            public int Count => Items.Count;
            public string Url { get; }
            public string this[int index] => Items[index];

            #region Enumerable

            public IEnumerator<string> GetEnumerator()
            {
                return Items.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion
        }
    }
}
