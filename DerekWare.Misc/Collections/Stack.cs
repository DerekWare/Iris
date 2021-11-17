using System;
using System.Collections;
using System.Collections.Generic;

namespace DerekWare.Collections
{
    public class Stack<T> : IQueue<T>
    {
        readonly System.Collections.Generic.Stack<T> Items;

        public Stack()
        {
            Items = new System.Collections.Generic.Stack<T>();
        }

        public Stack(int capacity)
        {
            Items = new System.Collections.Generic.Stack<T>(capacity);
        }

        public Stack(IEnumerable<T> items)
        {
            Items = new System.Collections.Generic.Stack<T>(items);
        }

        public virtual int Count => Items.Count;
        public virtual bool IsSynchronized => false;
        public object SyncRoot => Items;

        #region ICollection

        public void CopyTo(Array array, int index)
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
            return Items.Pop();
        }

        public virtual void Push(T item)
        {
            Items.Push(item);
        }

        public virtual void PushRange(IEnumerable<T> items)
        {
            items.ForEach(item => Items.Push(item));
        }

        public virtual bool TryPop(out T item)
        {
            if(Items.Count <= 0)
            {
                item = default;
                return false;
            }

            item = Items.Pop();
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
            var items = new T[Items.Count];
            Items.CopyTo(items, 0);
            return items;
        }

        public bool TryPeek(out T item)
        {
            if(Items.Count <= 0)
            {
                item = default;
                return false;
            }

            item = Items.Peek();
            return true;
        }

        #endregion

        public static Stack<T> operator +(Stack<T> target, T source)
        {
            target.Push(source);
            return target;
        }

        public static Stack<T> operator +(Stack<T> target, IEnumerable<T> source)
        {
            target.PushRange(source);
            return target;
        }
    }
}
