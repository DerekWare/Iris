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
using DerekWare.HomeAutomation.Lifx.Lan.Devices;
using DerekWare.HomeAutomation.Lifx.Lan.Messages;
using DeviceGroup = DerekWare.HomeAutomation.Lifx.Lan.Devices.DeviceGroup;

namespace DerekWare.HomeAutomation.Lifx.Lan
{
    public partial class Client : IClient
    {
        public const string BroadcastAddress = "255.255.255.255";
        public const int Port = 56700;

        public static readonly Client Instance = new();

        internal SynchronizedDictionary<string, Device> InternalDevices = new(StringComparer.OrdinalIgnoreCase);
        internal SynchronizedDictionary<string, DeviceGroup> InternalGroups = new(StringComparer.OrdinalIgnoreCase);

        readonly TimeSpan GetServiceInterval = TimeSpan.FromSeconds(30);
        readonly CancellationTokenSource GetServiceTaskCancellationTokenSource = new();
        readonly TimeSpan NetworkThrottle = TimeSpan.FromSeconds(1.0 / 20);
        readonly CancellationTokenSource ReceiveTaskCancellationTokenSource = new();

        Task GetServiceTask;
        Task ReceiveTask;
        UdpClient Socket;

        public event EventHandler<DeviceEventArgs> PropertiesChanged;
        public event EventHandler<DeviceEventArgs> StateChanged;

        Client()
        {
            InternalDevices.CollectionChanged += (sender, args) =>
            {
                if(args.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach(string i in args.NewItems)
                    {
                        OnDeviceDiscovered(InternalDevices[i]);
                    }
                }
            };

            InternalGroups.CollectionChanged += (sender, args) =>
            {
                if(args.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach(string i in args.NewItems)
                    {
                        OnDeviceDiscovered(InternalGroups[i]);
                    }
                }
            };
        }

        public IReadOnlyCollection<IDevice> Devices => InternalDevices.Values.ToList<IDevice>();
        public string Family => "LIFX";
        public IReadOnlyCollection<IDeviceGroup> Groups => InternalGroups.Values.ToList<IDeviceGroup>();

        public void Connect(string ipAddress)
        {
            CreateDevice(ipAddress, null, out var device);
        }

        public void Start()
        {
            Socket = new UdpClient(new IPEndPoint(IPAddress.Any, Port)) { Client = { Blocking = false, EnableBroadcast = true } };
            ReceiveTask = Task.Run(ReceiveLoop, ReceiveTaskCancellationTokenSource.Token);
            GetServiceTask = Task.Run(GetServiceLoop, GetServiceTaskCancellationTokenSource.Token);
        }

        internal bool CreateDevice(string ipAddress, StateService response, out Device device)
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

        internal bool CreateGroup(StateGroup response, out DeviceGroup group)
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

            // Debug.Trace(null, $"Sending {address}: {data.FormatByteArray()}");
            try
            {
                return Socket.SendAsync(data, data.Length, ipAddress, Port);
            }
            catch(Exception e)
            {
                Debug.Warning(this, e);
                return Task.FromException(e);
            }

            // TODO
#if false
            // Throttle the number of messages we send to each device so as to
            // not overrun their UDP receive buffer. The docs recommend no more
            // than 20 messages per second.
            if(device is null)
            {
                return;
            }

            var now = DateTime.Now;
            var wait = NetworkThrottle - (now - device.LastMessageTime);
            device.LastMessageTime = now;

            if(wait > TimeSpan.Zero)
            {
                await Task.Delay(wait);
            }
#endif
        }

        void GetServiceLoop()
        {
            var request = new GetServiceRequest().SerializeBinary();

            while(!GetServiceTaskCancellationTokenSource.IsCancellationRequested)
            {
                SendMessage(null, request);
                GetServiceTaskCancellationTokenSource.Token.WaitHandle.WaitOne(GetServiceInterval);
            }
        }

        void ReceiveLoop()
        {
            while(!ReceiveTaskCancellationTokenSource.IsCancellationRequested)
            {
                var receiveTask = Socket.ReceiveAsync();

                try
                {
                    receiveTask.Wait(ReceiveTaskCancellationTokenSource.Token);
                }
                catch(OperationCanceledException)
                {
                    return;
                }

                if(receiveTask.Result.Buffer.Length <= 0)
                {
                    continue;
                }

                // Debug.Trace(null, $"Received {receiveTask.Result.RemoteEndPoint.Address}: {receiveTask.Result.Buffer.FormatByteArray()}");

                var message = Message.DeserializeBinary(receiveTask.Result.Buffer);

                if(StateService.MessageType == message.MessageType)
                {
                    CreateDevice(receiveTask.Result.RemoteEndPoint.Address.ToString(),
                                 new StateService { Messages = new[] { message }.ToList() },
                                 out var device);
                }
                else
                {
                    Dispatcher.Instance.OnMessageReceived(receiveTask.Result.RemoteEndPoint, message);
                }
            }
        }

        #region IDisposable

        public void Dispose()
        {
            GetServiceTaskCancellationTokenSource.Cancel(false);
            GetServiceTask.Wait();

            ReceiveTaskCancellationTokenSource.Cancel(false);
            ReceiveTask.Wait();

            Socket.Dispose();
        }

        #endregion
    }
}
