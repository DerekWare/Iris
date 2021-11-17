using System;
using System.Collections.Generic;
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
        public delegate bool ResponseHandler<in TResponse>(TResponse message)
            where TResponse : Response;

        public static readonly Dispatcher Instance = new();

        readonly SynchronizedDictionary<ResponseKey, ResponseValue> PendingResponses = new();
        readonly TimeSpan ResponseTimeout = TimeSpan.FromSeconds(2);

        Dispatcher()
        {
        }

        public void OnMessageReceived(IPEndPoint endpoint, Message message)
        {
            // TODO there's a possibility that a second response could be received
            // while the lock is released and the response handler has been removed
            // from the list.
            Debug.Trace(null, $"Received {endpoint.Address}: {message}");

            var responseKey = new ResponseKey(message);
            ResponseValue responseValue;

            // Get the response handler from the map
            lock(PendingResponses.SyncRoot)
            {
                if(!PendingResponses.TryGetValue(responseKey, out responseValue))
                {
                    Debug.Warning(null, $"Unexpected message: {responseKey}");
                    return;
                }

                responseValue.Responses.Add(message);
                PendingResponses.Remove(responseKey);
            }

            // Call the handler. A return value of true means more responses are expected.
            if(!responseValue.OnResponseReceived(responseValue.Responses))
            {
                return;
            }

            // Push the response handler back into the map for more messages
            PendingResponses.Add(responseKey, responseValue);
        }

        public Task SendRequest(string ipAddress, Request request)
        {
            Debug.Trace(null, $"Sending {ipAddress}: {request.GetType().Name}");

            if(request.AcknowledgementRequired)
            {
                Debug.Error(null, "AcknowledgementRequired with no response type supplied");
            }

            if(request.ResponseRequired)
            {
                Debug.Error(null, "ResponseRequired with no response type supplied");
            }

            return Client.Instance.SendMessage(ipAddress, request.SerializeBinary());
        }

        public Task<TResponse> SendRequest<TResponse>(string ipAddress, Request request, ResponseHandler<TResponse> responseHandler = null)
            where TResponse : Response, new()
        {
            Debug.Trace(null, $"Sending {ipAddress}: {request.GetType().Name}");

            if(request.AcknowledgementRequired)
            {
                if(request.ResponseRequired)
                {
                    Debug.Error(null, "AcknowledgementRequired and ResponseRequired are mutually exclusive");
                }

                if(typeof(Acknowledgement) != typeof(TResponse))
                {
                    Debug.Error(null, "Response type must be AcknowledgementResponse");
                }
            }
            else
            {
                if(typeof(TResponse) == typeof(Acknowledgement))
                {
                    // Debug.Warning(null, "AcknowledgementRequired not set");
                    request.AcknowledgementRequired = true;
                }
                else if(!request.ResponseRequired)
                {
                    // Debug.Warning(null, "ResponseRequired not set");
                    request.ResponseRequired = true;
                }
            }

            var responseKey = new ResponseKey(request);
            var responseValue = new ResponseValue();
            var taskCompletionSource = new TaskCompletionSource<TResponse>();

            responseValue.OnResponseReceived = messages =>
            {
                // Validate the response type
                if(Response.Create(messages[0].MessageType) is not TResponse response)
                {
                    // TODO gracefully handle the case where we get an unexpected response
                    // because of an old request still lingering somewhere.
                    Debug.Error(null, $"Unexpected response: {messages[0].MessageType}");
                    return false;
                }

                // Set the message list. Most responses only have one message, but
                // some require multiple messages due to UDP packet size limitations.
                response.Messages = messages;

                // Call the response handler. If it returns false, no further message
                // processing is required.
                if(responseHandler?.Invoke(response) ?? false)
                {
                    // More processing required
                    return true;
                }

                // Signal task completion
                taskCompletionSource.SetResult(response);
                return false;
            };

            PendingResponses.Add(responseKey, responseValue);
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
            public readonly List<Message> Responses = new();
            public Func<List<Message>, bool> OnResponseReceived;
        }
    }
}
