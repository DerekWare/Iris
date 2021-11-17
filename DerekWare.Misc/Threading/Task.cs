using System;
using System.Threading;

namespace DerekWare.Threading
{
    public delegate void TaskCompletedEventHandler(object sender, TaskCompletedEventArgs e);

    /// <summary>
    ///     A task is simply a wrapper around an Action and a WaitHandle that's signaled when the task completes. Tasks are
    ///     managed by TaskFactory which uses a ThreadPool to execute. After the task has run, TaskCompleted is called
    ///     asynchronously. Event handlers should check the Exception property to determine if the task succeeded.
    /// </summary>
    public class Task
    {
        protected readonly ManualResetEventSlim CompletionEvent = new ManualResetEventSlim();

        public event TaskCompletedEventHandler TaskCompleted;

        public Task(TaskFactory factory)
        {
            Factory = factory;
        }

        public TaskFactory Factory { get; }
        public WaitHandle WaitHandle => CompletionEvent.WaitHandle;
        public Action Action { get; set; }
        public Exception Exception { get; protected set; }
        public object Tag { get; set; }

        public virtual void Reset()
        {
            CompletionEvent.Reset();
            Exception = null;
        }

        public virtual void Run()
        {
            Reset();

            try
            {
                Action();
            }
            catch(Exception ex)
            {
                Exception = ex;
            }

            CompletionEvent.Set();
            TaskCompleted?.ThreadedInvokeAsync(this, new TaskCompletedEventArgs(this));
        }

        public static implicit operator WaitHandle(Task task)
        {
            return task.WaitHandle;
        }
    }

    public class TaskCompletedEventArgs : EventArgs
    {
        public readonly Task Task;

        public TaskCompletedEventArgs(Task task)
        {
            Task = task;
        }
    }
}
