using System;
using System.Threading;

namespace DerekWare.Threading
{
    public class ThreadPriorityBoost : IDisposable
    {
        public readonly ThreadPriority BoostedPriority;
        public readonly ThreadPriority DefaultPriority;

        int IsDisposed;

        public ThreadPriorityBoost(ThreadPriority priority, bool allowReduction = false)
        {
            DefaultPriority = System.Threading.Thread.CurrentThread.Priority;
            BoostedPriority = priority;

            if((BoostedPriority > DefaultPriority) || allowReduction)
            {
                System.Threading.Thread.CurrentThread.Priority = BoostedPriority;
            }
            else
            {
                IsDisposed = 1;
            }
        }

        #region IDisposable

        public void Dispose()
        {
            if(0 == Interlocked.Exchange(ref IsDisposed, 1))
            {
                System.Threading.Thread.CurrentThread.Priority = DefaultPriority;
            }
        }

        #endregion
    }
}
