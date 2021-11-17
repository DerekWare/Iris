using System;
using System.Threading;
using DerekWare.Collections;
using DerekWare.Strings;

namespace DerekWare.Threading
{
    /// <summary>
    ///     Implements IThread using a a collection of threads that share the same properties.
    /// </summary>
    public class ThreadCollection : IThread
    {
        public static int DefaultThreadCapacity = Environment.ProcessorCount;

        protected readonly Thread[] Threads;

        string _Name;

        public event CancellationRequestedEventHandler CancellationRequested;
        public event DoWorkEventHandler DoWork;
        public event ProgressChangedEventHandler ProgressChanged;
        public event WorkCompletedEventHandler WorkCompleted;

        public ThreadCollection()
            : this(0)
        {
        }

        public ThreadCollection(int capacity)
        {
            if(capacity <= 0)
            {
                capacity = DefaultThreadCapacity;
            }

            Threads = new Thread[capacity];

            for(var i = 0; i < capacity; ++i)
            {
                Threads[i] = new Thread();
                Threads[i].CancellationRequested += OnCancellationRequested;
                Threads[i].DoWork += OnDoWork;
                Threads[i].ProgressChanged += OnProgressChanged;
                Threads[i].WorkCompleted += OnWorkCompleted;
            }

            Name = null;
            CancelEvent = Threads[0].CancelEvent;
            WorkEvent = Threads[0].WorkEvent;
            SyncRoot = Threads[0].SyncRoot;
        }

        public ApartmentState ApartmentState
        {
            get
            {
                lock(SyncRoot)
                {
                    return Threads[0].ApartmentState;
                }
            }
            set
            {
                lock(SyncRoot)
                {
                    Threads.ForEach(i => i.ApartmentState = value);
                }
            }
        }

        public EventWaitHandle CancelEvent
        {
            get
            {
                lock(SyncRoot)
                {
                    return Threads[0].CancelEvent;
                }
            }
            set
            {
                lock(SyncRoot)
                {
                    Threads.ForEach(i => i.CancelEvent = value);
                }
            }
        }

        public bool KeepAlive
        {
            get
            {
                lock(SyncRoot)
                {
                    return Threads[0].KeepAlive;
                }
            }
            set
            {
                lock(SyncRoot)
                {
                    Threads.ForEach(i => i.KeepAlive = value);
                }
            }
        }

        public string Name
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
                lock(SyncRoot)
                {
                    _Name = value.IfNullOrEmpty(GetType().Name);
                    Threads.Length.For(i => Threads[i].Name = FormatName(i));
                }
            }
        }

        public ThreadPriority Priority
        {
            get
            {
                lock(SyncRoot)
                {
                    return Threads[0].Priority;
                }
            }
            set
            {
                lock(SyncRoot)
                {
                    Threads.ForEach(i => i.Priority = value);
                }
            }
        }

        public bool SupportsCancellation
        {
            get
            {
                lock(SyncRoot)
                {
                    return Threads[0].SupportsCancellation;
                }
            }
            set
            {
                lock(SyncRoot)
                {
                    Threads.ForEach(i => i.SupportsCancellation = value);
                }
            }
        }

        public object SyncRoot { get => Threads[0].SyncRoot; private set { Threads.ForEach(i => i.SyncRoot = value); } }

        public EventWaitHandle WorkEvent
        {
            get
            {
                lock(SyncRoot)
                {
                    return Threads[0].WorkEvent;
                }
            }
            set
            {
                lock(SyncRoot)
                {
                    Threads.ForEach(i => i.WorkEvent = value);
                }
            }
        }

        protected string FormatName(int index)
        {
            return $"{_Name} {index}";
        }

        #region IThread

        public void Abort(bool wait = true)
        {
            Threads.ForEach(i => i.Abort(false));

            if(!wait)
            {
                return;
            }

            Threads.ForEach(i => i.Join());
        }

        public void Dispose()
        {
            Threads.ForEach(i => i.Dispose());
        }

        public void Join()
        {
            Threads.ForEach(i => i.Join());
        }

        public void Start()
        {
            Threads.ForEach(i => i.Start());
        }

        public void Stop(bool wait = true)
        {
            Threads.ForEach(i => i.Stop(false));

            if(!wait)
            {
                return;
            }

            Threads.ForEach(i => i.Join());
        }

        #endregion

        #region Event Handlers

        void OnCancellationRequested(Thread sender, CancellationRequestedEventArgs e)
        {
            CancellationRequested?.Invoke(sender, e);
        }

        void OnDoWork(Thread sender, DoWorkEventArgs e)
        {
            DoWork?.Invoke(sender, e);
        }

        void OnProgressChanged(Thread sender, ProgressChangedEventArgs e)
        {
            ProgressChanged?.Invoke(sender, e);
        }

        void OnWorkCompleted(Thread sender, WorkCompletedEventArgs e)
        {
            WorkCompleted?.Invoke(sender, e);
        }

        #endregion
    }
}
