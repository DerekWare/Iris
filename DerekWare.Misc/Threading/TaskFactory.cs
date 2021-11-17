using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DerekWare.Collections;
using DerekWare.Diagnostics;

namespace DerekWare.Threading
{
    public class TaskFactory : IReadOnlyCollection<Task>, IDisposable
    {
        protected readonly LinkedList<Task> Active = new LinkedList<Task>();
        protected readonly LinkedList<Task> Pending = new LinkedList<Task>();
        protected readonly List<Thread> Threads;
        protected readonly ManualResetEvent WakeEvent = new ManualResetEvent(false);

        public event TaskCompletedEventHandler TaskCompleted;

        public TaskFactory(string name = nameof(TaskFactory))
            : this(0, name)
        {
        }

        public TaskFactory(int capacity, string name = nameof(TaskFactory))
        {
            if(capacity <= 0)
            {
                capacity = DefaultThreadCount;
            }

            Name = name;
            Threads = new List<Thread>(capacity);
        }

        public static int DefaultThreadCount => Environment.ProcessorCount;

        public int ActiveCount
        {
            get
            {
                lock(SyncRoot)
                {
                    return Active.Count;
                }
            }
        }

        public WaitHandle[] ActiveWaitHandles
        {
            get
            {
                lock(SyncRoot)
                {
                    return Active.Select(i => i.WaitHandle).ToArray();
                }
            }
        }

        public int Count
        {
            get
            {
                lock(SyncRoot)
                {
                    return Pending.Count + Active.Count;
                }
            }
        }

        public string Name { get; }

        public int PendingCount
        {
            get
            {
                lock(SyncRoot)
                {
                    return Pending.Count;
                }
            }
        }

        public object SyncRoot { get; set; } = new object();

        /// <summary>
        ///     Pops the next node off the pending list and moves it to the active list.
        /// </summary>
        protected virtual LinkedListNode<Task> Next
        {
            get
            {
                lock(SyncRoot)
                {
                    var i = Pending.First;

                    if(i is null)
                    {
                        WakeEvent.Reset();
                        return null;
                    }

                    Pending.RemoveFirst();
                    Active.AddLast(i);

                    return i;
                }
            }
        }

        /// <summary>
        ///     Adds a task to the pending list.
        /// </summary>
        public virtual Task Add(Action action, object tag = null)
        {
            var t = new Task(this) { Action = action, Tag = tag };
            Add(t);
            return t;
        }

        /// <summary>
        ///     Adds a task to the pending list.
        /// </summary>
        public virtual void Add(Task task)
        {
            if(!ReferenceEquals(task.Factory, this))
            {
                throw new ArgumentException("Factory mismatch");
            }

            if(task.Action is null)
            {
                throw new ArgumentException("Task.Action must be non-null");
            }

            task.Reset();

            lock(SyncRoot)
            {
                Pending.AddLast(task);
                WakeEvent.Set();
                StartThreads();
            }
        }

        public void AddRange(IEnumerable<Task> tasks)
        {
            tasks.SafeEmpty().ForEach(Add);
        }

        /// <summary>
        ///     Cancels all pending tasks. Any tasks that are currently executing will continue and Clear will block until they
        ///     complete.
        /// </summary>
        public void Clear()
        {
            Clear(false, true);
        }

        /// <summary>
        ///     Cancels all pending tasks.
        /// </summary>
        /// <param name="abort">If true, any active tasks will be aborted. If false, they will be allowed to complete.</param>
        /// <param name="wait">
        ///     True to block until all active tasks have been aborted or completed. If abort is true, Clear will
        ///     always wait.
        /// </param>
        public virtual void Clear(bool abort, bool wait)
        {
            WaitHandle[] h;

            lock(SyncRoot)
            {
                Pending.Clear();
                h = ActiveWaitHandles;
            }

            if(abort)
            {
                StopThreads();
            }
            else if(wait)
            {
                h.WaitAll();
            }
        }

        public void WaitAll()
        {
            // WaitHandle.WaitAll is limited to 64 elements.
            while(true)
            {
                var h = new List<WaitHandle>();

                lock(SyncRoot)
                {
                    h.AddRange(Active.Select(i => i.WaitHandle));
                    h.AddRange(Pending.Select(i => i.WaitHandle));
                }

                if(h.Count <= 0)
                {
                    break;
                }

                h.Take(64).ToArray().WaitAll();
            }
        }

        /// <summary>
        ///     Removes a completed task from the active and notifies of its completion.
        /// </summary>
        protected virtual void OnComplete(LinkedListNode<Task> node)
        {
            lock(SyncRoot)
            {
                Debug.Assert(ReferenceEquals(node.List, Active));
                Active.Remove(node);
            }

            TaskCompleted?.ThreadedInvokeAsync(this, new TaskCompletedEventArgs(node.Value));
        }

        /// <summary>
        ///     Creates a worker thread.
        /// </summary>
        protected virtual Thread StartThread(int index)
        {
            var t = new Thread { Name = $"{Name} {index:N3}", SupportsCancellation = true };
            t.DoWork += DoWork;
            t.Start();
            return t;
        }

        protected virtual void StartThreads()
        {
            while(Threads.Count < Threads.Capacity)
            {
                Threads.Add(StartThread(Threads.Count));
            }
        }

        protected virtual void StopThreads()
        {
            Threads.ForEach(i => i.Abort(false));
            Threads.ForEach(i => i.Join());
            Threads.Clear();
        }

        #region IDisposable

        public void Dispose()
        {
            StopThreads();
            WakeEvent?.Dispose();
        }

        #endregion

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IEnumerable<Task>

        public IEnumerator<Task> GetEnumerator()
        {
            var result = new List<Task>();

            lock(SyncRoot)
            {
                result.AddRange(Pending);
                result.AddRange(Active);
            }

            return result.GetEnumerator();
        }

        #endregion

        #region Event Handlers

        void DoWork(Thread thread, DoWorkEventArgs e)
        {
            WaitHandle[] h = { thread.CancelEvent, WakeEvent };

            while(!thread.CancellationPending)
            {
                var node = Next;

                if(node is null)
                {
                    h.WaitAny();
                    continue;
                }

                node.Value.Run();
                OnComplete(node);
            }
        }

        #endregion
    }
}
