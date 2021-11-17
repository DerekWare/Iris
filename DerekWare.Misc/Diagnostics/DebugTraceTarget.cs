using DerekWare.Strings;

namespace DerekWare.Diagnostics
{
    public class DebugTraceTarget : ITraceTarget
    {
        #region IDisposable

        public void Dispose()
        {
        }

        #endregion

        #region ITraceTarget

        public void Trace(TraceContext context)
        {
            System.Diagnostics.Debug.WriteLine(Format(context));
        }

        #endregion

        public static string Format(TraceContext context)
        {
            var text = $"{context.Method.SourceFileName}({context.Method.SourceFileLine}):";

            if(!string.IsNullOrEmpty(context.ObjectValue))
            {
                text += " " + context.ObjectValue.Surround("[", "]");
            }

            if(!string.IsNullOrEmpty(context.Message))
            {
                text += " " + context.Message;
            }

            return text;
        }
    }
}
