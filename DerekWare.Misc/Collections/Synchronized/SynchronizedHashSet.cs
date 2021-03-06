using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DerekWare.Collections
{
    [DebuggerDisplay(nameof(Count) + " = {" + nameof(Count) + "}")]
    public class SynchronizedHashSet<T> : ObservableHashSet<T>
    {
        public SynchronizedHashSet()
        {
        }

        public SynchronizedHashSet(IEqualityComparer<T> comparer)
            : base(comparer)
        {
        }

        public SynchronizedHashSet(IEnumerable<T> items, IEqualityComparer<T> comparer = null)
            : base(items, comparer)
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

        public override bool IsSynchronized => true;

        public override bool Add(T item)
        {
            lock(SyncRoot)
            {
                return base.Add(item);
            }
        }

        public override int AddRange(IEnumerable<T> items)
        {
            lock(SyncRoot)
            {
                return base.AddRange(items);
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

        public override void CopyTo(Array array, int arrayIndex)
        {
            lock(SyncRoot)
            {
                base.CopyTo(array, arrayIndex);
            }
        }

        public override void CopyTo(T[] array, int arrayIndex)
        {
            lock(SyncRoot)
            {
                base.CopyTo(array, arrayIndex);
            }
        }

        public override void ExceptWith(IEnumerable<T> other)
        {
            lock(SyncRoot)
            {
                base.ExceptWith(other);
            }
        }

        public override IEnumerator<T> GetEnumerator()
        {
            lock(SyncRoot)
            {
                return this.ToList().GetEnumerator();
            }
        }

        public override void IntersectWith(IEnumerable<T> other)
        {
            lock(SyncRoot)
            {
                base.IntersectWith(other);
            }
        }

        public override bool IsProperSubsetOf(IEnumerable<T> other)
        {
            lock(SyncRoot)
            {
                return base.IsProperSubsetOf(other);
            }
        }

        public override bool IsProperSupersetOf(IEnumerable<T> other)
        {
            lock(SyncRoot)
            {
                return base.IsProperSupersetOf(other);
            }
        }

        public override bool IsSubsetOf(IEnumerable<T> other)
        {
            lock(SyncRoot)
            {
                return base.IsSubsetOf(other);
            }
        }

        public override bool IsSupersetOf(IEnumerable<T> other)
        {
            lock(SyncRoot)
            {
                return base.IsSupersetOf(other);
            }
        }

        public override bool Overlaps(IEnumerable<T> other)
        {
            lock(SyncRoot)
            {
                return base.Overlaps(other);
            }
        }

        public override bool Remove(T item)
        {
            lock(SyncRoot)
            {
                return base.Remove(item);
            }
        }

        public override bool SetEquals(IEnumerable<T> other)
        {
            lock(SyncRoot)
            {
                return base.SetEquals(other);
            }
        }

        public override void SymmetricExceptWith(IEnumerable<T> other)
        {
            lock(SyncRoot)
            {
                base.SymmetricExceptWith(other);
            }
        }

        public override void UnionWith(IEnumerable<T> other)
        {
            lock(SyncRoot)
            {
                base.UnionWith(other);
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
