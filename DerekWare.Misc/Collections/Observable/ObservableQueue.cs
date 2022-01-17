using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;

namespace DerekWare.Collections
{
    public interface IObservableQueue<T> : IQueue<T>, IObservableCollectionNotifier
    {
    }

    public class ObservableQueue<T> : ObservableCollectionNotifier<T>, IObservableQueue<T>
    {
        protected readonly Queue<T> Items;
        protected readonly ManualResetEventSlim ItemsAvailableEvent = new(false);

        public ObservableQueue()
        {
            Items = new Queue<T>();
        }

        public ObservableQueue(int capacity)
        {
            Items = new Queue<T>(capacity);
        }

        public ObservableQueue(IEnumerable<T> items)
        {
            Items = new Queue<T>(items);
        }

        public virtual int Count => Items.Count;
        public virtual bool IsSynchronized => false;
        public WaitHandle ItemsAvailable => ItemsAvailableEvent.WaitHandle;
        public object SyncRoot { get; set; } = new();

        public virtual int CopyTo(T[] array, int arrayIndex, int count)
        {
            count = Math.Min(count, Count);
            using var e = Items.GetEnumerator();

            for(var i = 0; i < count; ++i)
            {
                e.MoveNext();
                array[i] = e.Current;
            }

            return count;
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if(Count <= 0)
            {
                ItemsAvailableEvent.Reset();
            }
            else
            {
                ItemsAvailableEvent.Set();
            }

            base.OnCollectionChanged(e);
        }

        #region ICollection

        public virtual void CopyTo(Array array, int index)
        {
            Items.CopyTo(array, index);
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
            var items = this.ToList();
            Items.Clear();
            OnReset(items);
        }

        public virtual T Pop()
        {
            var item = Items.Pop();
            OnRemove(item);
            return item;
        }

        public virtual void Push(T item)
        {
            Items.Push(item);
            OnAdd(item);
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

        public static ObservableQueue<T> operator +(ObservableQueue<T> target, T source)
        {
            target.Push(source);
            return target;
        }

        public static ObservableQueue<T> operator +(ObservableQueue<T> target, IEnumerable<T> source)
        {
            target.PushRange(source);
            return target;
        }

        public static implicit operator System.Collections.Generic.Queue<T>(ObservableQueue<T> obj)
        {
            return new System.Collections.Generic.Queue<T>(obj);
        }

        public static implicit operator ObservableQueue<T>(System.Collections.Generic.Queue<T> obj)
        {
            return new ObservableQueue<T>(obj);
        }
    }
}
