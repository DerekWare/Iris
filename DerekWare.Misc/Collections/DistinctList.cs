using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DerekWare.Collections
{
    /// <summary>
    ///     Combines the uniqueness of a HashSet with the flexibility of a List. Unlike a HashSet, this class maintains the
    ///     order items were added.
    /// </summary>
    public class DistinctList<T> : ObservableList<T>, ISet<T>
    {
        protected readonly HashSet<T> Set;

        public DistinctList()
            : this(null, null)
        {
        }

        public DistinctList(DistinctList<T> items)
            : this(items, items.Comparer)
        {
        }

        public DistinctList(IEnumerable<T> items)
            : this(items, null)
        {
        }

        public DistinctList(IEnumerable items)
            : this(items, null)
        {
        }

        public DistinctList(IEqualityComparer<T> comparer)
            : this(null, comparer)
        {
        }

        public DistinctList(IEnumerable<T> items, IEqualityComparer<T> comparer)
        {
            Set = new HashSet<T>(comparer ?? EqualityComparer<T>.Default);
            AddRange(items);
        }

        public DistinctList(IEnumerable items, IEqualityComparer<T> comparer)
        {
            Set = new HashSet<T>(comparer ?? EqualityComparer<T>.Default);
            AddRange(items);
        }

        public IEqualityComparer<T> Comparer => Set.Comparer;

        public override T this[int index]
        {
            get => base[index];
            set
            {
                RemoveAt(index);
                Insert(index, value);
            }
        }

        public new int Add(params T[] items)
        {
            return AddRange(items);
        }

        public new int AddRange(IEnumerable<T> items)
        {
            return items.SafeEmpty().Count(Add);
        }

        public int AddRange(IEnumerable items)
        {
            return AddRange(items.OfType<T>());
        }

        public new bool Insert(int index, T item)
        {
            return InsertItem(index, item);
        }

        public new int InsertRange(int index, IEnumerable<T> items)
        {
            return items.SafeEmpty().Count(i => Insert(index++, i));
        }

        protected override void ClearItems()
        {
            Set.Clear();
            base.ClearItems();
        }

        protected override bool InsertItem(int index, T item)
        {
            return Set.Add(item) && base.InsertItem(index, item);
        }

        protected override bool RemoveItem(int index, T item)
        {
            return Set.Remove(item) || base.RemoveItem(index, item);
        }

        #region ISet<T>

        public new bool Add(T item)
        {
            return InsertItem(Count, item);
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            RemoveAll(other);
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            Set.IntersectWith(other);
            RemoveAll(i => !Set.Contains(i));
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return Set.IsProperSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return Set.IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return Set.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return Set.IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return Set.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return Set.SetEquals(other);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            RemoveAll(other.Where(i => !Add(i)).ToList());
        }

        public void UnionWith(IEnumerable<T> other)
        {
            AddRange(other);
        }

        #endregion

        public static implicit operator List<T>(DistinctList<T> obj)
        {
            return new List<T>(obj);
        }

        public static implicit operator HashSet<T>(DistinctList<T> obj)
        {
            return new HashSet<T>(obj, obj.Comparer);
        }
    }
}
