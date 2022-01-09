using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using Debug = DerekWare.Diagnostics.Debug;
using ThreadState = System.Threading.ThreadState;

namespace DerekWare.Threading
{
    /// <summary>
    ///     Thread is similar to Forms.BackgroundWorker except that it spawns a proper background thread that will not
    ///     block program exit. It also supports a forcible abort via exception as well as graceful cancellation.
    /// </summary>
    [DefaultEvent(nameof(DoWork)), DebuggerDisplay("{" + nameof(Name) + "}")]
    public class Thread : IThread
    {
        public static ApartmentState DefaultApartmentState = ApartmentState.MTA;
        public static ThreadPriority DefaultThreadPriority = ThreadPriority.Lowest;

        protected System.Threading.Thread SystemThread;

        ApartmentState _ApartmentState;
        string _Name;
        ThreadPriority _Priority;

        public event CancellationRequestedEventHandler CancellationRequested;
        public event DoWorkEventHandler DoWork;
        public event ProgressChangedEventHandler ProgressChanged;
        public event WorkCompletedEventHandler WorkCompleted;

        public Thread()
        {
            _Name = GetType().Name;
            _ApartmentState = DefaultApartmentState;
            _Priority = DefaultThreadPriority;
        }

        ~Thread()
        {
            Dispose(false);
        }

        /// <summary>
        ///     Is the worker thread the calling thread?
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsCurrentThread => System.Threading.Thread.CurrentThread == SystemThread;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ManagedThreadId => SystemThread?.ManagedThreadId ?? 0;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ThreadState ThreadState => SystemThread?.ThreadState ?? 0;

        public virtual ApartmentState ApartmentState
        {
            get
            {
                lock(SyncRoot)
                {
                    return _ApartmentState;
                }
            }
            set
            {
                lock(SyncRoot)
                {
                    _ApartmentState = value;
                    SystemThread?.SetApartmentState(_ApartmentState);
                }
            }
        }

        /// <summary>
        ///     The event signaled when cancellation has been requested. A DoWork handler that loops should periodically check
        ///     CancellationPending or CancelEvent.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public EventWaitHandle CancelEvent { get; set; } = new ManualResetEvent(false);

        /// <summary>
        ///     A cancellation has been requested.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool CancellationPending
        {
            get => CancelEvent.WaitOne(0);
            set
            {
                lock(SyncRoot)
                {
                    if(value && !SupportsCancellation)
                    {
                        throw new InvalidOperationException($"{nameof(SupportsCancellation)} must be {true} to trigger a cancellation.");
                    }

                    if(value)
                    {
                        CancelEvent.Set();
                        CancellationRequested?.ThreadedInvokeAsync(this, CancellationRequestedEventArgs.Default);
                    }
                    else
                    {
                        CancelEvent.Reset();
                    }
                }
            }
        }

        /// <summary>
        ///     The thread is running (although may be idle).
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsEnabled
        {
            get => SystemThread?.IsAlive ?? false;
            set
            {
                if(value)
                {
                    Start();
                }
                else
                {
                    Stop();
                }
            }
        }

        /// <summary>
        ///     If true, the thread will continuously call DoWork until it's stopped.
        /// </summary>
        public bool KeepAlive { get; set; }

        public virtual string Name
        {
            get
            {
                lock(SyncRoot)
                {
                    return _Name;
                }
            }
            set
            {
                _Name = value;

                if(!(SystemThread is null))
                {
                    SystemThread.Name = _Name;
                }
            }
        }

        public virtual ThreadPriority Priority
        {
            get
            {
                lock(SyncRoot)
                {
                    return _Priority;
                }
            }
            set
            {
                lock(SyncRoot)
                {
                    _Priority = value;

                    if(!(SystemThread is null))
                    {
                        SystemThread.Priority = _Priority;
                    }
                }
            }
        }

        /// <summary>
        ///     Does the thread support graceful cancellation or must it be terminated? Threads may gracefully exit by checking the
        ///     CancellationPending or CancelEvent properties and returning from their DoWork handler.
        /// </summary>
        public virtual bool SupportsCancellation { get; set; }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object SyncRoot { get; set; } = new();

