using System;
using System.Threading;

namespace DerekWare.Collections
{
    public class LimitedQueue<T> : SynchronizedQueue<T>
    {
        protected int _MaxCount;
        protected ManualResetEventSlim PopEvent;
        protected SemaphoreSlim PushSemaphore;

        public LimitedQueue(int maxCount)
        {
            _MaxCount = maxCount;
            PushSemaphore = new SemaphoreSlim(0, _MaxCount);
            PopEvent = new ManualResetEventSlim(false);
        }

        public int MaxCount => _MaxCount;
        public WaitHandle PopWaitHandle => PopEvent.WaitHandle;
        public WaitHandle PushWaitHandle => PushSemaphore.AvailableWaitHandle;

        public override void Clear()
        {
            lock(SyncRoot)
            {
                PopEvent.Reset();
                PushSemaphore.Release(Count);
                base.Clear();
            }
        }

        public sealed override T Pop()
        {
            if(!TryPop(-1, out var item))
            {
                throw new InvalidOperationException();
            }

            return item;
        }

        public sealed override void Push(T item)
        {
            Push(item, -1);
        }

        public bool Push(T item, TimeSpan timeout)
        {
            return Push(item, (int)timeout.TotalMilliseconds);
        }

        public virtual bool Push(T item, int millisecondsTimeout)
        {
            // TODO retry timeout should be decremented
            do
            {
                lock(SyncRoot)
                {
                    if(PushSemaphore.Wait(0))
                    {
                        base.Push(item);
                        return true;
                    }
                }
            }
            while(PushSemaphore.Wait(millisecondsTimeout));

            return false;
        }

        public bool TryPop(TimeSpan timeout, out T item)
        {
            return TryPop((int)timeout.TotalMilliseconds, out item);
        }

        public virtual bool TryPop(int millisecondsTimeout, out T item)
        {
            // TODO retry timeout should be decremented
            while(PopEvent.Wait(millisecondsTimeout))
            {
                lock(SyncRoot)
                {
                    if(base.TryPop(out item))
                    {
                        if(Count <= 0)
                        {
                            PopEvent.Reset();
                        }

                        PushSemaphore.Release();
                        return true;
                    }
                }
            }

            item = default;
            return false;
        }
    }
}
