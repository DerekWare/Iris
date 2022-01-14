using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DerekWare.Collections;
using DerekWare.Diagnostics;
using DerekWare.HomeAutomation.Common;
using DerekWare.Threading;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Bridge;
using Q42.HueApi.Models.Groups;
using Task = System.Threading.Tasks.Task;
using Thread = DerekWare.Threading.Thread;
#if ENABLE_STREAMING
using Q42.HueApi.Streaming;
#endif

namespace DerekWare.HomeAutomation.PhilipsHue
{
    public class Client : IClient
    {
        public static readonly Client Instance = new();
        public static readonly TimeSpan DeviceRefreshInterval = TimeSpan.FromSeconds(30);

        internal readonly SynchronizedDictionary<string, Device> InternalDevices = new();
        internal readonly SynchronizedDictionary<string, DeviceGroup> InternalGroups = new();

        CancellationTokenSource BridgeDiscoveryCancellationSource;
        Task BridgeDiscoveryTask;
        BridgeLocator BridgeLocater;
        Thread DeviceRefreshThread;
        ILocalHueClient LocalHueClient;
        string BridgeAddress;

#if ENABLE_STREAMING
        StreamingHueClient StreamingHueClient;
#endif

        public event EventHandler<BridgeEventArgs> BridgeDiscovered;

        public event EventHandler<DeviceEventArgs> DeviceDiscovered
        {
            add
            {
                _DeviceDiscovered += value;
                InternalDevices.ForEach(i => value.BeginInvoke(this, new DeviceEventArgs { Device = i.Value }, null, null));
                InternalGroups.ForEach(i => value.BeginInvoke(this, new DeviceEventArgs { Device = i.Value }, null, null));
            }
            remove => _DeviceDiscovered -= value;
        }

        public event EventHandler<DeviceEventArgs> PropertiesChanged;
        public event EventHandler<DeviceEventArgs> StateChanged;

        event EventHandler<DeviceEventArgs> _DeviceDiscovered;

        Client()
        {
            InternalDevices.CollectionChanged += (sender, args) =>
            {
                if(args.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach(string key in args.NewItems.SafeEmpty())
                    {
                        var device = InternalDevices[key];
                        Debug.Trace(this, $"Device discovered: {device}");
                        _DeviceDiscovered?.Invoke(this, new DeviceEventArgs { Device = device });
                    }
                }
            };

            InternalGroups.CollectionChanged += (sender, args) =>
            {
                if(args.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach(string key in args.NewItems.SafeEmpty())
                    {
                        var group = InternalGroups[key];
                        Debug.Trace(this, $"Group discovered: {group}");
                        _DeviceDiscovered?.Invoke(this, new DeviceEventArgs { Device = group });
                    }
                }
            };
        }

        public IReadOnlyCollection<IDevice> Devices => InternalDevices.Values.ToList<IDevice>();
        public string Family => "Philips Hue";
        public IReadOnlyCollection<IDeviceGroup> Groups => InternalGroups.Values.ToList<IDeviceGroup>();
        public TimeSpan MinMessageInterval => TimeSpan.FromMilliseconds(100);

        internal Task<HueResults> SendLightCommand(LightCommand command, IEnumerable<string> lightIds)
        {
            return LocalHueClient.SendCommandAsync(command, lightIds);
        }

        internal Task<HueResults> SendGroupCommand(LightCommand command, string groupId)
        {
            return LocalHueClient.SendGroupCommandAsync(command, groupId);
        }

        public Task BeginBridgeDiscovery()
        {
            return BeginBridgeDiscovery(TimeSpan.FromSeconds(10));
        }

        public Task BeginBridgeDiscovery(TimeSpan timeout)
        {
            if(BridgeLocater is null)
            {
                BridgeLocater = new HttpBridgeLocator();
                BridgeLocater.BridgeFound += OnBridgeDiscovered;
            }

            CancelBridgeDiscovery();

            BridgeDiscoveryCancellationSource = new CancellationTokenSource();
            BridgeDiscoveryTask = BridgeLocater.LocateBridgesAsync(BridgeDiscoveryCancellationSource.Token);

            return BridgeDiscoveryTask;
        }

