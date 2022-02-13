#if ENABLE_STREAMING
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DerekWare.HomeAutomation.Common.Colors;
using Q42.HueApi.ColorConverters.HSB;
using Q42.HueApi.Models.Groups;
using Q42.HueApi.Streaming.Extensions;
using Q42.HueApi.Streaming.Models;

namespace DerekWare.HomeAutomation.PhilipsHue
{
    class StreamingGroup : DeviceGroup
    {
        static readonly object ConnectLock = new();

        internal readonly Q42.HueApi.Streaming.Models.StreamingGroup HueStreamingGroup;

        bool _IsValid;
        EntertainmentLayer EntertainmentLayer;

        internal StreamingGroup(Group hueGroup)
            : base(hueGroup)
        {
            HueStreamingGroup = new Q42.HueApi.Streaming.Models.StreamingGroup(hueGroup.Locations);

            if(hueGroup.Name == "Turkish Entertainment 1")
            {
                Task.Run(Connect);
            }
        }

        public override bool IsValid => _IsValid && base.IsValid;

        protected override void ApplyColor(IReadOnlyCollection<Color> colors, TimeSpan transitionDuration)
        {
            var i = 0;

            foreach(var color in colors)
            {
                int? h, s, m;
                byte? b;

                color.ToHueColor(out h, out s, out b, out m);
                var rgb = new HSB(h ?? 0, s ?? 0, b ?? 0).GetRGB();

                EntertainmentLayer[i++].SetColor(CancellationToken.None, rgb, transitionDuration);
            }

            PhilipsHue.Client.Instance.StreamingHueClient.ManualUpdate(HueStreamingGroup);
        }

        void Connect()
        {
            Connect(HueGroup.Id);

            EntertainmentLayer = HueStreamingGroup.GetNewLayer(true);
            _IsValid = true;
        }

        static void Connect(string id)
        {
            lock(ConnectLock)
            {
                var task = PhilipsHue.Client.Instance.StreamingHueClient.Connect(id);
                task.Wait();

                if(task.Exception is not null)
                {
                    throw task.Exception;
                }
            }
        }
    }
}

#endif
