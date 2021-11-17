using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace DerekWare.Collections
{
    public interface IObservableCollection<T> : ICollection<T>, IReadOnlyObservableCollection<T>
    {
    }

    public interface IObservableList<T> : IList<T>, IReadOnlyObservableList<T>
    {
    }

    public interface IReadOnlyObservableCollection<out T> : IReadOnlyCollection<T>, IObservableCollectionNotifier
    {
    }

    public interface IReadOnlyObservableList<out T> : IReadOnlyList<T>, IReadOnlyObservableCollection<T>
    {
    }

    [DebuggerDisplay(nameof(Count) + " = {" + nameof(Count) + "}")]
    public class ObservableList<T> : ObservableCollectionNotifier<T>, IObservableList<T>, IList, IEquatable<IEnumerable<T>>, IEquatable<object>
    {
        protected readonly List<T> Items;

        public ObservableList()
        {
            Items = new List<T>();
        }

        public ObservableList(int capacity)
        {
            Items = new List<T>(capacity);
        }

        public ObservableList(IEnumerable<T> items)
        {
            Items = new List<T>(items.SafeEmpty());
        }

        public virtual int Count => Items.Count;
        public virtual bool IsFixedSize => false;
        public virtual bool IsReadOnly => false;
        public virtual bool IsSynchronized => false;
        public object SyncRoot => Items;
        public virtual int Capacity { get => Items.Capacity; set => Items.Capacity = value; }

        public virtual T this[int index]
        {
            get => Items[index];
            set
            {
                var prev = Items[index];
                Items[index] = value;
                OnReplace(prev, value);
            }
        }

        object IList.this[int index] { get => this[index]; set => this[index] = (T)value; }

        public virtual void Add(params T[] items)
        {
            AddRange(items);
        }

        public virtual void AddRange(IEnumerable<T> items)
        {
            items.ForEach(Add);
        }

        public virtual ReadOnlyCollection<T> AsReadOnly()
        {
            return Items.AsReadOnly();
        }

        public virtual int BinarySearch(int index, int count, T item, IComparer<T> comparer)
        {
            return Items.BinarySearch(index, count, item, comparer);
        }

        public virtual int BinarySearch(T item)
        {
            return Items.BinarySearch(item);
        }

        public virtual int BinarySearch(T item, IComparer<T> comparer)
        {
            return Items.BinarySearch(item, comparer);
        }

        public virtual void CopyTo(T[] array)
        {
            Items.CopyTo(array);
        }

        public virtual void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            Items.CopyTo(index, array, arrayIndex, count);
        }

        public virtual bool Exists(Predicate<T> match)
        {
            return Items.Exists(match);
        }

        public virtual T Find(Predicate<T> match)
        {
            return Items.Find(match);
        }

        public virtual List<T> FindAll(Predicate<T> match)
        {
            return Items.FindAll(match);
        }

        public virtual int FindIndex(Predicate<T> match)
        {
            return Items.FindIndex(match);
        }

        public virtual int FindIndex(int startIndex, Predicate<T> match)
        {
            return Items.FindIndex(startIndex, match);
        }

        public virtual int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            return Items.FindIndex(startIndex, count, match);
        }

        public virtual T FindLast(Predicate<T> match)
        {
            return Items.FindLast(match);
        }

        public virtual int FindLastIndex(Predicate<T> match)
        {
            return Items.FindLastIndex(match);
        }

        public virtual int FindLastIndex(int startIndex, Predicate<T> match)
        {
            return Items.FindLastIndex(startIndex, match);
        }

        public virtual int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            return Items.FindLastIndex(startIndex, count, match);
        }

        public virtual List<T> GetRange(int index, int count)
        {
            return Items.GetRange(index, count);
        }

        public virtual int IndexOf(T item, int index)
        {
            return Items.IndexOf(item, index);
        }

        public virtual int IndexOf(T item, int index, int count)
        {
            return Items.IndexOf(item, index, count);
        }

        public virtual void InsertRange(int index, IEnumerable<T> items)
        {
            items.ForEach(i => Insert(index++, i));
        }

        public virtual int LastIndexOf(T item)
        {
            return Items.LastIndexOf(item);
        }

        public virtual int LastIndexOf(T item, int index)
        {
            return Items.LastIndexOf(item, index);
        }

        public virtual int LastIndexOf(T item, int index, int count)
        {
            return Items.LastIndexOf(item, index, count);
        }

        public virtual int RemoveAll(Predicate<T> match)
        {
            return Items.Where(item => match(item)).ToArray().Count(Remove);
        }

        public virtual int RemoveAll(IEnumerable<T> items)
        {
            return items.Count(Remove);
        }

        public virtual void RemoveRange(int index, int count)
        {
            count.For(i => RemoveAt(index));
        }

        public virtual void Reverse()
        {
            Items.Reverse();

            // TODO notify
        }

        public virtual void Reverse(int index, int count)
        {
            Items.Reverse(index, count);

            // TODO notify
        }

        public virtual void Sort()
        {
            Items.Sort();

            // TODO notify
        }

        public virtual void Sort(IComparer<T> comparer)
        {
            Items.Sort(comparer);

            // TODO notify
        }

        public virtual void Sort(int index, int count, IComparer<T> comparer)
        {
            Items.Sort(index, count, comparer);

            // TODO notify
        }

        public virtual void Sort(Comparison<T> comparison)
        {
            Items.Sort(comparison);

            // TODO notify
        }

        public virtual T[] ToArray()
        {
            return Items.ToArray();
        }

        public override string ToString()
        {
            return Items.ToString();
        }

        public virtual void TrimExcess()
        {
            Items.TrimExcess();
        }

        public virtual bool TrueForAll(Predicate<T> match)
        {
            return Items.TrueForAll(match);
        }

        protected virtual void ClearItems()
        {
            var items = this.ToList();
            Items.Clear();
            OnReset(items);
        }

        protected virtual bool InsertItem(int index, T item)
        {
            Items.Insert(index, item);
            OnAdd(item);
            return true;
        }

        protected virtual bool RemoveItem(int index, T item)
        {
            Items.RemoveAt(index);
            OnRemove(item);
            return true;
        }

        #region Conversion

        public virtual List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
        {
            return Items.ConvertAll(converter);
        }

        #endregion

        #region Equality

        public virtual bool Equals(IEnumerable<T> other)
        {
            return SequenceComparer<T>.Default.Equals(Items, other);
        }

        public override bool Equals(object other)
        {
            return SequenceComparer<T>.Default.Equals(Items, other);
        }

        public override int GetHashCode()
        {
            return SequenceComparer<T>.Default.GetHashCode(Items);
        }

        #endregion

        #region Enumerable

        public virtual IEnumerator<T> GetEnumerator()
        {
            return Items.CreateEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region ICollection

        public virtual void CopyTo(Array array, int arrayIndex)
        {
            Items.ForEach(i => array.SetValue(i, arrayIndex++));
        }

        #endregion

        #region ICollection<T>

        public virtual void Add(T item)
        {
            Insert(Count, item);
        }

        public virtual void Clear()
        {
            ClearItems();
        }

        public virtual bool Contains(T item)
        {
            return Items.Contains(item);
        }

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            Items.CopyTo(array, arrayIndex);
        }

        public virtual bool Remove(T item)
        {
            var index = IndexOf(item);
            return (index >= 0) && RemoveItem(index, item);
        }

        #endregion

        #region IList

        public virtual int Add(object value)
        {
            if(!(value is T t))
            {
                return -1;
            }

            Add(t);
            return Count - 1;
        }

        public virtual bool Contains(object value)
        {
            if(!(value is T t))
            {
                return false;
            }

            return Contains(t);
        }

        public virtual int IndexOf(object value)
        {
            if(!(value is T t))
            {
                return -1;
            }

            return IndexOf(t);
        }

        public virtual void Insert(int index, object value)
        {
            if(!(value is T t))
            {
                return;
            }

            Insert(index, t);
        }

        public virtual void Remove(object value)
        {
            if(!(value is T t))
            {
                return;
            }

            Remove(t);
        }

        #endregion

        #region IList<T>

        public virtual int IndexOf(T item)
        {
            return Items.IndexOf(item);
        }

        public virtual void Insert(int index, T item)
        {
            InsertItem(index, item);
        }

        public virtual void RemoveAt(int index)
        {
            if((index >= 0) && (index < Count))
            {
                RemoveItem(index, this[index]);
            }
        }

        #endregion

        public static ObservableList<T> operator +(ObservableList<T> target, T source)
        {
            target.Add(source);
            return target;
        }

        public static ObservableList<T> operator +(ObservableList<T> target, IEnumerable<T> source)
        {
            target.AddRange(source);
            return target;
        }

        public static implicit operator List<T>(ObservableList<T> obj)
        {
            return new List<T>(obj);
        }

        public static implicit operator ObservableList<T>(List<T> obj)
        {
            return new ObservableList<T>(obj);
        }

        public static ObservableList<T> operator -(ObservableList<T> target, T source)
        {
            target.Remove(source);
            return target;
        }

        public static ObservableList<T> operator -(ObservableList<T> target, IEnumerable<T> source)
        {
            target.RemoveAll(source);
            return target;
        }
    }
}
