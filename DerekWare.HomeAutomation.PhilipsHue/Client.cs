using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DerekWare.Collections;
using DerekWare.Diagnostics;
using DerekWare.HomeAutomation.Common;
using Q42.HueApi;
using Q42.HueApi.Models.Groups;

namespace DerekWare.HomeAutomation.PhilipsHue
{
    public class BridgeEventArgs : EventArgs
    {
        public string BridgeId { get; set; }
        public string IpAddress { get; set; }
    }

    public class Client : IClient
    {
        public static readonly Client Instance = new();

        internal readonly SynchronizedDictionary<string, Device> InternalDevices = new();
        internal readonly SynchronizedHashSet<DeviceGroup> InternalGroups = new();

        CancellationTokenSource BridgeDiscoveryCancellationSource;
        Task BridgeDiscoveryTask;
        BridgeLocator BridgeLocater;

        public event EventHandler<BridgeEventArgs> BridgeDiscovered;

        public event EventHandler<DeviceEventArgs> DeviceDiscovered
        {
            add
            {
                _DeviceDiscovered += value;

                foreach(var device in InternalDevices)
                {
                    value.BeginInvoke(this, new DeviceEventArgs { Device = device.Value }, null, null);
                }
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
                foreach(string key in args.NewItems)
                {
                    var device = InternalDevices[key];
                    Debug.Trace(this, $"Device discovered: {device}");
                    _DeviceDiscovered?.Invoke(this, new DeviceEventArgs { Device = device });
                }
            };

            InternalGroups.CollectionChanged += (sender, args) =>
            {
                foreach(DeviceGroup group in args.NewItems)
                {
                    Debug.Trace(this, $"Group discovered: {group}");
                    _DeviceDiscovered?.Invoke(this, new DeviceEventArgs { Device = group });
                }
            };
        }

        public IReadOnlyCollection<IDevice> Devices => InternalDevices.Values.ToList();
        public string Family => "Philips Hue";
        public IReadOnlyCollection<IDeviceGroup> Groups => InternalGroups;

        internal LocalHueClient HueClient { get; private set; }

        public Task BeginBridgeDiscovery()
        {
            return BeginBridgeDiscovery(TimeSpan.FromSeconds(10));
        }

        public Task BeginBridgeDiscovery(TimeSpan timeout)
        {
            if(BridgeLocater is null)
            {
                BridgeLocater = new HttpBridgeLocator();
                BridgeLocater.BridgeFound += (sender, bridge) =>
                {
                    BridgeDiscovered?.Invoke(this, new BridgeEventArgs { BridgeId = bridge.BridgeId, IpAddress = bridge.IpAddress });
                };
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
        public void Connect(string bridgeAddress, string apiKey)
        {
            HueClient = new LocalHueClient(bridgeAddress);
            HueClient.Initialize(apiKey);

            RefreshDevices();
        }

        internal Task<IReadOnlyCollection<Group>> GetGroups()
        {
            return HueClient.GetGroupsAsync();
        }

        internal Task<Light> GetLightById(string id)
        {
            return HueClient.GetLightAsync(id);
        }

        internal Task<IEnumerable<Light>> GetLights()
        {
            return HueClient.GetLightsAsync();
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

            InternalDevices.AddRange(lights.Select(i => new KeyValuePair<string, Device>(i.Id, new Device(i))));
            InternalGroups.AddRange(groups.Select(i => new DeviceGroup(i)));
        }

        #region IDisposable

        public void Dispose()
        {
            CancelBridgeDiscovery();
            HueClient = null;
        }

        #endregion

        public static Task<string> Register(string bridgeAddress)
        {
            return new LocalHueClient(bridgeAddress).RegisterAsync("DerekWare", "PC");
        }
    }
}
