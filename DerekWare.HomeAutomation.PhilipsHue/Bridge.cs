using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using DerekWare.Strings;
using Newtonsoft.Json;
using Q42.HueApi.Models.Bridge;

namespace DerekWare.HomeAutomation.PhilipsHue
{
    public class Bridge
    {
        public BridgeConfig Config;

        public Bridge(LocatedBridge obj)
        {
            BridgeId = obj.BridgeId;
            IpAddress = obj.IpAddress;
        }

        public string BridgeId { get; }
        public string IpAddress { get; }
        public string Name => Config?.Name;

        public override string ToString()
        {
            return Name.IsNullOrEmpty() ? IpAddress : $"{Name} ({IpAddress})";
        }

        async Task<BridgeConfig> GetConfig()
        {
            var request = (HttpWebRequest)WebRequest.Create($"http://{IpAddress}/api/config");
            using var response = (HttpWebResponse)await request.GetResponseAsync();
            using var reader = new StreamReader(response.GetResponseStream());
            var json = await reader.ReadToEndAsync();
            return JsonConvert.DeserializeObject<BridgeConfig>(json);
        }

        public static async Task<Bridge> Create(LocatedBridge obj)
        {
            var bridge = new Bridge(obj);
            bridge.Config = await bridge.GetConfig();
            return bridge;
        }
    }

    public class BridgeConfig
    {
        [JsonProperty("apiversion")]
        public string ApiVersion;

        [JsonProperty("bridgeid")]
        public string BridgeId;

        [JsonProperty("datastoreversion")]
        public string DataStoreVersion;

        [JsonProperty("factorynew")]
        public bool FactoryNew;

        [JsonProperty("mac")]
        public string MacAddress;

        [JsonProperty("modelid")]
        public string ModelId;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("replacesbridgeid")]
        public string ReplacesBridgeId;

        [JsonProperty("starterkitid")]
        public string StarterKitId;

        [JsonProperty("swversion")]
        public string SwVersion;
    }

    public class BridgeEventArgs : EventArgs
    {
        public Bridge Bridge { get; set; }
    }
}
