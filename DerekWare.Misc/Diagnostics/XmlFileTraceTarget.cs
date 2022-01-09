using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace DerekWare.Diagnostics
{
    public class XmlFileTraceTarget : ITraceTarget
    {
        long IsDisposed;
        StreamWriter StreamWriter;
        XmlSerializer XmlSerializer = new(typeof(TraceContext));
        XmlWriter XmlWriter;

        public XmlFileTraceTarget(string fileName = null, bool append = false)
        {
            if(string.IsNullOrEmpty(fileName))
            {
                fileName = TextFileTraceTarget.GetDefaultPath("xml");
            }

            StreamWriter = new StreamWriter(fileName, append);

            var settings = new XmlWriterSettings { Indent = true, IndentChars = "\t" };

            XmlWriter = XmlWriter.Create(StreamWriter, settings);
            XmlWriter.WriteStartDocument();
            XmlWriter.WriteStartElement("DerekWare.Diagnostics");
        }

        ~XmlFileTraceTarget()
        {
            Dispose();
        }

        #region IDisposable

        public void Dispose()
        {
            if(0 != Interlocked.Exchange(ref IsDisposed, 1))
            {
                return;
            }

            XmlSerializer = null;

            XmlWriter.WriteEndElement();
            XmlWriter.WriteEndDocument();
            XmlWriter.Close();
            XmlWriter = null;

            try
            {
                StreamWriter.Dispose();
            }
            catch
            {
            }

            StreamWriter = null;
        }

        #endregion

        #region ITraceTarget

        public void Trace(TraceContext context)
        {
            lock(this)
            {
                XmlSerializer.Serialize(XmlWriter, context);
            }
        }

        #endregion
    }
}
