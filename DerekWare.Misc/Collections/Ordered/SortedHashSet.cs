using System;
using System.Collections.Generic;

namespace DerekWare.Collections
{
    /// <summary>
    ///     Combines a SortedCollection with a SynchronizedHashSet to maintain the order of items based on an order comparer.
    /// </summary>
    public class SortedHashSet<T> : SynchronizedHashSet<T>
    {
        protected readonly SortedCollection<T> SortedCollection;

        public SortedHashSet(Func<T, T, int> orderComparer, IEqualityComparer<T> equalityComparer = null)
            : base(equalityComparer)
        {
            SortedCollection = new SortedCollection<T>(orderComparer);
        }

        public SortedHashSet(IComparer<T> orderComparer, IEqualityComparer<T> equalityComparer = null)
            : this(orderComparer.Compare, equalityComparer)
        {
        }

        public SortedHashSet(IEnumerable<T> items, Func<T, T, int> orderComparer, IEqualityComparer<T> equalityComparer = null)
            : this(orderComparer, equalityComparer)
        {
            AddRange(items);
        }

        public SortedHashSet(IEnumerable<T> items, IComparer<T> orderComparer, IEqualityComparer<T> equalityComparer = null)
            : this(items, orderComparer.Compare, equalityComparer)
        {
        }

        public override bool Add(T item)
        {
            lock(SyncRoot)
            {
                if(!base.Add(item))
                {
                    return false;
                }

                SortedCollection.Add(item);
                return true;
            }
        }

        public override void Clear()
        {
            lock(SyncRoot)
            {
                base.Clear();
                SortedCollection.Clear();
            }
        }

        public override void CopyTo(T[] array, int arrayIndex)
        {
            lock(SyncRoot)
            {
                SortedCollection.CopyTo(array, arrayIndex);
            }
        }

        public override IEnumerator<T> GetEnumerator()
        {
            lock(SyncRoot)
            {
                return SortedCollection.CreateEnumerator();
            }
        }

        public override bool Remove(T item)
        {
            lock(SyncRoot)
            {
                if(!base.Remove(item))
                {
                    return false;
                }

                SortedCollection.Remove(item);
                return true;
            }
        }

        public virtual void Sort()
        {
            lock(SyncRoot)
            {
                SortedCollection.Sort();
            }
        }
    }
}
