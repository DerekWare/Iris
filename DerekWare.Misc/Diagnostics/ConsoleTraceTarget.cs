using System;

namespace DerekWare.Diagnostics
{
    public class ConsoleTraceTarget : ITraceTarget
    {
        #region IDisposable

        public void Dispose()
        {
        }

        #endregion

        #region ITraceTarget

        public void Trace(TraceContext context)
        {
            Console.WriteLine(TextFileTraceTarget.Format(context));
        }

        #endregion
    }
}
