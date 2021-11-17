using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DerekWare.Collections;
using DerekWare.Strings;

namespace DerekWare.ffmpeg
{
    public class FilterList<T> : List<T>
    {
        public FilterList()
        {
        }

        public FilterList(IEnumerable items)
            : base(items.SafeEmpty().Cast<T>())
        {
        }

        public override string ToString()
        {
            return this.Select(i => i.ToString()).Join(" ");
        }
    }
}
