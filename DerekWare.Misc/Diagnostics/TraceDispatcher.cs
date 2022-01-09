using System;
using System.Collections;
using System.Collections.Generic;
using DerekWare.Collections;
using DerekWare.Threading;

namespace DerekWare.Diagnostics
{
    public class TraceDispatcher : ITraceTarget, ICollection<ITraceTarget>
    {
        readonly Thread DispatchThread = new() { KeepAlive = true };
        readonly SynchronizedQueue<TraceContext> Pending = new();
        readonly List<ITraceTarget> Targets = new();

        public TraceDispatcher()
        {
            DispatchThread.DoWork += Dispatch;
            DispatchThread.Start();
        }

        public int Count
        {
            get
            {
                lock(Targets)
                {
                    return Targets.Count;
                }
            }
        }

        public bool IsReadOnly => false;

        #region ICollection<ITraceTarget>

        public void Add(ITraceTarget item)
        {
            lock(Targets)
            {
                Targets.Add(item);
            }
        }

        public void Clear()
        {
            lock(Targets)
            {
                Targets.Clear();
            }
        }

        public bool Contains(ITraceTarget item)
        {
            lock(Targets)
            {
                return Targets.Contains(item);
            }
        }

        public void CopyTo(ITraceTarget[] array, int arrayIndex)
        {
            lock(Targets)
            {
                Targets.CopyTo(array, arrayIndex);
            }
        }

        public bool Remove(ITraceTarget item)
        {
            lock(Targets)
            {
                return Targets.Remove(item);
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            DispatchThread.Stop();

            lock(Targets)
            {
                foreach(var t in Targets)
                {
                    t.Dispose();
                }

                Targets.Clear();
            }
        }

        #endregion

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IEnumerable<ITraceTarget>

        public IEnumerator<ITraceTarget> GetEnumerator()
        {
            lock(Targets)
            {
                return Targets.CreateEnumerator();
            }
        }

        #endregion

        #region ITraceTarget

        public void Trace(TraceContext context)
        {
            Pending.Push(context);
            DispatchThread.Start();
        }

        #endregion

        #region Event Handlers

        void Dispatch(object sender, EventArgs e)
        {
            while(Pending.TryPop(out var c))
            {
                lock(Targets)
                {
                    foreach(var t in Targets)
                    {
                        t.Trace(c);
                    }
                }
            }
        }

        #endregion
    }
}
