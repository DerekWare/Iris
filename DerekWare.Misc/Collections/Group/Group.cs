using System.Collections.Generic;
using System.Linq;
using DerekWare.Strings;

namespace DerekWare.Collections
{
    /// <summary>
    ///     Extends IGrouping and is used by GroupCollection.
    /// </summary>
    public interface IGroup<out TKey, TValue> : IReadOnlyGroup<TKey, TValue>, ICollection<TValue>
    {
    }

    /// <summary>
    ///     Extends IGrouping and is used by GroupCollection.
    /// </summary>
    public interface IReadOnlyGroup<out TKey, out TValue> : IGrouping<TKey, TValue>, IReadOnlyCollection<TValue>
    {
    }

    /// <summary>
    ///     Default IGroup implementation for GroupCollection.
    /// </summary>
    public class Group<TKey, TValue> : List<TValue>, IGroup<TKey, TValue>
    {
        public Group(TKey key)
        {
            Key = key;
        }

        public Group(TKey key, IEnumerable<TValue> items)
            : base(items.SafeEmpty())
        {
            Key = key;
        }

        public TKey Key { get; }

        public override string ToString()
        {
            return Key.SafeToString();
        }
    }
}
