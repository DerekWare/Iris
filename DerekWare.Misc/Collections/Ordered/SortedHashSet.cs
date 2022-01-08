using System;
using System.Collections.Generic;
using System.Linq;
using DerekWare.Collections;

namespace DerekWare.Misc
{
    public class SortedHashSet<T> : OrderedHashSet<T>
    {
        protected new readonly Func<T, T, int> Comparer;

        public SortedHashSet(Func<T, T, int> comparer, IEqualityComparer<T> equalityComparer = null)
            : base(equalityComparer)
        {
            Comparer = comparer;
        }

        public SortedHashSet(IComparer<T> comparer, IEqualityComparer<T> equalityComparer = null)
            : this(comparer.Compare, equalityComparer)
        {
        }

        public SortedHashSet(IEnumerable<T> items, Func<T, T, int> comparer, IEqualityComparer<T> equalityComparer = null)
            : base(items, equalityComparer)
        {
            Comparer = comparer;
        }

        public SortedHashSet(IEnumerable<T> items, IComparer<T> comparer, IEqualityComparer<T> equalityComparer = null)
            : this(items, comparer.Compare, equalityComparer)
        {
        }

        public override T this[int index] { get => base[index]; set => throw new NotSupportedException(); }

        public override bool Add(T item)
        {
            lock(SyncRoot)
            {
                return base.Insert(List.FindInsertionPoint(item, Comparer), item);
            }
        }

        public override bool Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        public virtual void Sort()
        {
            lock(SyncRoot)
            {
                var items = List.ToList();
                List.Clear();
                items.ForEach(item => List.Insert(List.FindInsertionPoint(item, Comparer), item));
            }
        }
    }
}
