using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DerekWare.Collections
{
    public interface IObservableSet<T> : ISet<T>, IReadOnlyCollection<T>, IObservableCollectionNotifier
    {
    }

    /// <summary>
    ///     Observable version of HashSet.
    /// </summary>
    [DebuggerDisplay(nameof(Count) + " = {" + nameof(Count) + "}")]
    public class ObservableHashSet<T> : ObservableCollectionNotifier<T>, IObservableSet<T>, ICollection, IEquatable<IEnumerable<T>>, IEquatable<object>
    {
        protected readonly HashSet<T> Items;

        public ObservableHashSet()
        {
            Items = new HashSet<T>();
        }

        public ObservableHashSet(IEqualityComparer<T> comparer)
        {
            Items = new HashSet<T>(comparer ?? EqualityComparer<T>.Default);
        }

        public ObservableHashSet(IEnumerable<T> items)
        {
            Items = new HashSet<T>(items.SafeEmpty());
        }

        public ObservableHashSet(IEnumerable<T> items, IEqualityComparer<T> comparer)
        {
            Items = new HashSet<T>(items.SafeEmpty(), comparer ?? EqualityComparer<T>.Default);
        }

        public IEqualityComparer<T> Comparer => Items.Comparer;
        public virtual int Count => Items.Count;
        public virtual bool IsReadOnly => false;
        public virtual bool IsSynchronized => false;
        public object SyncRoot { get; set; } = new object();

        public int Add(params T[] items)
        {
            return AddRange(items);
        }

        public int AddRange(IEnumerable<T> items)
        {
            return items.Count(Add);
        }

        #region Equality

        public virtual bool Equals(IEnumerable<T> other)
        {
            return !ReferenceEquals(null, other) && SetEquals(other);
        }

        public override bool Equals(object other)
        {
            return Equals(other as IEnumerable<T>);
        }

        public override int GetHashCode()
        {
            return Items.GetHashCode();
        }

        public static bool operator ==(ObservableHashSet<T> left, object right)
        {
            return SequenceComparer<T>.Default.Equals(left, right);
        }

        public static bool operator ==(object left, ObservableHashSet<T> right)
        {
            return SequenceComparer<T>.Default.Equals(left, right);
        }

        public static bool operator !=(ObservableHashSet<T> left, object right)
        {
            return !SequenceComparer<T>.Default.Equals(left, right);
        }

        public static bool operator !=(object left, ObservableHashSet<T> right)
        {
            return !SequenceComparer<T>.Default.Equals(left, right);
        }

        #endregion

        #region ICollection

        public virtual void CopyTo(Array array, int arrayIndex)
        {
            Items.ForEach(i => array.SetValue(i, arrayIndex++));
        }

        #endregion

        #region ICollection<T>

        public virtual void Clear()
        {
            var items = this.ToList();
            Items.Clear();
            OnReset(items);
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
            if(!Items.Remove(item))
            {
                return false;
            }

            OnRemove(item);
            return true;
        }

        void ICollection<T>.Add(T item)
        {
            Add(item);
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
            return Items.CreateEnumerator();
        }

        #endregion

        #region ISet<T>

        public virtual bool Add(T item)
        {
            if(!Items.Add(item))
            {
                return false;
            }

            OnAdd(item);
            return true;
        }

        public virtual void ExceptWith(IEnumerable<T> other)
        {
            Items.ExceptWith(other);

            // TODO notify
        }

        public virtual void IntersectWith(IEnumerable<T> other)
        {
            Items.IntersectWith(other);

            // TODO notify
        }

        public virtual bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return Items.IsProperSubsetOf(other);
        }

        public virtual bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return Items.IsProperSupersetOf(other);
        }

        public virtual bool IsSubsetOf(IEnumerable<T> other)
        {
            return Items.IsSubsetOf(other);
        }

        public virtual bool IsSupersetOf(IEnumerable<T> other)
        {
            return Items.IsSupersetOf(other);
        }

        public virtual bool Overlaps(IEnumerable<T> other)
        {
            return Items.Overlaps(other);
        }

        public virtual bool SetEquals(IEnumerable<T> other)
        {
            return Items.SetEquals(other);
        }

        public virtual void SymmetricExceptWith(IEnumerable<T> other)
        {
            Items.SymmetricExceptWith(other);

            // TODO notify
        }

        public virtual void UnionWith(IEnumerable<T> other)
        {
            Items.UnionWith(other);

            // TODO notify
        }

        #endregion

        public static implicit operator HashSet<T>(ObservableHashSet<T> obj)
        {
            return new HashSet<T>(obj, obj.Comparer);
        }

        public static implicit operator ObservableHashSet<T>(HashSet<T> obj)
        {
            return new ObservableHashSet<T>(obj, obj.Comparer);
        }
    }
}
