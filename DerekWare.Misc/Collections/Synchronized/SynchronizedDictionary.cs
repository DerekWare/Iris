using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;

namespace DerekWare.Collections
{
    [DebuggerDisplay(nameof(Count) + " = {" + nameof(Count) + "}")]
    public class SynchronizedDictionary<TKey, TValue> : ObservableDictionary<TKey, TValue>
    {
        public override event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                lock(SyncRoot)
                {
                    base.CollectionChanged += value;
                }
            }
            remove
            {
                lock(SyncRoot)
                {
                    base.CollectionChanged -= value;
                }
            }
        }

        public override event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                lock(SyncRoot)
                {
                    base.PropertyChanged += value;
                }
            }
            remove
            {
                lock(SyncRoot)
                {
                    base.PropertyChanged -= value;
                }
            }
        }

        public SynchronizedDictionary()
        {
        }

        public SynchronizedDictionary(IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
        }

        public SynchronizedDictionary(IDictionary<TKey, TValue> other, IEqualityComparer<TKey> comparer = null)
            : base(other, comparer)
        {
        }

        public SynchronizedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> other, IEqualityComparer<TKey> comparer = null)
            : base(other, comparer)
        {
        }

        public override IEqualityComparer<TKey> Comparer
        {
            get
            {
                lock(SyncRoot)
                {
                    return base.Comparer;
                }
            }
        }

        public override int Count
        {
            get
            {
                lock(SyncRoot)
                {
                    return base.Count;
                }
            }
        }

        public override bool IsSynchronized => true;

        public override ICollection<TKey> Keys
        {
            get
            {
                lock(SyncRoot)
                {
                    return base.Keys;
                }
            }
        }

        public override ICollection<TValue> Values
        {
            get
            {
                lock(SyncRoot)
                {
                    return base.Values;
                }
            }
        }

        public override bool IsReadOnly
        {
            get
            {
                lock(SyncRoot)
                {
                    return base.IsReadOnly;
                }
            }
        }

        public override void Add(TKey key, TValue value)
        {
            lock(SyncRoot)
            {
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

        public override void Clear()
        {
            lock(SyncRoot)
            {
                base.Clear();
            }
        }

        public override bool ContainsKey(TKey key)
        {
            lock(SyncRoot)
            {
                return base.ContainsKey(key);
            }
        }

        public override void CopyTo(Array array, int index)
        {
            lock(SyncRoot)
            {
                base.CopyTo(array, index);
            }
        }

        public override void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            lock(SyncRoot)
            {
                base.CopyTo(array, arrayIndex);
            }
        }

        public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            lock(SyncRoot)
            {
                return this.CreateEnumerator();
            }
        }

        public override TValue GetValue(TKey key)
        {
            lock(SyncRoot)
            {
                return base.GetValue(key);
            }
        }

        public override bool Remove(TKey key)
        {
            lock(SyncRoot)
            {
                return base.Remove(key);
            }
        }

        public override bool SetValue(TKey key, TValue value)
        {
            lock(SyncRoot)
            {
                return base.SetValue(key, value);
            }
        }

        public override bool TryGetValue(TKey key, out TValue value)
        {
            lock(SyncRoot)
            {
                return base.TryGetValue(key, out value);
            }
        }
    }
}
