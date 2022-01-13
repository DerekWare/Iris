using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using DerekWare.Collections;
using DerekWare.Diagnostics;
using DerekWare.HomeAutomation.Common;
using DerekWare.HomeAutomation.Lifx.Lan.Messages;
using Device = DerekWare.HomeAutomation.Lifx.Lan.Devices.Device;
using DeviceGroup = DerekWare.HomeAutomation.Lifx.Lan.Devices.DeviceGroup;

namespace DerekWare.HomeAutomation.Lifx.Lan
{
    public partial class Client : IClient
    {
        public const string BroadcastAddress = "255.255.255.255";
        public const int Port = 56700;
        public static readonly Client Instance = new();
        public static readonly TimeSpan ServiceInterval = TimeSpan.FromSeconds(30);

        internal SynchronizedDictionary<string, Device> InternalDevices = new(StringComparer.OrdinalIgnoreCase);
        internal SynchronizedDictionary<string, DeviceGroup> InternalGroups = new(StringComparer.OrdinalIgnoreCase);

        Task ReceiveTask;
        CancellationTokenSource ReceiveTaskCancellationTokenSource = new();
        Task ServiceTask;
        CancellationTokenSource ServiceTaskCancellationTokenSource = new();
        UdpClient Socket;

        public event EventHandler<DeviceEventArgs> PropertiesChanged;
        public event EventHandler<DeviceEventArgs> StateChanged;

        Client()
        {
            InternalDevices.CollectionChanged += (sender, args) =>
            {
                if(args.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach(string i in args.NewItems.SafeEmpty())
                    {
                        OnDeviceDiscovered(InternalDevices[i]);
                    }
                }
            };

            InternalGroups.CollectionChanged += (sender, args) =>
            {
                if(args.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach(string i in args.NewItems.SafeEmpty())
                    {
                        OnDeviceDiscovered(InternalGroups[i]);
                    }
                }
            };
        }

        public IReadOnlyCollection<IDevice> Devices => InternalDevices.Values.ToList<IDevice>();
        public string Family => "LIFX";
        public IReadOnlyCollection<IDeviceGroup> Groups => InternalGroups.Values.ToList<IDeviceGroup>();
        public TimeSpan MinMessageInterval => TimeSpan.FromMilliseconds(50);

        public void Connect(string ipAddress)
        {
            CreateDevice(ipAddress, null, out var device);
        }

        public void Start()
        {
            Socket = new UdpClient(new IPEndPoint(IPAddress.Any, Port)) { Client = { Blocking = false, EnableBroadcast = true } };
            ReceiveTask = Task.Run(ReceiveWorker, ReceiveTaskCancellationTokenSource.Token);
            ServiceTask = Task.Run(ServiceWorker, ServiceTaskCancellationTokenSource.Token);
        }

        internal bool CreateDevice(string ipAddress, ServiceResponse response, out Device device)
        {
            lock(InternalDevices.SyncRoot)
            {
                if(InternalDevices.TryGetValue(ipAddress, out device))
                {
                    return false;
                }

                device = new Device(ipAddress, response);
                InternalDevices.Add(device.IpAddress, device);
            }

            return true;
        }

        internal bool CreateGroup(GroupResponse response, out DeviceGroup group)
        {
            lock(InternalGroups.SyncRoot)
            {
                if(InternalGroups.TryGetValue(response.Uuid, out group))
                {
                    return false;
                }

                group = new DeviceGroup(response);
                InternalGroups.Add(group.Uuid, group);
            }

            return true;
        }

        internal void OnPropertiesChanged(IDevice device)
        {
            PropertiesChanged?.Invoke(this, new DeviceEventArgs { Device = device });
        }

        internal void OnStateChanged(IDevice device)
        {
            StateChanged?.Invoke(this, new DeviceEventArgs { Device = device });
        }

        internal Task SendMessage(string ipAddress, byte[] data)
        {
            ipAddress ??= BroadcastAddress;

            try
            {
                return Socket.SendAsync(data, data.Length, ipAddress, Port);
            }
            catch(SocketException e)
            {
                Debug.Error(this, e);
                return Task.FromException(e);
            }
        }

        void ReceiveWorker()
        {
            while(!ReceiveTaskCancellationTokenSource.IsCancellationRequested)
            {
                var receiveTask = Socket.ReceiveAsync();
                Message message;

                try
                {
                    receiveTask.Wait(ReceiveTaskCancellationTokenSource.Token);
                }
                catch(OperationCanceledException)
                {
                    return;
                }
                catch(Exception ex)
                {
                    Debug.Error(this, ex);
                    return;
                }

                try
                {
                    message = Message.DeserializeBinary(receiveTask.Result.Buffer);
                }
                catch(Exception ex)
                {
                    Debug.Error(this, ex);
                    continue;
                }

                if(ServiceResponse.MessageType != message.MessageType)
                {
                    Dispatcher.Instance.OnMessageReceived(receiveTask.Result.RemoteEndPoint, message);
                    continue;
                }

                Debug.Trace(this, $"Received {receiveTask.Result.RemoteEndPoint.Address}: {message}");
                var response = new ServiceResponse();
                response.Messages.Add(message);
                CreateDevice(receiveTask.Result.RemoteEndPoint.Address.ToString(), response, out var device);
            }
        }

        void ServiceWorker()
        {
            var request = new GetServiceRequest().SerializeBinary();

            while(!ServiceTaskCancellationTokenSource.IsCancellationRequested)
            {
                SendMessage(null, request);
                ServiceTaskCancellationTokenSource.Token.WaitHandle.WaitOne(ServiceInterval);
            }
        }

        #region IDisposable

        public void Dispose()
        {
            ServiceTaskCancellationTokenSource?.Cancel(false);
            ReceiveTaskCancellationTokenSource?.Cancel(false);
            ServiceTask?.Wait();
            ReceiveTask?.Wait();

            ServiceTaskCancellationTokenSource = null;
            ReceiveTaskCancellationTokenSource = null;
            ServiceTask = null;
            ReceiveTask = null;

            Extensions.Dispose(ref Socket);
        }

        #endregion
    }
}
