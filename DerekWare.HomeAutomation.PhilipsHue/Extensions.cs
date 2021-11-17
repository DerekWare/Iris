using System.Collections.Generic;
using System.Threading.Tasks;
using Q42.HueApi;

namespace DerekWare.HomeAutomation.PhilipsHue
{
    public static class Extensions
    {
        public static Task SendCommandAsync(this LightCommand command, IEnumerable<string> lightIds)
        {
            return Client.Instance.HueClient.SendCommandAsync(command, lightIds);
        }
    }
}
