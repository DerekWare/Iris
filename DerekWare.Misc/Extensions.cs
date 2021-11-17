using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DerekWare.Diagnostics;

namespace DerekWare
{
    public static partial class Extensions
    {
        public static void Dispose<T>(T obj)
            where T : IDisposable
        {
            try
            {
                obj?.Dispose();
            }
            catch(Exception ex)
            {
                Debug.Error(null, ex);
            }
        }

        public static void Dispose<T>(ref T obj)
            where T : class, IDisposable
        {
            Dispose(Interlocked.Exchange(ref obj, null));
        }

        public static void Dispose<T>(ref T obj, Action<T> action)
            where T : class, IDisposable
        {
            var t = Interlocked.Exchange(ref obj, null);

            if(t is null)
            {
                return;
            }

            action?.Invoke(t);
            t.Dispose();
        }

        public static IEnumerable<System.Diagnostics.Process> FindOtherProcesses(this System.Diagnostics.Process process)
        {
            return from i in System.Diagnostics.Process.GetProcessesByName(process.ProcessName)
                   where i.Id != process.Id
                   where i.MainWindowHandle != IntPtr.Zero
                   select i;
        }

        public static IEnumerable<System.Diagnostics.Process> FindOtherProcesses()
        {
            return FindOtherProcesses(System.Diagnostics.Process.GetCurrentProcess());
        }

        public static void Swap<T>(ref T a, ref T b)
        {
            var c = a;
            a = b;
            b = c;
        }
    }
}
