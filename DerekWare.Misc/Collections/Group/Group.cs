using System.Collections.Generic;
using System.Linq;
using DerekWare.Strings;

namespace DerekWare.Collections
{
    /// <summary>
    ///     IGroup extends IGrouping and is used by GroupCollection.
    /// </summary>
    public interface IGroup<out TKey, TValue> : IReadOnlyGroup<TKey, TValue>, ICollection<TValue>
    {
        new int Count { get; }
    }

    /// <summary>
    ///     IGroup extends IGrouping and is used by GroupCollection.
    /// </summary>
    public interface IReadOnlyGroup<out TKey, out TValue> : IGrouping<TKey, TValue>, IReadOnlyCollection<TValue>
    {
        new int Count { get; }
    }

    /// <summary>
    ///     IGroup extends IGrouping and is used by GroupCollection.
    /// </summary>
    public class Group<TKey, TValue> : List<TValue>, IGroup<TKey, TValue>
    {
        public Group(TKey key, IEnumerable<TValue> items = null)
        {
            Key = key;
            AddRange(items.SafeEmpty());
        }

        public TKey Key { get; }

        public override string ToString()
        {
            return Key.SafeToString();
        }
    }
}
