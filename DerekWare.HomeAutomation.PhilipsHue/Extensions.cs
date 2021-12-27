using System.Collections.Generic;
using System.Threading.Tasks;
using Q42.HueApi;
using Q42.HueApi.Models.Groups;

namespace DerekWare.HomeAutomation.PhilipsHue
{
    public static class Extensions
    {
        public static Task<HueResults> SendCommand(this LightCommand command, Light light)
        {
            return Client.Instance.HueClient.SendCommandAsync(command, new[] { light.Id });
        }

        public static Task<HueResults> SendCommand(this LightCommand command, Group group)
        {
            return Client.Instance.HueClient.SendGroupCommandAsync(command, group.Id);
        }

        public static Task<HueResults> SendGroupCommand(this LightCommand command, string groupId)
        {
            return Client.Instance.HueClient.SendGroupCommandAsync(command, groupId);
        }

        public static Task<HueResults> SendLightCommand(this LightCommand command, string lightId)
        {
            return Client.Instance.HueClient.SendCommandAsync(command, new[] { lightId });
        }

        public static Task<HueResults> SendLightCommand(this LightCommand command, IEnumerable<string> lightIds)
        {
            return Client.Instance.HueClient.SendCommandAsync(command, lightIds);
        }
    }
}
