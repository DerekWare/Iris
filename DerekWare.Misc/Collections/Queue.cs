using System;
using System.Collections;
using System.Collections.Generic;

namespace DerekWare.Collections
{
    public interface IQueue<T> : IReadOnlyQueue<T>
    {
        void Clear();
        T Pop();
        void Push(T item);
        void PushRange(IEnumerable<T> items);
        bool TryPop(out T item);
    }

    public interface IReadOnlyQueue<T> : IReadOnlyCollection<T>, ICollection
    {
        bool Contains(T item);
        void CopyTo(T[] array, int arrayIndex);
        T Peek();
        T[] ToArray();
        bool TryPeek(out T item);
    }

    public class Queue<T> : IQueue<T>
    {
        readonly System.Collections.Generic.Queue<T> Items;

        public Queue()
        {
            Items = new System.Collections.Generic.Queue<T>();
        }

        public Queue(int capacity)
        {
            Items = new System.Collections.Generic.Queue<T>(capacity);
        }

        public Queue(IEnumerable<T> items)
        {
            Items = new System.Collections.Generic.Queue<T>(items);
        }

        public virtual int Count => Items.Count;
        public virtual bool IsSynchronized => false;
        public object SyncRoot => Items;

        #region ICollection

        public virtual void CopyTo(Array array, int index)
        {
            Items.ForEach(i => array.SetValue(i, index++));
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

        #region IQueue<T>

        public virtual void Clear()
        {
            Items.Clear();
        }

        public virtual T Pop()
        {
            return Items.Dequeue();
        }

        public virtual void Push(T item)
        {
            Items.Enqueue(item);
        }

        public virtual void PushRange(IEnumerable<T> items)
        {
            items.ForEach(Push);
        }

        public virtual bool TryPop(out T item)
        {
            if(Count <= 0)
            {
                item = default;
                return false;
            }

            item = Pop();
            return true;
        }

        #endregion

        #region IReadOnlyQueue<T>

        public virtual bool Contains(T item)
        {
            return Items.Contains(item);
        }

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            Items.CopyTo(array, arrayIndex);
        }

        public virtual T Peek()
        {
            return Items.Peek();
        }

        public virtual T[] ToArray()
        {
            return Items.ToArray();
        }

        public virtual bool TryPeek(out T item)
        {
            if(Count <= 0)
            {
                item = default;
                return false;
            }

            item = Peek();
            return true;
        }

        #endregion

        public static Queue<T> operator +(Queue<T> target, T source)
        {
            target.Push(source);
            return target;
        }

        public static Queue<T> operator +(Queue<T> target, IEnumerable<T> source)
        {
            target.PushRange(source);
            return target;
        }

        public static implicit operator System.Collections.Generic.Queue<T>(Queue<T> obj)
        {
            return new System.Collections.Generic.Queue<T>(obj);
        }

        public static implicit operator Queue<T>(System.Collections.Generic.Queue<T> obj)
        {
            return new Queue<T>(obj);
        }
    }
}
