using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DerekWare.HomeAutomation.Common;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.HomeAutomation.Lifx.Lan.Messages;

namespace DerekWare.HomeAutomation.Lifx.Lan.Devices
{
    // DeviceController abstracts the raw messaging between the application and the LIFX device
    class DeviceController
    {
        public DeviceController(string ipAddress)
        {
            IpAddress = ipAddress;
        }

        public string IpAddress { get; }

        public Task<LightState> GetColor(Action<LightState> responseHandler)
        {
            return GetProperty<GetColorRequest, LightState>(responseHandler);
        }

        public Task<StateMultiZone> GetColorZones(Action<StateMultiZone> responseHandler)
        {
            // The response to this request will be N messages where N is ZoneCount / 8. We
            // have to process the first message to get the zone count, then tell the dispatcher
            // whether to continue processing responses.
            var taskCompletionSource = new TaskCompletionSource<StateMultiZone>();
            var task = taskCompletionSource.Task;

            Dispatcher.Instance.SendRequest<StateMultiZone>(IpAddress,
                                                            new GetColorZonesRequest(),
                                                            response =>
                                                            {
                                                                if(response.Messages.Count < ((response.ZoneCount + 7) / 8))
                                                                {
                                                                    // More responses needed
                                                                    return true;
                                                                }

                                                                // Caught them all
                                                                taskCompletionSource.SetResult(response);
                                                                responseHandler(response);
                                                                return false;
                                                            });

            return task;
        }

        public Task<StateExtendedColorZones> GetExtendedColorZones(Action<StateExtendedColorZones> responseHandler)
        {
            return GetProperty<GetExtendedColorZonesRequest, StateExtendedColorZones>(responseHandler);
        }

        public Task<StateGroup> GetGroup(Action<StateGroup> responseHandler)
        {
            return GetProperty<GetGroupRequest, StateGroup>(responseHandler);
        }

        public Task<StateLabel> GetLabel(Action<StateLabel> responseHandler)
        {
            return GetProperty<GetLabelRequest, StateLabel>(responseHandler);
        }

        public Task<StateLocation> GetLocation(Action<StateLocation> responseHandler)
        {
            return GetProperty<GetLocationRequest, StateLocation>(responseHandler);
        }

        public Task<StateMultiZoneEffect> GetMultiZoneEffect(Action<StateMultiZoneEffect> responseHandler)
        {
            return GetProperty<GetMultiZoneEffectRequest, StateMultiZoneEffect>(responseHandler);
        }

        public Task<StatePower> GetPower(Action<StatePower> responseHandler)
        {
            return GetProperty<GetPowerRequest, StatePower>(responseHandler);
        }

        public Task<StateVersion> GetVersion(Action<StateVersion> responseHandler)
        {
            return GetProperty<GetVersionRequest, StateVersion>(responseHandler);
        }

        public Task SetColor(Color color, TimeSpan duration)
        {
            return Dispatcher.Instance.SendRequest(IpAddress, new SetColorRequest { Color = color, Duration = duration });
        }

        public async Task SetColorZones(IReadOnlyCollection<ColorZone> colors, TimeSpan duration)
        {
            // Blast out the compressed color list, specifying not to change the color until we're done
            foreach(var i in colors.Take(colors.Count - 1))
            {
                await Dispatcher.Instance.SendRequest(IpAddress,
                                                      new SetColorZonesRequest
                                                      {
                                                          Settings = new ColorZoneSettings
                                                          {
                                                              Color = i.Color,
                                                              Duration = duration,
                                                              StartIndex = (byte)i.StartIndex,
                                                              EndIndex = (byte)i.EndIndex,
                                                              Apply = MultiZoneApplicationRequest.NoApply
                                                          }
                                                      });
            }

            // Send the last one to apply the colors
            foreach(var i in colors.Skip(colors.Count - 1))
            {
                await Dispatcher.Instance.SendRequest(IpAddress,
                                                      new SetColorZonesRequest
                                                      {
                                                          Settings = new ColorZoneSettings
                                                          {
                                                              Color = i.Color,
                                                              Duration = duration,
                                                              StartIndex = (byte)i.StartIndex,
                                                              EndIndex = (byte)i.EndIndex,
                                                              Apply = MultiZoneApplicationRequest.Apply
                                                          }
                                                      });
            }
        }

        public Task SetExtendedColorZones(IReadOnlyCollection<Color> colors, TimeSpan duration)
        {
            return Dispatcher.Instance.SendRequest(IpAddress,
                                                   new SetExtendedColorZonesRequest
                                                   {
                                                       Settings = new ExtendedColorZoneSettings { Colors = colors, Duration = duration }
                                                   });
        }

        public Task SetMultiZoneEffect(MultiZoneEffectSettings effectSettings)
        {
            return Dispatcher.Instance.SendRequest(IpAddress, new SetMultiZoneEffectRequest { Settings = effectSettings });
        }

        public Task SetPower(PowerState power)
        {
            return Dispatcher.Instance.SendRequest(IpAddress, new SetPowerRequest { Power = power });
        }

        public Task SetWaveform(WaveformSettings waveformSettings)
        {
            return Dispatcher.Instance.SendRequest(IpAddress, new SetWaveformRequest { Settings = waveformSettings });
        }

        Task<TResponse> GetProperty<TRequest, TResponse>(Action<TResponse> responseHandler)
            where TRequest : Request, new() where TResponse : Response, new()
        {
            return GetProperty(new TRequest(), responseHandler);
        }

        Task<TResponse> GetProperty<TRequest, TResponse>(TRequest request, Action<TResponse> responseHandler)
            where TRequest : Request where TResponse : Response, new()
        {
            var taskCompletionSource = new TaskCompletionSource<TResponse>();
            var task = taskCompletionSource.Task;

            Dispatcher.Instance.SendRequest<TResponse>(IpAddress,
                                                       request,
                                                       response =>
                                                       {
                                                           taskCompletionSource.SetResult(response);
                                                           responseHandler(response);
                                                           return false;
                                                       });

            return task;
        }
    }
}
