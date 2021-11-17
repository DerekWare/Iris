using System;
using System.Collections.Generic;

namespace DerekWare.Collections
{
    /// <summary>
    ///     Combines a List with a Dictionary.
    /// </summary>
    public class KeyedList<TKey, TValue> : ObservableList<TValue>, IReadOnlyLookup<TKey, TValue>
    {
        protected readonly Dictionary<TKey, TValue> Dictionary;

        public KeyedList()
        {
        }

        public KeyedList(Func<TValue, TKey> keySelector)
            : this(keySelector, null, null)
        {
        }

        public KeyedList(Func<TValue, TKey> keySelector, IEqualityComparer<TKey> keyComparer)
            : this(keySelector, keyComparer, null)
        {
        }

        public KeyedList(Func<TValue, TKey> keySelector, IEnumerable<TValue> items)
            : this(keySelector, null, items)
        {
        }

        public KeyedList(Func<TValue, TKey> keySelector, IEqualityComparer<TKey> keyComparer, IEnumerable<TValue> items)
        {
            KeySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
            Dictionary = new Dictionary<TKey, TValue>(keyComparer ?? EqualityComparer<TKey>.Default);
            AddRange(items);
        }

        public KeyedList(KeyedList<TKey, TValue> other)
        {
            KeySelector = other.KeySelector;
            Dictionary = new Dictionary<TKey, TValue>(other.Dictionary, other.KeyComparer);
            Items.AddRange(Items);
        }

        public IEqualityComparer<TKey> KeyComparer => Dictionary.Comparer;
        public Dictionary<TKey, TValue>.KeyCollection Keys => Dictionary.Keys;
        public Func<TValue, TKey> KeySelector { get; }
        public TValue this[TKey key] => Dictionary[key];

        public bool Add(TKey key, TValue value)
        {
            if(Dictionary.ContainsKey(key))
            {
                return false;
            }

            Dictionary.Add(key, value);
            Add(value);

            return true;
        }

        public bool ContainsKey(TKey key)
        {
            return Dictionary.ContainsKey(key);
        }

        public bool RemoveKey(TKey key)
        {
            if(!TryGetValue(key, out var value))
            {
                return false;
            }

            Dictionary.Remove(key);
            Remove(value);

            return true;
        }

        protected override void ClearItems()
        {
            Dictionary.Clear();
            base.ClearItems();
        }

        protected override bool InsertItem(int index, TValue item)
        {
            if(!(KeySelector is null))
            {
                Dictionary.Add(KeySelector(item), item);
            }

            return base.InsertItem(index, item);
        }

        protected override bool RemoveItem(int index, TValue item)
        {
            if(!base.RemoveItem(index, item))
            {
                return false;
            }

            if(!(KeySelector is null) && !Dictionary.Remove(KeySelector(item)))
            {
                return false;
            }

            return true;
        }

        #region IReadOnlyLookup<TKey,TValue>

        public bool TryGetValue(TKey key, out TValue item)
        {
            return Dictionary.TryGetValue(key, out item);
        }

        #endregion
    }
}
