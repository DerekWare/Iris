using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DerekWare.Collections;
using Action = System.Func<object>;

namespace DerekWare.Threading
{
    public enum TaskState
    {
        Idle,
        Executing,
        Completed,
        Failed,
        Cancelled
    }

    public class TaskCompletedEventArgs : EventArgs
    {
        internal TaskCompletedEventArgs(object result)
        {
            Result = result;
        }

        internal TaskCompletedEventArgs(Exception exception)
        {
            Exception = exception;
        }

        public Exception Exception { get; }
        public object Result { get; }
    }

    public delegate void TaskCompletedEventHandler(Task sender, TaskCompletedEventArgs e);

    public sealed class Task
    {
        readonly ManualResetEventSlim CompletionEvent = new ManualResetEventSlim();

        public event TaskCompletedEventHandler TaskCompleted;

        internal Task(TaskFactory factory, Action action)
        {
            Factory = factory;
            Action = action;
        }

        public Action Action { get; }
        public bool Cancelled => State == TaskState.Cancelled;
        public bool Completed => State >= TaskState.Completed;
        public bool Excecuting => State == TaskState.Executing;
        public TaskFactory Factory { get; }
        public bool Failed => State == TaskState.Failed;
        public WaitHandle WaitHandle => CompletionEvent.WaitHandle;
        public ThreadPriority Priority { get; set; } = ThreadPriority.Lowest;
        public TaskState State { get; private set; } = TaskState.Idle;

        public bool Cancel()
        {
            if(Excecuting)
            {
                Wait();
                return false;
            }

            State = TaskState.Cancelled;
            TaskCompleted?.ThreadedInvokeAsync(this, new TaskCompletedEventArgs(new OperationCanceledException()));
            CompletionEvent.Set();
            return true;
        }

        public void Wait()
        {
            CompletionEvent.Wait();
        }

        public bool Wait(int millisecondsTimeout)
        {
            return CompletionEvent.Wait(millisecondsTimeout);
        }

        public bool Wait(TimeSpan timeout)
        {
            return CompletionEvent.Wait(timeout);
        }

        internal void Run()
        {
            using(new ThreadPriorityBoost(Priority))
            {
                if(Cancelled)
                {
                    return;
                }

                CompletionEvent.Reset();
                State = TaskState.Executing;

                try
                {
                    var result = Action();
                    State = TaskState.Completed;
                    TaskCompleted?.ThreadedInvokeAsync(this, new TaskCompletedEventArgs(result));
                }
                catch(Exception ex)
                {
                    State = TaskState.Failed;
                    TaskCompleted?.ThreadedInvokeAsync(this, new TaskCompletedEventArgs(ex));
                }
                finally
                {
                    CompletionEvent.Set();
                }
            }
        }

        public static implicit operator WaitHandle(Task task)
        {
            return task.WaitHandle;
        }

        public static void Wait(IEnumerable<Task> tasks)
        {
            var t = tasks.SafeEmpty().Select(i => i.WaitHandle).ToArray();

            if(t.Length <= 0)
            {
                return;
            }

            WaitHandle.WaitAll(t);
        }

        public static bool Wait(IEnumerable<Task> tasks, int millisecondsTimeout)
        {
            var t = tasks.SafeEmpty().Select(i => i.WaitHandle).ToArray();
            return (t.Length <= 0) || WaitHandle.WaitAll(t, millisecondsTimeout);
        }

        public static bool Wait(IEnumerable<Task> tasks, TimeSpan timeout)
        {
            var t = tasks.SafeEmpty().Select(i => i.WaitHandle).ToArray();
            return (t.Length <= 0) || WaitHandle.WaitAll(t, timeout);
        }
    }
}
