using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DerekWare.Collections
{
    public interface IGroupCollection<TKey, TValue> : IReadOnlyGroupCollection<TKey, TValue>
    {
        IReadOnlyGroupCollection<TKey, TValue> AsReadOnly();
    }

    public interface IReadOnlyGroupCollection<TKey, TValue>
        : IReadOnlyCollection<IReadOnlyGroup<TKey, TValue>>, IReadOnlyDictionary<TKey, IReadOnlyGroup<TKey, TValue>>
    {
        IEqualityComparer<TKey> Comparer { get; }
        new int Count { get; }

        new IEnumerator<IReadOnlyGroup<TKey, TValue>> GetEnumerator();
    }

    [DebuggerDisplay(nameof(Count) + " = {" + nameof(Count) + "}")]
    public class GroupCollection<TKey, TValue> : IGroupCollection<TKey, TValue>
    {
        public delegate IGroup<TKey, TValue> GroupAllocatorDelegate(TKey key);

        public static readonly GroupAllocatorDelegate DefaultAllocator = key => new Group<TKey, TValue>(key);

        protected readonly Dictionary<TKey, IGroup<TKey, TValue>> Items;

        public GroupCollection()
        {
            Items = new Dictionary<TKey, IGroup<TKey, TValue>>();
        }

        public GroupCollection(IEqualityComparer<TKey> keyComparer)
        {
            Items = new Dictionary<TKey, IGroup<TKey, TValue>>(keyComparer);
        }

        public GroupCollection(IEnumerable<KeyValuePair<TKey, TValue>> items, IEqualityComparer<TKey> keyComparer = null)
            : this(keyComparer)
        {
            AddRange(items);
        }

        public GroupCollection(IEnumerable<IReadOnlyGroup<TKey, TValue>> items, IEqualityComparer<TKey> keyComparer = null)
            : this(keyComparer)
        {
            items.SafeEmpty().ForEach(i => AddRange(i.Key, i));
        }

        public GroupCollection(IReadOnlyGroupCollection<TKey, TValue> that)
            : this(that, that.Comparer)
        {
            // Dictionary's constructor requires an IDictionary and won't work with an IReadOnlyDictionary
        }

        public GroupCollection(IEnumerable<TValue> items, Func<TValue, TKey> keySelector, IEqualityComparer<TKey> keyComparer = null)
            : this(keyComparer)
        {
            AddRange(items, keySelector);
        }

        public GroupCollection(IEnumerable<TValue> items, Func<TValue, IEnumerable<TKey>> keySelector, IEqualityComparer<TKey> keyComparer = null)
            : this(keyComparer)
        {
            AddRange(items, keySelector);
        }

        public IEqualityComparer<TKey> Comparer => Items.Comparer;
        public int Count => Items.Count;
        public int ItemCount => Items.Sum(i => i.Value.Count);
        public IEnumerable<TKey> Keys => Items.Keys;
        public IEnumerable<IReadOnlyGroup<TKey, TValue>> Values => Items.Values;
        public GroupAllocatorDelegate Allocator { get; set; } = DefaultAllocator;
        public bool IsReadOnly { get; protected set; }
        public IReadOnlyGroup<TKey, TValue> this[TKey key] => Items[key];

        IReadOnlyGroup<TKey, TValue> IReadOnlyDictionary<TKey, IReadOnlyGroup<TKey, TValue>>.this[TKey key] => Items[key];

        /// <summary>
        ///     Adds an item to the collection.
        /// </summary>
        /// <param name="key">Grouping key.</param>
        /// <param name="item">Item to add.</param>
        /// <returns>True if a new grouping was created.</returns>
        public virtual bool Add(TKey key, TValue item)
        {
            if(IsReadOnly)
            {
                throw new AccessViolationException();
            }

            var create = !Items.TryGetValue(key, out var grouping);

            if(create)
            {
                Items[key] = grouping = Allocator(key);
            }

            grouping.Add(item);

            return create;
        }

        /// <summary>
        ///     Adds an item to the collection.
        /// </summary>
        /// <returns>True if a new grouping was created.</returns>
        public bool Add<TKey2, TValue2>(KeyValuePair<TKey2, TValue2> item)
            where TKey2 : TKey where TValue2 : TValue
        {
            return Add(item.Key, item.Value);
        }

        /// <summary>
        ///     Adds an item using multiple keys to the collection.
        /// </summary>
        /// <returns>The count of groupings that were created.</returns>
        public int Add<TKey2>(IEnumerable<TKey2> keys, TValue item)
            where TKey2 : TKey
        {
            return keys.Count(key => Add(key, item));
        }

        /// <summary>
        ///     Adds multiple items to the collection.
        /// </summary>
        /// <returns>True if a new grouping was created.</returns>
        public bool AddRange<TValue2>(TKey key, IEnumerable<TValue2> items)
            where TValue2 : TValue
        {
            return items.Count(item => Add(key, item)) > 0;
        }

        /// <summary>
        ///     Adds multiple items to the collection.
        /// </summary>
        /// <returns>The count of new groupings that were created.</returns>
        public int AddRange<TKey2, TValue2>(IEnumerable<KeyValuePair<TKey2, TValue2>> items)
            where TKey2 : TKey where TValue2 : TValue
        {
            return items.Count(Add);
        }

        /// <summary>
        ///     Adds multiple items to the collection.
        /// </summary>
        /// <returns>The count of new groupings that were created.</returns>
        public int AddRange(IEnumerable<TValue> items, Func<TValue, TKey> keySelector)
        {
            return items.Count(item => Add(keySelector(item), item));
        }

        /// <summary>
        ///     Adds multiple items to the collection.
        /// </summary>
        /// <returns>The count of new groupings that were created.</returns>
        public int AddRange(IEnumerable<TValue> items, Func<TValue, IEnumerable<TKey>> keySelector)
        {
            return items.Sum(item => Add(keySelector(item), item));
        }

        public void Clear()
        {
            if(IsReadOnly)
            {
                throw new AccessViolationException();
            }

            Items.Clear();
        }

        public bool Contains(IGrouping<TKey, TValue> item)
        {
            return TryGetValue(item.Key, out var other) && ReferenceEquals(item, other);
        }

        public bool Remove(TKey key)
        {
            if(IsReadOnly)
            {
                throw new AccessViolationException();
            }

            return Items.Remove(key);
        }

        public bool Remove(IGrouping<TKey, TValue> item)
        {
            if(IsReadOnly)
            {
                throw new AccessViolationException();
            }

            return Contains(item) && Items.Remove(item.Key);
        }

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,IReadOnlyGroup<TKey,TValue>>>

        IEnumerator<KeyValuePair<TKey, IReadOnlyGroup<TKey, TValue>>> IEnumerable<KeyValuePair<TKey, IReadOnlyGroup<TKey, TValue>>>.GetEnumerator()
        {
            return Items.Select(i => new KeyValuePair<TKey, IReadOnlyGroup<TKey, TValue>>(i.Key, i.Value)).GetEnumerator();
        }

        #endregion

        #region IGroupCollection<TKey,TValue>

        public IReadOnlyGroupCollection<TKey, TValue> AsReadOnly()
        {
            return new GroupCollection<TKey, TValue>(this) { IsReadOnly = true };
        }

        #endregion

        #region IReadOnlyDictionary<TKey,IReadOnlyGroup<TKey,TValue>>

        public bool ContainsKey(TKey key)
        {
            return Items.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out IReadOnlyGroup<TKey, TValue> value)
        {
            var f = Items.TryGetValue(key, out var v);
            value = v;
            return f;
        }

        #endregion

        #region IReadOnlyGroupCollection<TKey,TValue>

        public IEnumerator<IReadOnlyGroup<TKey, TValue>> GetEnumerator()
        {
            return Items.Values.GetEnumerator();
        }

        #endregion
    }

    public static partial class Enumerable
    {
        public static IEnumerable<IReadOnlyGroup<TKey, TValue>> GroupBy<TKey, TValue>(
            this IEnumerable<TValue> items,
            Func<TValue, TKey> keySelector,
            IEqualityComparer<TKey> keyComparer = null)
        {
            return GroupCollection.Create(items, keySelector, keyComparer);
        }

        public static IEnumerable<IReadOnlyGroup<TKey, TValue>> GroupByMany<TKey, TValue>(
            this IEnumerable<TValue> items,
            Func<TValue, IEnumerable<TKey>> keySelector,
            IEqualityComparer<TKey> keyComparer = null)
        {
            return GroupCollection.Create(items, keySelector, keyComparer);
        }
    }

    public static class GroupCollection
    {
        public static GroupCollection<TKey, TValue> Create<TKey, TValue>(IReadOnlyGroupCollection<TKey, TValue> that)
        {
            return new GroupCollection<TKey, TValue>(that);
        }

        public static GroupCollection<TKey, TValue> Create<TKey, TValue>(
            IEnumerable<KeyValuePair<TKey, TValue>> items,
            IEqualityComparer<TKey> keyComparer = null)
        {
            return new GroupCollection<TKey, TValue>(items, keyComparer);
        }

        public static GroupCollection<TKey, TValue> Create<TKey, TValue>(
            IEnumerable<IReadOnlyGroup<TKey, TValue>> items,
            IEqualityComparer<TKey> keyComparer = null)
        {
            return new GroupCollection<TKey, TValue>(items, keyComparer);
        }

        public static GroupCollection<TKey, TValue> Create<TKey, TValue>(
            IEnumerable<TValue> items,
            Func<TValue, TKey> keySelector,
            IEqualityComparer<TKey> keyComparer = null)
        {
            return new GroupCollection<TKey, TValue>(items, keySelector, keyComparer);
        }

        public static GroupCollection<TKey, TValue> Create<TKey, TValue>(
            IEnumerable<TValue> items,
            Func<TValue, IEnumerable<TKey>> keySelector,
            IEqualityComparer<TKey> keyComparer = null)
        {
            return new GroupCollection<TKey, TValue>(items, keySelector, keyComparer);
        }
    }
}
