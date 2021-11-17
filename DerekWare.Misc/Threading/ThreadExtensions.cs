using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace DerekWare.Threading
{
    public static class ThreadExtensions
    {
        /// <summary>
        ///     Invokes a delegate from a worker thread.
        /// </summary>
        public static void ThreadedInvoke(this Delegate @this, params object[] args)
        {
            foreach(var i in @this.GetInvocationList())
            {
                if(i.Target is ISynchronizeInvoke synchronizeInvoke && synchronizeInvoke.InvokeRequired)
                {
                    synchronizeInvoke.Invoke(i, args);
                }
                else
                {
                    i.DynamicInvoke(args);
                }
            }
        }

        /// <summary>
        ///     Invokes a delegate from a worker thread asynchronously when possible.
        /// </summary>
        public static void ThreadedInvokeAsync(this Delegate @this, params object[] args)
        {
            foreach(var i in @this.GetInvocationList())
            {
                if(i.Target is ISynchronizeInvoke synchronizeInvoke)
                {
                    synchronizeInvoke.BeginInvoke(i, args);
                }
                else
                {
                    i.DynamicInvoke(args);
                }
            }
        }

        public static void WaitAll(this WaitHandle[] items)
        {
            WaitHandle.WaitAll(items);
        }

        public static bool WaitAll(this WaitHandle[] items, int millisecondsTimeout)
        {
            return WaitHandle.WaitAll(items, millisecondsTimeout);
        }

        public static bool WaitAll(this WaitHandle[] items, TimeSpan timeout)
        {
            return WaitHandle.WaitAll(items, timeout);
        }

        public static void WaitAll(this IEnumerable<WaitHandle> items)
        {
            WaitHandle.WaitAll(items.ToArray());
        }

        public static bool WaitAll(this IEnumerable<WaitHandle> items, int millisecondsTimeout)
        {
            return WaitHandle.WaitAll(items.ToArray(), millisecondsTimeout);
        }

        public static bool WaitAll(this IEnumerable<WaitHandle> items, TimeSpan timeout)
        {
            return WaitHandle.WaitAll(items.ToArray(), timeout);
        }

        public static int WaitAny(this WaitHandle[] items)
        {
            return WaitHandle.WaitAny(items);
        }

        public static int WaitAny(this WaitHandle[] items, int millisecondsTimeout)
        {
            return WaitHandle.WaitAny(items, millisecondsTimeout);
        }

        public static int WaitAny(this WaitHandle[] items, TimeSpan timeout)
        {
            return WaitHandle.WaitAny(items, timeout);
        }

        public static int WaitAny(this IEnumerable<WaitHandle> items)
        {
            return WaitHandle.WaitAny(items.ToArray());
        }

        public static int WaitAny(this IEnumerable<WaitHandle> items, int millisecondsTimeout)
        {
            return WaitHandle.WaitAny(items.ToArray(), millisecondsTimeout);
        }

        public static int WaitAny(this IEnumerable<WaitHandle> items, TimeSpan timeout)
        {
            return WaitHandle.WaitAny(items.ToArray(), timeout);
        }
    }
}
