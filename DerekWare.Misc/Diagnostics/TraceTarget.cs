using System;
using System.Diagnostics;
using System.Reflection;
using System.Xml.Serialization;
using DerekWare.Strings;

namespace DerekWare.Diagnostics
{
    public enum TraceType
    {
        Error,
        Warning,
        Info
    }

    [XmlType("Trace")]
    public struct TraceContext
    {
        public struct MethodContext
        {
            public readonly string MethodName;
            public readonly string ModuleName;
            public readonly int SourceFileLine;
            public readonly string SourceFileName;

            public MethodContext(StackFrame frame, MethodBase method)
            {
                MethodName = method?.Name;
                ModuleName = method?.Module.Name;
                SourceFileName = frame?.GetFileName();
                SourceFileLine = frame?.GetFileLineNumber() ?? 0;
            }
        }

        public struct TypeContext
        {
            public readonly string FullName;
            public readonly string Name;

            public TypeContext(Type type)
            {
                Name = type?.Name;
                FullName = type?.FullName;
            }
        }

        public readonly TypeContext DeclaringType;
        public readonly string Message;
        public readonly MethodContext Method;
        public readonly TypeContext ObjectType;
        public readonly string ObjectValue;
        public readonly DateTime TimeStamp;
        public readonly TraceType TraceType;

        public TraceContext(TraceType traceType, int stackFrameIndex, object obj, string message, params object[] args)
        {
            var frame = new StackFrame(stackFrameIndex + 1, true);
            var method = frame.GetMethod();

            TraceType = traceType;
            TimeStamp = DateTime.Now;
            ObjectValue = obj?.ToString();
            Method = new MethodContext(frame, method);
            DeclaringType = new TypeContext(method?.DeclaringType);
            ObjectType = new TypeContext(obj?.GetType());
            Message = message.Format(args);
        }
    }

    public interface ITraceTarget : IDisposable
    {
        void Trace(TraceContext context);
    }
}
