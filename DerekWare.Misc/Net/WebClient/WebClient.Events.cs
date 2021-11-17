using System;
using DerekWare.IO;

namespace DerekWare.Net
{
    public partial class WebClient : System.Net.WebClient
    {
        public delegate void DownloadDataEventHandler(object sender, DownloadDataEventArgs e);

        public delegate void DownloadFileEventHandler(object sender, DownloadFileEventArgs e);

        public delegate void ErrorEventHandler(object sender, ErrorEventArgs e);

        public delegate void RequestEventHandler(object sender, RequestEventArgs e);

        public class DownloadDataEventArgs : RequestEventArgs<byte[]>
        {
        }

        public class DownloadDataRequest : Request<DownloadDataEventHandler>
        {
        }

        public class DownloadFileEventArgs : RequestEventArgs<Path>
        {
        }

        public class DownloadFileRequest : Request<DownloadFileEventHandler>
        {
            public Path TargetPath;
        }

        public class ErrorEventArgs : RequestEventArgs
        {
            public Exception Exception;
        }

        public abstract class Request
        {
            public Path Path;
            public ErrorEventHandler RequestError;
            public RequestEventHandler RequestStarted;

            public override string ToString()
            {
                return Path.ToString();
            }
        }

        public abstract class Request<TRequestCompleted> : Request
        {
            public TRequestCompleted RequestCompleted;
        }

        public class RequestEventArgs : EventArgs
        {
            public Request Request;
        }

        public abstract class RequestEventArgs<TResult> : RequestEventArgs
        {
            public TResult Result;
        }
    }
}
