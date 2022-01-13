using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DerekWare.Collections;
using DerekWare.Diagnostics;
using DerekWare.HomeAutomation.Common;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Bridge;
using Q42.HueApi.Models.Groups;
#if ENABLE_STREAMING
using Q42.HueApi.Streaming;
#endif

namespace DerekWare.HomeAutomation.PhilipsHue
{
    public class Client : IClient
    {
        public static readonly Client Instance = new();

        internal readonly SynchronizedDictionary<string, Device> InternalDevices = new();
        internal readonly SynchronizedHashSet<DeviceGroup> InternalGroups = new();

        CancellationTokenSource BridgeDiscoveryCancellationSource;
        Task BridgeDiscoveryTask;
        BridgeLocator BridgeLocater;
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
                InternalGroups.ForEach(i => value.BeginInvoke(this, new DeviceEventArgs { Device = i }, null, null));
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
                    foreach(DeviceGroup group in args.NewItems.SafeEmpty())
                    {
                        Debug.Trace(this, $"Group discovered: {group}");
                        _DeviceDiscovered?.Invoke(this, new DeviceEventArgs { Device = group });
                    }
                }
            };
        }

        public IReadOnlyCollection<IDevice> Devices => InternalDevices.Values.ToList();
        public string Family => "Philips Hue";
        public IReadOnlyCollection<IDeviceGroup> Groups => InternalGroups;
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
            if(BridgeDiscoveryCancellationSource is null)
            {
                return;
            }

            BridgeDiscoveryCancellationSource.Cancel();
            BridgeDiscoveryTask.Wait();
            BridgeDiscoveryCancellationSource.Dispose();
            BridgeDiscoveryCancellationSource = null;
            BridgeDiscoveryTask.Dispose();
            BridgeDiscoveryTask = null;
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

            RefreshDevices();
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
            var lights = await GetLights();
            var groups = await GetGroups();

            InternalDevices.AddRange(lights.Where(i => !InternalDevices.ContainsKey(i.Id)).Select(i => KeyValuePair.Create(i.Id, new Device(i))));
            InternalDevices.AddRange(lights.Where(i => !InternalDevices.ContainsKey(i.Id)).Select(i => KeyValuePair.Create(i.Id, new Device(i))));

#if ENABLE_STREAMING
            InternalGroups.AddRange(groups.Select(i => i.Type == GroupType.Entertainment ? new StreamingGroup(i) : new DeviceGroup(i)));
#else
            InternalGroups.AddRange(groups.Select(i => new DeviceGroup(i)));
#endif
        }

        #region IDisposable

        public void Dispose()
        {
            CancelBridgeDiscovery();
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
