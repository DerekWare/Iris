using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;

namespace DerekWare.Collections
{
    public interface IObservableDictionary<TKey, TValue> : IReadOnlyObservableDictionary<TKey, TValue>, IDictionary<TKey, TValue>
    {
    }

    public interface IReadOnlyObservableDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>, ILookup<TKey, TValue>, IObservableCollectionNotifier
    {
        IEqualityComparer<TKey> Comparer { get; }
    }

    [DebuggerDisplay(nameof(Count) + " = {" + nameof(Count) + "}")]
    public class ObservableDictionary<TKey, TValue> : IObservableDictionary<TKey, TValue>
    {
        protected readonly Dictionary<TKey, TValue> Items;
        protected readonly ObservableDictionaryNotifier<TKey> Notifier = new ObservableDictionaryNotifier<TKey>();

        public virtual event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add => Notifier.CollectionChanged += value;
            remove => Notifier.CollectionChanged -= value;
        }

        public virtual event PropertyChangedEventHandler PropertyChanged
        {
            add => Notifier.PropertyChanged += value;
            remove => Notifier.PropertyChanged -= value;
        }

        public ObservableDictionary()
        {
            Items = new Dictionary<TKey, TValue>();
        }

        public ObservableDictionary(IEqualityComparer<TKey> comparer)
        {
            Items = new Dictionary<TKey, TValue>(comparer ?? EqualityComparer<TKey>.Default);
        }

        public ObservableDictionary(IDictionary<TKey, TValue> other)
        {
            Items = new Dictionary<TKey, TValue>(other);
        }

        public virtual IEqualityComparer<TKey> Comparer => Items.Comparer;
        public virtual int Count => Items.Count;
        public virtual bool IsSynchronized => false;
        public virtual ICollection<TKey> Keys => Items.Keys;
        public virtual ICollection<TValue> Values => Items.Values;
        public virtual bool IsReadOnly { get; protected set; } = false;
        public object SyncRoot { get; set; } = new object();

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        public TValue this[TKey key] { get => GetValue(key); set => SetValue(key, value); }

        public void Add(params KeyValuePair<TKey, TValue>[] items)
        {
            AddRange(items);
        }

        public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            items.ForEach(Add);
        }

        public virtual void CopyTo(Array array, int index)
        {
            Items.ForEach(i => array.SetValue(i, index++));
        }

        public virtual TValue GetValue(TKey key)
        {
            return Items[key];
        }

        public virtual bool SetValue(TKey key, TValue value, bool force = false)
        {
            var exists = false;

            if(!force)
            {
                exists = Items.ContainsKey(key);

                if(exists && Equals(Items[key], value))
                {
                    return false;
                }
            }

            Items[key] = value;

            if(exists)
            {
                Notifier.OnReplace(key, key);
            }
            else
            {
                Notifier.OnAdd(key);
            }

            return true;
        }

        #region ICollection<KeyValuePair<TKey,TValue>>

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public virtual void Clear()
        {
            Items.Clear();
            Notifier.OnReset();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ContainsKey(item.Key);
        }

        public virtual void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            Items.ForEach(i => array[arrayIndex++] = i);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        #endregion

        #region IDictionary<TKey,TValue>

        public virtual void Add(TKey key, TValue value)
        {
            Items.Add(key, value);
            Notifier.OnAdd(key);
        }

        public virtual bool Remove(TKey key)
        {
            if(!Items.Remove(key))
            {
                return false;
            }

            Notifier.OnRemove(key);
            return true;
        }

        #endregion

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>>

        public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        #endregion

        #region IReadOnlyDictionary<TKey,TValue>

        public virtual bool ContainsKey(TKey key)
        {
            return Items.ContainsKey(key);
        }

        public virtual bool TryGetValue(TKey key, out TValue value)
        {
            return Items.TryGetValue(key, out value);
        }

        #endregion

        public static implicit operator Dictionary<TKey, TValue>(ObservableDictionary<TKey, TValue> obj)
        {
            return new Dictionary<TKey, TValue>(obj);
        }

        public static implicit operator ObservableDictionary<TKey, TValue>(Dictionary<TKey, TValue> obj)
        {
            return new ObservableDictionary<TKey, TValue>(obj);
        }
    }
}
