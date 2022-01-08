using System;
using System.Collections;
using System.Collections.Generic;

namespace DerekWare.Collections
{
    /// <summary>
    ///     Combines a List with a HashSet to maintain the insertion order of items.
    /// </summary>
    public class OrderedHashSet<T> : SynchronizedHashSet<T>, IList<T>
    {
        protected readonly List<T> List = new();

        public OrderedHashSet()
        {
        }

        public OrderedHashSet(IEqualityComparer<T> comparer)
            : base(comparer)
        {
        }

        public OrderedHashSet(IEnumerable<T> items, IEqualityComparer<T> comparer = null)
            : base(items, comparer)
        {
        }

        public virtual T this[int index]
        {
            get
            {
                lock(SyncRoot)
                {
                    return List[index];
                }
            }
            set => throw new NotSupportedException();
        }

        public override bool Add(T item)
        {
            lock(SyncRoot)
            {
                if(!base.Add(item))
                {
                    return false;
                }

                List.Add(item);
                return true;
            }
        }

        public virtual bool Insert(int index, T item)
        {
            lock(SyncRoot)
            {
                if(!base.Add(item))
                {
                    return false;
                }

                List.Insert(index, item);
                return true;
            }
        }

        #region ICollection<T>

        public override void Clear()
        {
            lock(SyncRoot)
            {
                List.Clear();
                base.Clear();
            }
        }

        public override void CopyTo(T[] array, int arrayIndex)
        {
            lock(SyncRoot)
            {
                List.CopyTo(array, arrayIndex);
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

                List.Remove(item);
                return true;
            }
        }

        #endregion

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IEnumerable<T>

        public override IEnumerator<T> GetEnumerator()
        {
            lock(SyncRoot)
            {
                return List.CreateEnumerator();
            }
        }

        #endregion

        #region IList<T>

        /// <inheritdoc />
        public int IndexOf(T item)
        {
            lock(SyncRoot)
            {
                return List.IndexOf(item);
            }
        }

        public void RemoveAt(int index)
        {
            lock(SyncRoot)
            {
                Remove(List[index]);
            }
        }

        void IList<T>.Insert(int index, T item)
        {
            Insert(index, item);
        }

        #endregion
    }
}
