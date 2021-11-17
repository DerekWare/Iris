using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace DerekWare.Collections
{
    public class SynchronizedQueue<T> : ObservableQueue<T>
    {
        public override event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                lock(SyncRoot)
                {
                    base.CollectionChanged += value;
                }
            }
            remove
            {
                lock(SyncRoot)
                {
                    base.CollectionChanged -= value;
                }
            }
        }

        public override event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                lock(SyncRoot)
                {
                    base.PropertyChanged += value;
                }
            }
            remove
            {
                lock(SyncRoot)
                {
                    base.PropertyChanged -= value;
                }
            }
        }

        public SynchronizedQueue()
        {
        }

        public SynchronizedQueue(int capacity)
            : base(capacity)
        {
        }

        public SynchronizedQueue(IEnumerable<T> items)
            : base(items)
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

        public override void CopyTo(Array array, int index)
        {
            lock(SyncRoot)
            {
                base.CopyTo(array, index);
            }
        }

        public override void CopyTo(T[] array, int arrayIndex)
        {
            lock(SyncRoot)
            {
                base.CopyTo(array, arrayIndex);
            }
        }

        public override IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)ToArray()).GetEnumerator();
        }

        public override T Peek()
        {
            lock(SyncRoot)
            {
                return base.Peek();
            }
        }

        public override T Pop()
        {
            lock(SyncRoot)
            {
                return base.Pop();
            }
        }

        public override void Push(T item)
        {
            lock(SyncRoot)
            {
                base.Push(item);
            }
        }

        public override void PushRange(IEnumerable<T> items)
        {
            lock(SyncRoot)
            {
                base.PushRange(items);
            }
        }

        public override T[] ToArray()
        {
            lock(SyncRoot)
            {
                return base.ToArray();
            }
        }

        public override bool TryPeek(out T item)
        {
            lock(SyncRoot)
            {
                return base.TryPeek(out item);
            }
        }

        public override bool TryPop(out T item)
        {
            lock(SyncRoot)
            {
                return base.TryPop(out item);
            }
        }
    }
}
