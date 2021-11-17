using System;
using System.Diagnostics;
using DerekWare.Strings;

namespace DerekWare.Diagnostics
{
    public static class Debug
    {
        public static readonly TraceDispatcher DefaultTraceTarget = new TraceDispatcher();

        static Debug()
        {
            DefaultTraceTarget?.Add(new DebugTraceTarget());
        }

        [Conditional("DEBUG")]
        public static void Assert(bool condition)
        {
            if(!condition)
            {
                Break();
            }
        }

        public static void Assert(bool condition, string message, params object[] args)
        {
            if(!condition)
            {
                DefaultTraceTarget?.Trace(new TraceContext(TraceType.Error, 1, null, message, args));
                Break();
            }
        }

        public static void Assert(this ITraceTarget target, bool condition, string message, params object[] args)
        {
            if(condition)
            {
                return;
            }

            target.Trace(new TraceContext(TraceType.Error, 1, null, message, args));
            Break();
        }

        [Conditional("DEBUG")]
        public static void Break()
        {
            Debugger.Break();
        }

        public static void Dispose()
        {
            DefaultTraceTarget?.Dispose();
        }

        public static void Error(object @this, string message, params object[] args)
        {
            DefaultTraceTarget?.Trace(new TraceContext(TraceType.Error, 1, @this, message, args));
            Break();
        }

        public static void Error(object @this, Exception ex)
        {
            while(null != ex)
            {
                DefaultTraceTarget?.Trace(new TraceContext(TraceType.Error, 1, @this, ex.Message + Environment.NewLine + ex.StackTrace));
                ex = ex.InnerException;
            }

            Break();
        }

        public static void Error(this ITraceTarget target, object @this, string message, params object[] args)
        {
            target.Trace(new TraceContext(TraceType.Error, 1, @this, message, args));
            Break();
        }

        public static void Error(this ITraceTarget target, object @this, Exception ex)
        {
            while(null != ex)
            {
                target.Trace(new TraceContext(TraceType.Error, 1, @this, ex.Message + Environment.NewLine + ex.StackTrace));
                ex = ex.InnerException;
            }

            Break();
        }

        public static void Error(object @this, object obj)
        {
            DefaultTraceTarget?.Trace(new TraceContext(TraceType.Error, 1, @this, obj.SafeToString()));
        }

        public static void Trace(TraceContext context)
        {
            DefaultTraceTarget?.Trace(context);
        }

        public static void Trace(TraceType type, object @this, string message, params object[] args)
        {
            DefaultTraceTarget?.Trace(new TraceContext(type, 1, @this, message, args));
        }

        public static void Trace(TraceType type, object @this, Exception ex)
        {
            while(null != ex)
            {
                DefaultTraceTarget?.Trace(new TraceContext(type, 1, @this, ex.Message + Environment.NewLine + ex.StackTrace));
                ex = ex.InnerException;
            }
        }

        public static void Trace(object @this, string message, params object[] args)
        {
            DefaultTraceTarget?.Trace(new TraceContext(TraceType.Info, 1, @this, message, args));
        }

        public static void Trace(object @this, object obj)
        {
            DefaultTraceTarget?.Trace(new TraceContext(TraceType.Info, 1, @this, obj.SafeToString()));
        }

        public static void Trace(object @this, Exception ex)
        {
            while(null != ex)
            {
                DefaultTraceTarget?.Trace(new TraceContext(TraceType.Info, 1, @this, ex.Message + Environment.NewLine + ex.StackTrace));
                ex = ex.InnerException;
            }
        }

        public static void Trace(this ITraceTarget target, TraceType type, object @this, string message, params object[] args)
        {
            target.Trace(new TraceContext(type, 1, @this, message, args));
        }

        public static void Trace(this ITraceTarget target, TraceType type, object @this, Exception ex)
        {
            while(null != ex)
            {
                target.Trace(new TraceContext(type, 1, @this, ex.Message + Environment.NewLine + ex.StackTrace));
                ex = ex.InnerException;
            }
        }

        public static void Trace(this ITraceTarget target, object @this, string message, params object[] args)
        {
            target.Trace(new TraceContext(TraceType.Info, 1, @this, message, args));
        }

        public static void Trace(this ITraceTarget target, object @this, Exception ex)
        {
            while(null != ex)
            {
                target.Trace(new TraceContext(TraceType.Info, 1, @this, ex.Message + Environment.NewLine + ex.StackTrace));
                ex = ex.InnerException;
            }
        }

        public static void Warning(object @this, object obj)
        {
            DefaultTraceTarget?.Trace(new TraceContext(TraceType.Warning, 1, @this, obj.SafeToString()));
        }

        public static void Warning(object @this, string message, params object[] args)
        {
            DefaultTraceTarget?.Trace(new TraceContext(TraceType.Warning, 1, @this, message, args));
        }

        public static void Warning(object @this, Exception ex)
        {
            while(null != ex)
            {
                DefaultTraceTarget?.Trace(new TraceContext(TraceType.Warning, 1, @this, ex.Message + Environment.NewLine + ex.StackTrace));
                ex = ex.InnerException;
            }
        }

        public static void Warning(this ITraceTarget target, object @this, string message, params object[] args)
        {
            target.Trace(new TraceContext(TraceType.Warning, 1, @this, message, args));
        }

        public static void Warning(this ITraceTarget target, object @this, Exception ex)
        {
            while(null != ex)
            {
                target.Trace(new TraceContext(TraceType.Warning, 1, @this, ex.Message + Environment.NewLine + ex.StackTrace));
                ex = ex.InnerException;
            }
        }
    }
}