        public void CancelBridgeDiscovery()
        {
            BridgeDiscoveryCancellationSource?.Cancel();
            BridgeDiscoveryTask?.Wait();

            DerekWare.Extensions.Dispose(ref BridgeDiscoveryCancellationSource);
            DerekWare.Extensions.Dispose(ref BridgeDiscoveryTask);
        }

        void BeginDeviceRefresh()
        {
            if(DeviceRefreshThread?.IsEnabled == true)
            {
                return;
            }

            DeviceRefreshThread = new Thread { KeepAlive = true, Name = GetType().FullName + ".DeviceRefreshThread", SupportsCancellation = true };
            DeviceRefreshThread.DoWork += DeviceRefreshWorker;
            DeviceRefreshThread.Start();
        }

        void CancelDeviceRefresh()
        {
            DerekWare.Extensions.Dispose(ref DeviceRefreshThread);
        }

        // This API key must be retrieved from the Register method, which involves the
        // user pressing the link button on their Hue Bridge, then calling Register.
        // From then on, we can connect to the bridge using the API key.
        public void Connect(string bridgeAddress, string apiKey, string entertainmentKey)
        {
            if(!string.Equals(BridgeAddress, bridgeAddress))
            {
                InternalDevices.Clear();
                InternalGroups.Clear();
            }

#if ENABLE_STREAMING
            StreamingHueClient = new StreamingHueClient(bridgeAddress, apiKey, entertainmentKey);
            LocalHueClient = StreamingHueClient.LocalHueClient;
#else
            LocalHueClient = new LocalHueClient(bridgeAddress, apiKey);
#endif
            BridgeAddress = bridgeAddress;

            BeginDeviceRefresh();
        }

        void DeviceRefreshWorker(Thread thread, DoWorkEventArgs eventArgs)
        {
            while(!thread.CancellationPending)
            {
                RefreshDevices().Wait();
                thread.CancelEvent.WaitOne(DeviceRefreshInterval);
            }
        }

        internal Task<IReadOnlyCollection<Group>> GetGroups()
        {
            return LocalHueClient.GetGroupsAsync();
        }

        internal Task<Light> GetLightById(string id)
        {
            return LocalHueClient.GetLightAsync(id);
        }

        internal Task<IEnumerable<Light>> GetLights()
        {
            return LocalHueClient.GetLightsAsync();
        }

        internal void OnPropertiesChanged(IDevice device)
        {
            PropertiesChanged?.Invoke(this, new DeviceEventArgs { Device = device });
        }

        internal void OnStateChanged(IDevice device)
        {
            StateChanged?.Invoke(this, new DeviceEventArgs { Device = device });
        }

        async Task RefreshDevices()
        {
            foreach(var light in await GetLights())
            {
                if(InternalDevices.TryGetValue(light.Id, out var device))
                {
                    device.SetState(light);
                }
                else
                {
                    InternalDevices.Add(light.Id, new Device(light));
                }
            }

            foreach(var group in await GetGroups())
            {
                if(InternalGroups.TryGetValue(group.Id, out var device))
                {
                    device.SetState(group);
                }
                else
                {
#if ENABLE_STREAMING
                    InternalGroups.Add(group.Id, group.Type == GroupType.Entertainment ? new StreamingGroup(group) : new DeviceGroup(group)));
#else
                    InternalGroups.Add(group.Id, new DeviceGroup(group));
#endif
                }
            }
        }

        #region IDisposable

        public void Dispose()
        {
            CancelBridgeDiscovery();
            CancelDeviceRefresh();

            LocalHueClient = null;
        }

        #endregion

        #region Event Handlers

        async void OnBridgeDiscovered(IBridgeLocator sender, LocatedBridge other)
        {
            if(BridgeDiscovered is null)
            {
                return;
            }

            var bridge = await Bridge.Create(other);
            BridgeDiscovered.Invoke(this, new BridgeEventArgs { Bridge = bridge });
        }

        #endregion

        public static Task<RegisterEntertainmentResult> Register(string bridgeAddress)
        {
#if ENABLE_STREAMING
            var generateClientKey = true;
#else
            var generateClientKey = false;
#endif
            return new LocalHueClient(bridgeAddress).RegisterAsync("DerekWare", Environment.MachineName, generateClientKey);
        }
    }
}
