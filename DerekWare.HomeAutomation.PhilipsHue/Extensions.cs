using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DerekWare.Diagnostics;
using Q42.HueApi;
using Q42.HueApi.Models.Groups;

namespace DerekWare.HomeAutomation.PhilipsHue
{
    public static class Extensions
    {
        public static Effect GetEffectType(object effect)
        {
            var hueEffect = Effect.None;

            if(effect is null)
            {
                // Nothing to do
            }
            else if(effect is Effect)
            {
                hueEffect = (Effect)effect;
            }
            else if(effect is string effectName)
            {
                hueEffect = (Effect)Enum.Parse(typeof(Effect), effectName, true);
            }
            else
            {
                Debug.Warning(effect, "Invalid effect settings");
            }

            return hueEffect;
        }

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
