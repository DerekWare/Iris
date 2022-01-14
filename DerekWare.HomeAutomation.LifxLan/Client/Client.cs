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
using DerekWare.Threading;
using Device = DerekWare.HomeAutomation.Lifx.Lan.Devices.Device;
using DeviceGroup = DerekWare.HomeAutomation.Lifx.Lan.Devices.DeviceGroup;
using Task = System.Threading.Tasks.Task;
using Thread = DerekWare.Threading.Thread;

namespace DerekWare.HomeAutomation.Lifx.Lan
{
    public partial class Client : IClient
    {
        public const string BroadcastAddress = "255.255.255.255";
        public const int Port = 56700;

        public static readonly Client Instance = new();
        public static readonly TimeSpan ServiceInterval = TimeSpan.FromSeconds(30);
        readonly ObservableDictionary<string, Device> InternalDevices = new(StringComparer.OrdinalIgnoreCase);
        readonly ObservableDictionary<string, DeviceGroup> InternalGroups = new(StringComparer.OrdinalIgnoreCase);
        readonly ObservableDictionary<string, Device> PendingDevices = new(StringComparer.OrdinalIgnoreCase);
        readonly ObservableDictionary<string, DeviceGroup> PendingGroups = new(StringComparer.OrdinalIgnoreCase);

        readonly object SyncRoot = new();

        Thread ReceiveThread;
        Thread ServiceThread;
        UdpClient Socket;

        public event EventHandler<DeviceEventArgs> PropertiesChanged;
        public event EventHandler<DeviceEventArgs> StateChanged;

        Client()
        {
            InternalDevices.CollectionChanged += (sender, args) =>
            {
                if(args.Action == NotifyCollectionChangedAction.Add)
                {
                    lock(SyncRoot)
                    {
                        foreach(string i in args.NewItems.SafeEmpty())
                        {
                            OnDeviceDiscovered(InternalDevices[i]);
                        }
                    }
                }
            };

            InternalGroups.CollectionChanged += (sender, args) =>
            {
                if(args.Action == NotifyCollectionChangedAction.Add)
                {
                    lock(SyncRoot)
                    {
                        foreach(string i in args.NewItems.SafeEmpty())
                        {
                            OnDeviceDiscovered(InternalGroups[i]);
                        }
                    }
                }
            };
        }

        public IReadOnlyCollection<IDevice> Devices
        {
            get
            {
                lock(SyncRoot)
                {
                    return InternalDevices.Values.ToList<IDevice>();
                }
            }
        }

        public string Family => "LIFX";

        public IReadOnlyCollection<IDeviceGroup> Groups
        {
            get
            {
                lock(SyncRoot)
                {
                    return InternalGroups.Values.ToList<IDeviceGroup>();
                }
            }
        }

        public TimeSpan MinMessageInterval => TimeSpan.FromMilliseconds(50);

        public void Connect(string ipAddress)
        {
            CreateDevice(ipAddress, out var device);
        }

        public void Start()
        {
            Socket = new UdpClient(new IPEndPoint(IPAddress.Any, Port)) { Client = { Blocking = false, EnableBroadcast = true } };

            ReceiveThread = new Thread
            {
                KeepAlive = true, Name = GetType().FullName + ".ReceiveThread", Priority = ThreadPriority.AboveNormal, SupportsCancellation = true
            };

            ServiceThread = new Thread
            {
                KeepAlive = true, Name = GetType().FullName + ".ServiceThread", Priority = ThreadPriority.BelowNormal, SupportsCancellation = true
            };

            ReceiveThread.DoWork += ReceiveWorker;
            ServiceThread.DoWork += ServiceWorker;

            ReceiveThread.Start();
            ServiceThread.Start();
        }

        internal bool CreateDevice(string ipAddress, out Device device)
        {
            lock(SyncRoot)
            {
                if(InternalDevices.TryGetValue(ipAddress, out device) || PendingDevices.TryGetValue(ipAddress, out device))
                {
                    return false;
                }

                device = new Device(ipAddress);

                if(device.IsValid)
                {
                    InternalDevices.Add(device.IpAddress, device);
                }
                else
                {
                    PendingDevices.Add(device.IpAddress, device);
                }
            }

            return true;
        }

        internal bool CreateGroup(GroupResponse response, out DeviceGroup group)
        {
            lock(SyncRoot)
            {
                if(InternalGroups.TryGetValue(response.Uuid, out group) || PendingGroups.TryGetValue(response.Uuid, out group))
                {
                    return false;
                }

                group = new DeviceGroup(response);

                if(group.IsValid)
                {
                    InternalGroups.Add(group.Uuid, group);
                }
                else
                {
                    PendingGroups.Add(group.Uuid, group);
                }
            }

            return true;
        }

        internal void OnPropertiesChanged(IDevice idevice)
        {
            if(!idevice.IsValid)
            {
                return;
            }

            lock(SyncRoot)
            {
                if(idevice is DeviceGroup group)
                {
                    if(PendingGroups.Remove(group.Uuid))
                    {
                        InternalGroups.Add(group.Uuid, group);
                    }
                }
                else if(idevice is Device device)
                {
                    if(PendingDevices.Remove(device.IpAddress))
                    {
                        InternalDevices.Add(device.IpAddress, device);
                    }
                }
            }

            PropertiesChanged?.Invoke(this, new DeviceEventArgs { Device = idevice });
        }

        internal void OnStateChanged(IDevice device)
        {
            if(!device.IsValid)
            {
                return;
            }

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

        #region IDisposable

        public void Dispose()
        {
            ReceiveThread.Stop(false);
            ServiceThread.Stop(false);

            Extensions.Dispose(ref Socket);
            Extensions.Dispose(ref ReceiveThread);
            Extensions.Dispose(ref ServiceThread);
        }

        #endregion

        #region Event Handlers

        void ReceiveWorker(Thread thread, DoWorkEventArgs eventArgs)
        {
            while(!thread.CancellationPending)
            {
                Task<UdpReceiveResult> receiveTask;

                try
                {
                    receiveTask = Socket.ReceiveAsync();
                    receiveTask.Wait();
                }
                catch
                {
                    return;
                }

                Message msg;

                try
                {
                    msg = Message.DeserializeBinary(receiveTask.Result.Buffer);
                }
                catch(Exception ex)
                {
                    Debug.Error(this, ex);
                    receiveTask.Dispose();
                    continue;
                }

                if(ServiceResponse.MessageType == msg.MessageType)
                {
                    Debug.Trace(this, $"Received {receiveTask.Result.RemoteEndPoint.Address}: {msg}");
                    CreateDevice(receiveTask.Result.RemoteEndPoint.Address.ToString(), out var device);
                }
                else
                {
                    Dispatcher.Instance.OnMessageReceived(receiveTask.Result.RemoteEndPoint, msg);
                }

                receiveTask.Dispose();
            }
        }

        void ServiceWorker(Thread thread, DoWorkEventArgs eventArgs)
        {
            while(!thread.CancellationPending)
            {
                SendMessage(null, new GetServiceRequest().SerializeBinary());
                thread.CancelEvent.WaitOne(ServiceInterval);
            }
        }

        #endregion
    }
}
