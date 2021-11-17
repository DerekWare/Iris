using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using Debug = DerekWare.Diagnostics.Debug;
using ThreadState = System.Threading.ThreadState;

namespace DerekWare.Threading
{
    [DefaultEvent(nameof(DoWork)), DebuggerDisplay("BackgroundThread", Name = "{" + nameof(Name) + "}")]
    public partial class BackgroundThread : Component
    {
        [Flags]
        public enum StopMode : uint
        {
            RequestCancellation = 0,
            Abort = 1u << 0,
            Wait = 1u << 1
        }

        public class DoWorkEventArgs : EventArgs
        {
            public readonly object Argument;

            public DoWorkEventArgs(object argument)
            {
                Argument = argument;
            }
        }

        public class ProgressChangedEventArgs : EventArgs
        {
            /// <summary>
            ///     Progress of the operation, between 0 and 1.
            /// </summary>
            public readonly double Progress;

            /// <summary>
            ///     Current status of the operation.
            /// </summary>
            public readonly object UserState;

            public ProgressChangedEventArgs(double progress, object userState)
            {
                if((progress < 0) || (progress > 1))
                {
                    throw new ArgumentOutOfRangeException(nameof(progress));
                }

                Progress = progress;
                UserState = userState;
            }
        }

        public class WorkCompletedEventArgs : EventArgs
        {
            /// <summary>
            ///     The exception that caused the thread to exit or null if the work completed successfully.
            /// </summary>
            public readonly Exception Exception;

            public WorkCompletedEventArgs(Exception exception)
            {
                Exception = exception;
            }
        }

        public delegate void ProgressChangedEventHandler(BackgroundThread sender, ProgressChangedEventArgs e);

        public delegate void DoWorkEventHandler(BackgroundThread sender, DoWorkEventArgs e);

        public delegate void WorkCompletedEventHandler(BackgroundThread sender, WorkCompletedEventArgs e);
    }

    public partial class BackgroundThread
    {
        string _Name;
        ThreadPriority _Priority;
        Thread _Thread;

        public event DoWorkEventHandler DoWork;
        public event ProgressChangedEventHandler ProgressChanged;
        public event WorkCompletedEventHandler WorkCompleted;

        public BackgroundThread()
            : this(null)
        {
        }

        public BackgroundThread(IContainer container)
        {
            container?.Add(this);
            InitializeComponent();
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ManagedThreadId => _Thread?.ManagedThreadId ?? 0;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ThreadState ThreadState => _Thread?.ThreadState ?? 0;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CancellationPending { get; private set; }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Enabled
        {
            get => _Thread?.IsAlive ?? false;
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

        public string Name
        {
            get => _Name;
            set
            {
                _Name = value;

                if(Enabled)
                {
                    _Thread.Name = value;
                }
            }
        }

        public ThreadPriority Priority
        {
            get => _Priority;
            set
            {
                _Priority = value;

                if(Enabled)
                {
                    _Thread.Priority = value;
                }
            }
        }

        public bool SupportsCancellation { get; set; }

        public void ReportProgress(double progress, object userState)
        {
            ProgressChanged?.ThreadedInvoke(this, new ProgressChangedEventArgs(progress, userState));
        }

        public void Start(object argument = null)
        {
            CancellationPending = false;

            if(Enabled)
            {
                return;
            }

            _Thread = new Thread(ThreadProc) { IsBackground = true, Name = Name, Priority = Priority };
            _Thread.Start(argument);
        }

        public void Stop(bool wait = true)
        {
            if(!Enabled)
            {
                return;
            }

            if(SupportsCancellation)
            {
                CancellationPending = true;
            }
            else
            {
                _Thread.Abort();
            }

            if(wait && (Thread.CurrentThread != _Thread))
            {
                _Thread.Join();
            }
        }

        void ThreadProc(object argument)
        {
            Exception exception = null;

            try
            {
                DoWork.Invoke(this, new DoWorkEventArgs(argument));
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

            CancellationPending = false;
            WorkCompleted?.ThreadedInvoke(this, new WorkCompletedEventArgs(exception));
        }
    }
}