using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DerekWare.Collections
{
    /// <summary>
    ///     SortedCollection provides basic dynamic collection functionality using a predefined order comparison. Note that
    ///     this base class is not Observable nor Synchronized.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerDisplay(nameof(Count) + " = {" + nameof(Count) + "}")]
    public class SortedCollection<T> : ICollection<T>, ICollection, IEquatable<IEnumerable<T>>, IEquatable<object>
    {
        public readonly Func<T, T, int> OrderComparer;

        protected readonly LinkedList<T> Items = new();

        public SortedCollection(Func<T, T, int> orderComparer)
        {
            OrderComparer = orderComparer;
        }

        public SortedCollection(IComparer<T> orderComparer)
        {
            OrderComparer = orderComparer.Compare;
        }

        public SortedCollection(IEnumerable<T> items, Func<T, T, int> orderComparer)
            : this(orderComparer)
        {
            AddRange(items);
        }

        public SortedCollection(IEnumerable<T> items, IComparer<T> orderComparer)
            : this(orderComparer.Compare)
        {
        }

        public virtual int Count => Items.Count;
        public virtual bool IsReadOnly => false;
        public virtual bool IsSynchronized => false;
        public virtual object SyncRoot { get; set; } = new();

        int ICollection.Count => Count;
        int ICollection<T>.Count => Count;

        public virtual void AddRange(IEnumerable<T> items)
        {
            items.SafeEmpty().ForEach(Add);
        }

        public virtual void Sort()
        {
            var items = Items.ToList();
            Clear();
            AddRange(items);
        }

        #region Equality

        public virtual bool Equals(IEnumerable<T> other)
        {
            return SequenceComparer<T>.Default.Equals(Items, other);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IEnumerable<T>);
        }

        public override int GetHashCode()
        {
            return SequenceComparer<T>.Default.GetHashCode(Items);
        }

        #endregion

        #region ICollection

        public virtual void CopyTo(Array array, int index)
        {
            ((ICollection)Items).CopyTo(array, index);
        }

        #endregion

        #region ICollection<T>

        public virtual void Add(T item)
        {
            Items.InsertSorted(item, OrderComparer);
        }

        public virtual void Clear()
        {
            Items.Clear();
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
            return Items.Remove(item);
        }

        #endregion

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IEnumerable<T>

        public virtual IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        #endregion
    }
}
