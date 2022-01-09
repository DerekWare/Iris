using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DerekWare.Collections
{
    /// <summary>
    ///     Combines a List with a Dictionary to maintain the insertion order of items.
    /// </summary>
    public class OrderedDictionary<TKey, TValue> : SynchronizedDictionary<TKey, TValue>, IList<KeyValuePair<TKey, TValue>>
    {
        protected readonly List<KeyValuePair<TKey, TValue>> List = new();

        public OrderedDictionary()
        {
        }

        public OrderedDictionary(IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
        }

        public OrderedDictionary(IDictionary<TKey, TValue> other, IEqualityComparer<TKey> comparer = null)
            : base(other, comparer)
        {
        }

        public OrderedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> other, IEqualityComparer<TKey> comparer = null)
        {
            AddRange(other);
        }

        public new IEnumerable<TKey> Keys => List.Select(i => i.Key);
        public new IEnumerable<TValue> Values => List.Select(i => i.Value);
        public KeyValuePair<TKey, TValue> this[int index] { get => List[index]; set => throw new NotSupportedException(); }

        public override void Add(TKey key, TValue value)
        {
            lock(SyncRoot)
            {
                List.Add(key.ToKeyValuePair(value));
                base.Add(key, value);
            }
        }

        public override void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            lock(SyncRoot)
            {
                base.AddRange(items);
            }
        }

        public void Insert(int index, TKey key, TValue value)
        {
            Insert(index, key.ToKeyValuePair(value));
        }

        public override bool Remove(TKey key)
        {
            lock(SyncRoot)
            {
                foreach(var i in List)
                {
                    if(Equals(i.Key, key))
                    {
                        return Remove(i);
                    }
                }
            }

            return false;
        }

        public override bool SetValue(TKey key, TValue value)
        {
            throw new NotSupportedException();
        }

        #region ICollection<KeyValuePair<TKey,TValue>>

        public override void Clear()
        {
            lock(SyncRoot)
            {
                List.Clear();
                base.Clear();
            }
        }

        public override void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            lock(SyncRoot)
            {
                List.CopyTo(array, arrayIndex);
            }
        }

        #endregion

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>>

        public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            lock(SyncRoot)
            {
                return Items.CreateEnumerator();
            }
        }

        #endregion

        #region IList<KeyValuePair<TKey,TValue>>

        /// <inheritdoc />
        public int IndexOf(KeyValuePair<TKey, TValue> item)
        {
            lock(SyncRoot)
            {
                return List.IndexOf(item);
            }
        }

        /// <inheritdoc />
        public virtual void Insert(int index, KeyValuePair<TKey, TValue> item)
        {
            lock(SyncRoot)
            {
                List.Insert(index, item);
                base.Add(item);
            }
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            lock(SyncRoot)
            {
                var item = this[index];
                List.RemoveAt(index);
                base.Remove(item);
            }
        }

        #endregion
    }
}
