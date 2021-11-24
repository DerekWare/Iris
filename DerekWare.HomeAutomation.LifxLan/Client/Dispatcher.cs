using System;
using System.Net;
using System.Threading.Tasks;
using DerekWare.Collections;
using DerekWare.Diagnostics;
using DerekWare.HomeAutomation.Lifx.Lan.Messages;

namespace DerekWare.HomeAutomation.Lifx.Lan
{
    // Register a request to be sent and the delegate will be called when the response is received.
    // You may additionally wait on the returned response task, which will be signaled when the
    // response is received.
    // TODO purge stale messages building up in PendingResponses.
    class Dispatcher
    {
        public delegate void ResponseHandler(Response response);

        public delegate void ResponseHandler<in TResponse>(TResponse response)
            where TResponse : Response;

        public static readonly Dispatcher Instance = new();

        readonly SynchronizedDictionary<ResponseKey, ResponseValue> PendingResponses = new();
        readonly TimeSpan ResponseTimeout = TimeSpan.FromSeconds(2);

        Dispatcher()
        {
        }

        public void OnMessageReceived(IPEndPoint endpoint, Message message)
        {
            var responseKey = new ResponseKey(message);

            lock(PendingResponses.SyncRoot)
            {
                // Get the response handler from the map
                if(!PendingResponses.TryGetValue(responseKey, out var responseValue))
                {
                    Debug.Warning(this, $"Unexpected message: {responseKey}");
                    return;
                }

                // Validate the message type
                if(responseValue.Response.MessageType != message.MessageType)
                {
                    Debug.Warning(this, $"Unexpected response message type: {message.MessageType}");
                    return;
                }

                // Attempt to parse the message. A return value of false means more responses
                // are expected.
                responseValue.Response.Messages.Add(message);

                if(!responseValue.Response.Parse())
                {
                    return;
                }

                // Response is complete
                PendingResponses.Remove(responseKey);

                // Call the handler
                responseValue.Handler(responseValue.Response);
            }
        }

        // Sends a request that requires no response
        public Task SendRequest(string ipAddress, Request request)
        {
            Debug.Trace(this, $"Sending {ipAddress}: {request.GetType().Name}");

            request.AcknowledgementRequired = false;
            request.ResponseRequired = false;

            return Client.Instance.SendMessage(ipAddress, request.SerializeBinary());
        }

        // Sends a request that requires a response
        public Task<TResponse> SendRequest<TResponse>(string ipAddress, Request request, ResponseHandler<TResponse> responseHandler)
            where TResponse : Response, new()
        {
            Debug.Trace(this, $"Sending {ipAddress}: {request.GetType().Name}");

            var responseType = typeof(TResponse);

            if(responseType == typeof(Acknowledgement))
            {
                request.AcknowledgementRequired = false;
                request.ResponseRequired = false;
            }
            else
            {
                request.AcknowledgementRequired = false;
                request.ResponseRequired = true;
            }

            // Create the response key/value pair. We use a lambda response handler
            // to maintain the templated response type.
            var response = new TResponse();
            var responseKey = new ResponseKey(request);
            var taskCompletionSource = new TaskCompletionSource<TResponse>();
            var responseValue = new ResponseValue(response,
                                                  r =>
                                                  {
                                                      Debug.Assert(ReferenceEquals(r, response));

                                                      // Signal task completion
                                                      taskCompletionSource.SetResult(response);

                                                      // Call the associated handler
                                                      responseHandler?.Invoke(response);
                                                  });

            // Add the response to our pending list
            PendingResponses.Add(responseKey, responseValue);

            // Send the request
            Client.Instance.SendMessage(ipAddress, request.SerializeBinary());

            return taskCompletionSource.Task;
        }

        class ResponseKey : IEquatable<ResponseKey>
        {
            public readonly byte Sequence;
            public readonly uint Source;

            public ResponseKey(Message msg)
            {
                Sequence = msg.Sequence;
                Source = msg.Source;
            }

            public override string ToString()
            {
                return $"{{ Sequence:{Sequence}, Source:{Source} }}";
            }

            #region Equality

            public bool Equals(ResponseKey other)
            {
                if(ReferenceEquals(null, other))
                {
                    return false;
                }

                if(ReferenceEquals(this, other))
                {
                    return true;
                }

                return (Sequence == other.Sequence) && (Source == other.Source);
            }

            public override bool Equals(object obj)
            {
                if(ReferenceEquals(null, obj))
                {
                    return false;
                }

                if(ReferenceEquals(this, obj))
                {
                    return true;
                }

                if(obj.GetType() != GetType())
                {
                    return false;
                }

                return Equals((ResponseKey)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Sequence.GetHashCode() * 397) ^ (int)Source;
                }
            }

            public static bool operator ==(ResponseKey left, ResponseKey right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(ResponseKey left, ResponseKey right)
            {
                return !Equals(left, right);
            }

            #endregion
        }

        class ResponseValue
        {
            public readonly DateTime CreationTime = DateTime.Now;
            public readonly ResponseHandler Handler;
            public readonly Response Response;

            public ResponseValue(Response response, ResponseHandler handler)
            {
                Response = response;
                Handler = handler;
            }
        }
    }
}
