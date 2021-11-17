using System.Collections;
using System.Collections.Generic;

namespace DerekWare.Collections
{
    /// <inheritdoc />
    public class ValueMap<T> : ValueMap<T, T>
    {
    }

    /// <summary>
    ///     A ValueMap is a quick way to convert one value to another and works in both directions. Mapped values may or may
    ///     not be the same type.
    /// </summary>
    public class ValueMap<L, R> : IEnumerable<KeyValuePair<L, R>>
    {
        readonly Dictionary<L, R> _LeftToRight;
        readonly Dictionary<R, L> _RightToLeft;

        public ValueMap()
            : this(null, null, null)
        {
        }

        public ValueMap(IEnumerable<KeyValuePair<L, R>> items)
            : this(items, null, null)
        {
        }

        public ValueMap(IEqualityComparer<L> leftComparer, IEqualityComparer<R> rightComparer)
            : this(null, leftComparer, rightComparer)
        {
        }

        public ValueMap(IEnumerable<KeyValuePair<L, R>> items, IEqualityComparer<L> leftComparer, IEqualityComparer<R> rightComparer)
        {
            _LeftToRight = new Dictionary<L, R>(leftComparer ?? EqualityComparer<L>.Default);
            _RightToLeft = new Dictionary<R, L>(rightComparer ?? EqualityComparer<R>.Default);

            AddRange(items);
        }

        public int Count => _LeftToRight.Count;
        public IReadOnlyDictionary<L, R> LeftToRight => _LeftToRight;
        public IReadOnlyDictionary<R, L> RightToLeft => _RightToLeft;

        public void Add(L key, R value)
        {
            _LeftToRight.Add(key, value);
            _RightToLeft.Add(value, key);
        }

        public void Add(KeyValuePair<L, R> item)
        {
            Add(item.Key, item.Value);
        }

        public void AddRange(IEnumerable<KeyValuePair<L, R>> items)
        {
            items.SafeEmpty().ForEach(Add);
        }

        public L GetLeft(R key)
        {
            TryGetLeft(key, out var value);
            return value;
        }

        public R GetRight(L key)
        {
            TryGetRight(key, out var value);
            return value;
        }

        public bool HasLeft(R key)
        {
            return TryGetLeft(key, out var value);
        }

        public bool HasRight(L key)
        {
            return TryGetRight(key, out var value);
        }

        public bool TryGetLeft(R key, out L value)
        {
            return _RightToLeft.TryGetValue(key, out value);
        }

        public bool TryGetRight(L key, out R value)
        {
            return _LeftToRight.TryGetValue(key, out value);
        }

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IEnumerable<KeyValuePair<L,R>>

        public IEnumerator<KeyValuePair<L, R>> GetEnumerator()
        {
            return _LeftToRight.GetEnumerator();
        }

        #endregion
    }
}
