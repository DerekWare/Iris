using System.IO;
using System.Reflection;
using System.Text;
using Path = DerekWare.IO.Path;

namespace DerekWare.Diagnostics
{
    public class TextFileTraceTarget : ITraceTarget
    {
        StreamWriter StreamWriter;

        public TextFileTraceTarget(string fileName = null, bool append = false)
        {
            if(string.IsNullOrEmpty(fileName))
            {
                fileName = GetDefaultPath();
            }

            StreamWriter = new StreamWriter(fileName, append, Encoding.UTF8);
        }

        #region IDisposable

        public void Dispose()
        {
            try
            {
                Extensions.Dispose(ref StreamWriter);
            }
            catch
            {
            }
        }

        #endregion

        #region ITraceTarget

        public void Trace(TraceContext context)
        {
            var text = Format(context);

            lock(this)
            {
                StreamWriter.WriteLine(text);
            }
        }

        #endregion

        public static string Format(TraceContext context)
        {
            var result = string.Empty;

            if(!string.IsNullOrEmpty(context.Method.MethodName))
            {
                if(!string.IsNullOrEmpty(result))
                {
                    result += " ";
                }

                result += "[";

                if(!string.IsNullOrEmpty(context.DeclaringType.FullName))
                {
                    result += context.DeclaringType.FullName + ".";
                }

                result += context.Method.MethodName + "]";
            }

            if(!string.IsNullOrEmpty(context.Message))
            {
                if(!string.IsNullOrEmpty(result))
                {
                    result += " ";
                }

                result += context.Message;
            }

            return result;
        }

        public static Path GetDefaultPath(string extension = "log")
        {
            var path = new Path(Assembly.GetEntryAssembly().GetName().Name);
            path.ChangeExtension(extension);
            return path;
        }
    }
}
