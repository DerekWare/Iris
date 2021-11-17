using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace DerekWare.Collections
{
    public class SynchronizedList<T> : ObservableList<T>
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

        public SynchronizedList()
        {
        }

        public SynchronizedList(int capacity)
            : base(capacity)
        {
        }

        public SynchronizedList(IEnumerable<T> items)
            : base(items)
        {
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

        public override bool IsFixedSize
        {
            get
            {
                lock(SyncRoot)
                {
                    return base.IsFixedSize;
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

        public override bool IsSynchronized => true;

        public override int Capacity
        {
            get
            {
                lock(SyncRoot)
                {
                    return base.Capacity;
                }
            }
            set
            {
                lock(SyncRoot)
                {
                    base.Capacity = value;
                }
            }
        }

        public override T this[int index]
        {
            get
            {
                lock(SyncRoot)
                {
                    return Items[index];
                }
            }
            set
            {
                lock(SyncRoot)
                {
                    Items[index] = value;
                }
            }
        }

        public override void Add(params T[] items)
        {
            lock(SyncRoot)
            {
                base.Add(items);
            }
        }

        public override void Add(T item)
        {
            lock(SyncRoot)
            {
                base.Add(item);
            }
        }

        public override int Add(object value)
        {
            lock(SyncRoot)
            {
                return base.Add(value);
            }
        }

        public override void AddRange(IEnumerable<T> items)
        {
            lock(SyncRoot)
            {
                base.AddRange(items);
            }
        }

        public override ReadOnlyCollection<T> AsReadOnly()
        {
            lock(SyncRoot)
            {
                return new ReadOnlyCollection<T>(Items);
            }
        }

        public override int BinarySearch(int index, int count, T item, IComparer<T> comparer)
        {
            lock(SyncRoot)
            {
                return base.BinarySearch(index, count, item, comparer);
            }
        }

        public override int BinarySearch(T item)
        {
            lock(SyncRoot)
            {
                return base.BinarySearch(item);
            }
        }

        public override int BinarySearch(T item, IComparer<T> comparer)
        {
            lock(SyncRoot)
            {
                return base.BinarySearch(item, comparer);
            }
        }

        public override void Clear()
        {
            lock(SyncRoot)
            {
                base.Clear();
            }
        }

        public override bool Contains(T item)
        {
            lock(SyncRoot)
            {
                return base.Contains(item);
            }
        }

        public override bool Contains(object value)
        {
            lock(SyncRoot)
            {
                return base.Contains(value);
            }
        }

        public override List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
        {
            lock(SyncRoot)
            {
                return base.ConvertAll(converter);
            }
        }

        public override void CopyTo(T[] array)
        {
            lock(SyncRoot)
            {
                base.CopyTo(array);
            }
        }

        public override void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            lock(SyncRoot)
            {
                base.CopyTo(index, array, arrayIndex, count);
            }
        }

        public override void CopyTo(Array array, int index)
        {
            lock(SyncRoot)
            {
                base.CopyTo(array, index);
            }
        }

        public override void CopyTo(T[] array, int arrayIndex)
        {
            lock(SyncRoot)
            {
                base.CopyTo(array, arrayIndex);
            }
        }

        public override bool Exists(Predicate<T> match)
        {
            lock(SyncRoot)
            {
                return base.Exists(match);
            }
        }

        public override T Find(Predicate<T> match)
        {
            lock(SyncRoot)
            {
                return base.Find(match);
            }
        }

        public override List<T> FindAll(Predicate<T> match)
        {
            lock(SyncRoot)
            {
                return base.FindAll(match);
            }
        }

        public override int FindIndex(Predicate<T> match)
        {
            lock(SyncRoot)
            {
                return base.FindIndex(match);
            }
        }

        public override int FindIndex(int startIndex, Predicate<T> match)
        {
            lock(SyncRoot)
            {
                return base.FindIndex(startIndex, match);
            }
        }

        public override int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            lock(SyncRoot)
            {
                return base.FindIndex(startIndex, count, match);
            }
        }

        public override T FindLast(Predicate<T> match)
        {
            lock(SyncRoot)
            {
                return base.FindLast(match);
            }
        }

        public override int FindLastIndex(Predicate<T> match)
        {
            lock(SyncRoot)
            {
                return base.FindLastIndex(match);
            }
        }

        public override int FindLastIndex(int startIndex, Predicate<T> match)
        {
            lock(SyncRoot)
            {
                return base.FindLastIndex(startIndex, match);
            }
        }

        public override int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            lock(SyncRoot)
            {
                return base.FindLastIndex(startIndex, count, match);
            }
        }

        public override IEnumerator<T> GetEnumerator()
        {
            lock(SyncRoot)
            {
                return this.CreateEnumerator();
            }
        }

        public override List<T> GetRange(int index, int count)
        {
            lock(SyncRoot)
            {
                return base.GetRange(index, count);
            }
        }

        public override int IndexOf(T item, int index)
        {
            lock(SyncRoot)
            {
                return base.IndexOf(item, index);
            }
        }

        public override int IndexOf(T item, int index, int count)
        {
            lock(SyncRoot)
            {
                return base.IndexOf(item, index, count);
            }
        }

        public override int IndexOf(object value)
        {
            lock(SyncRoot)
            {
                return base.IndexOf(value);
            }
        }

        public override int IndexOf(T item)
        {
            lock(SyncRoot)
            {
                return base.IndexOf(item);
            }
        }

        public override void Insert(int index, object value)
        {
            lock(SyncRoot)
            {
                base.Insert(index, value);
            }
        }

        public override void Insert(int index, T item)
        {
            lock(SyncRoot)
            {
                base.Insert(index, item);
            }
        }

        public override void InsertRange(int index, IEnumerable<T> items)
        {
            lock(SyncRoot)
            {
                base.InsertRange(index, items);
            }
        }

        public override int LastIndexOf(T item)
        {
            lock(SyncRoot)
            {
                return base.LastIndexOf(item);
            }
        }

        public override int LastIndexOf(T item, int index)
        {
            lock(SyncRoot)
            {
                return base.LastIndexOf(item, index);
            }
        }

        public override int LastIndexOf(T item, int index, int count)
        {
            lock(SyncRoot)
            {
                return base.LastIndexOf(item, index, count);
            }
        }

        public override bool Remove(T item)
        {
            lock(SyncRoot)
            {
                return base.Remove(item);
            }
        }

        public override void Remove(object value)
        {
            lock(SyncRoot)
            {
                base.Remove(value);
            }
        }

        public override int RemoveAll(Predicate<T> match)
        {
            lock(SyncRoot)
            {
                return base.RemoveAll(match);
            }
        }

        public override int RemoveAll(IEnumerable<T> items)
        {
            lock(SyncRoot)
            {
                return base.RemoveAll(items);
            }
        }

        public override void RemoveAt(int index)
        {
            lock(SyncRoot)
            {
                base.RemoveAt(index);
            }
        }

        public override void RemoveRange(int index, int count)
        {
            lock(SyncRoot)
            {
                base.RemoveRange(index, count);
            }
        }

        public override void Reverse()
        {
            lock(SyncRoot)
            {
                base.Reverse();
            }
        }

        public override void Reverse(int index, int count)
        {
            lock(SyncRoot)
            {
                base.Reverse(index, count);
            }
        }

        public override void Sort()
        {
            lock(SyncRoot)
            {
                base.Sort();
            }
        }

        public override void Sort(IComparer<T> comparer)
        {
            lock(SyncRoot)
            {
                base.Sort(comparer);
            }
        }

        public override void Sort(int index, int count, IComparer<T> comparer)
        {
            lock(SyncRoot)
            {
                base.Sort(index, count, comparer);
            }
        }

        public override void Sort(Comparison<T> comparison)
        {
            lock(SyncRoot)
            {
                base.Sort(comparison);
            }
        }

        public override T[] ToArray()
        {
            lock(SyncRoot)
            {
                return base.ToArray();
            }
        }

        public override void TrimExcess()
        {
            lock(SyncRoot)
            {
                base.TrimExcess();
            }
        }

        public override bool TrueForAll(Predicate<T> match)
        {
            lock(SyncRoot)
            {
                return base.TrueForAll(match);
            }
        }

        #region Equality

        public override bool Equals(IEnumerable<T> other)
        {
            lock(SyncRoot)
            {
                return base.Equals(other);
            }
        }

        #endregion
    }
}
