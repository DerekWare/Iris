using System;
using System.Threading;

namespace DerekWare.Threading
{
    public delegate void CancellationRequestedEventHandler(Thread sender, CancellationRequestedEventArgs e);

    public delegate void DoWorkEventHandler(Thread sender, DoWorkEventArgs e);

    public delegate void ProgressChangedEventHandler(Thread sender, ProgressChangedEventArgs e);

    public delegate void WorkCompletedEventHandler(Thread sender, WorkCompletedEventArgs e);

    public interface IThread
    {
        event CancellationRequestedEventHandler CancellationRequested;
        event DoWorkEventHandler DoWork;
        event ProgressChangedEventHandler ProgressChanged;
        event WorkCompletedEventHandler WorkCompleted;

        object SyncRoot { get; }
        ApartmentState ApartmentState { get; set; }
        EventWaitHandle CancelEvent { get; set; }
        bool KeepAlive { get; set; }
        string Name { get; set; }
        ThreadPriority Priority { get; set; }
        bool SupportsCancellation { get; set; }
        EventWaitHandle WorkEvent { get; set; }

        void Abort(bool wait = true);
        void Dispose();
        void Join();
        void Start();
        void Stop(bool wait = true);
    }

    public class CancellationRequestedEventArgs : EventArgs
    {
        public static readonly CancellationRequestedEventArgs Default = new CancellationRequestedEventArgs();
    }

    public class DoWorkEventArgs : EventArgs
    {
        /// <summary>
        ///     Result data to be passed to WorkCompleted.
        /// </summary>
        public object Result;
    }

    public class ProgressChangedEventArgs : EventArgs
    {
        /// <summary>
        ///     Context data passed through ReportProgress.
        /// </summary>
        public readonly object Context;

        /// <summary>
        ///     Total progress duration.
        /// </summary>
        public readonly long Length;

        /// <summary>
        ///     Current progress position.
        /// </summary>
        public readonly long Position;

        public ProgressChangedEventArgs(long position, long length, object context)
        {
            if((position < 0) || (position > length))
            {
                throw new ArgumentOutOfRangeException(nameof(position));
            }

            Position = position;
            Length = length;
            Context = context;
        }

        public double ProgressPercentage
        {
            get
            {
                if((Position <= 0) || (Length <= 0))
                {
                    return 0;
                }

                return (Length * 100.0) / Position;
            }
        }
    }

    public class WorkCompletedEventArgs : EventArgs
    {
        /// <summary>
        ///     Context data passed through Start.
        /// </summary>
        public readonly object Context;

        /// <summary>
        ///     The exception that caused the thread to exit or null if the work completed successfully.
        /// </summary>
        public readonly Exception Exception;

        /// <summary>
        ///     Result data from the DoWork handler.
        /// </summary>
        public readonly object Result;

        public WorkCompletedEventArgs(object result, Exception exception)
        {
            Result = result;
            Exception = exception;
        }
    }
}