        /// <summary>
        ///     User context.
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        ///     The event signaled when the thread should start [re]running.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public EventWaitHandle WorkEvent { get; set; } = new AutoResetEvent(false);

        /// <summary>
        ///     Asynchronously posts a progress changed event.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="length"></param>
        /// <param name="context"></param>
        public virtual void ReportProgress(long position, long length, object context)
        {
            ProgressChanged?.ThreadedInvokeAsync(this, new ProgressChangedEventArgs(position, length, context));
        }

        public override string ToString()
        {
            return Name;
        }

        protected virtual void Dispose(bool disposing)
        {
            if(!IsEnabled)
            {
                return;
            }

            if(disposing)
            {
                Stop();
            }
            else
            {
                Abort();
            }
        }

        protected virtual void OnCancellationRequested()
        {
            CancellationRequested?.ThreadedInvoke(this, CancellationRequestedEventArgs.Default);
        }

        protected virtual void OnDoWork(out object result)
        {
            var e = new DoWorkEventArgs();
            DoWork.Invoke(this, e);
            result = e.Result;
        }

        protected virtual void OnWorkCompleted(object result, Exception exception)
        {
            WorkCompleted?.ThreadedInvokeAsync(this, new WorkCompletedEventArgs(result, exception));
        }

        void ThreadProc()
        {
            WaitHandle[] h = { WorkEvent, CancelEvent };

            do
            {
                WaitHandle.WaitAny(h);

                if(CancellationPending)
                {
                    break;
                }

                object result = null;
                Exception exception = null;

                try
                {
                    OnDoWork(out result);
                }
                catch(ThreadAbortException ex)
                {
                    exception = ex;
                }
                catch(OperationCanceledException ex)
                {
                    exception = ex;
                }
                catch(Exception ex)
                {
                    Debug.Error(this, ex);
                    exception = ex;
                }
                finally
                {
                    OnWorkCompleted(result, exception);
                }
            }
            while(KeepAlive);
        }

        #region IThread

        /// <summary>
        ///     Stops the thread by terminating it.
        /// </summary>
        /// <param name="wait">True to block until the thread has stopped.</param>
        public virtual void Abort(bool wait = true)
        {
            lock(SyncRoot)
            {
                if(!IsEnabled)
                {
                    return;
                }

                SystemThread.Abort();

                if(!wait)
                {
                    return;
                }
            }

            SystemThread.Join();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        ///     Waits for the thread to stop.
        /// </summary>
        public virtual void Join()
        {
            lock(SyncRoot)
            {
                if(IsCurrentThread)
                {
                    throw new InvalidOperationException("Can't call Join from the worker thread");
                }

                if(!IsEnabled)
                {
                    return;
                }
            }

            SystemThread.Join();
        }

        /// <summary>
        ///     Starts the thread running.
        /// </summary>
        public virtual void Start()
        {
            lock(SyncRoot)
            {
                CancelEvent.Reset();
                WorkEvent.Set();

                if(IsEnabled)
                {
                    return;
                }

                SystemThread = new System.Threading.Thread(ThreadProc) { IsBackground = true, Name = _Name, Priority = _Priority };
                SystemThread.SetApartmentState(_ApartmentState);
                SystemThread.Start();
            }
        }

        /// <summary>
        ///     Stops the thread by issuing a cancellation request or terminating it.
        /// </summary>
        /// <param name="wait">True to block until the thread has stopped.</param>
        public virtual void Stop(bool wait = true)
        {
            lock(SyncRoot)
            {
                if(IsCurrentThread && (!SupportsCancellation || wait))
                {
                    throw new InvalidOperationException("Can't call Stop from the worker thread");
                }

                if(!IsEnabled)
                {
                    return;
                }

                if(SupportsCancellation)
                {
                    CancellationPending = true;
                }
                else
                {
                    SystemThread.Abort();
                }
            }

            if(!wait)
            {
                return;
            }

            SystemThread.Join();
        }

        #endregion
    }
}
