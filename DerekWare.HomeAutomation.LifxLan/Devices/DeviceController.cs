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

        public Task<LightStateResponse> GetColor(Dispatcher.ResponseHandler<LightStateResponse> responseHandler)
        {
            return GetProperty<GetColorRequest, LightStateResponse>(responseHandler);
        }

        public Task<MultiZoneColorsResponse> GetColorZones(Dispatcher.ResponseHandler<MultiZoneColorsResponse> responseHandler)
        {
            return GetProperty<GetMultiZoneColorsRequest, MultiZoneColorsResponse>(responseHandler);
        }

        public Task<ExtendedMultiZoneColorsResponse> GetExtendedColorZones(Dispatcher.ResponseHandler<ExtendedMultiZoneColorsResponse> responseHandler)
        {
            return GetProperty<GetExtendedMultiZoneColorsRequest, ExtendedMultiZoneColorsResponse>(responseHandler);
        }

        public Task<GroupResponse> GetGroup(Dispatcher.ResponseHandler<GroupResponse> responseHandler)
        {
            return GetProperty<GetGroupRequest, GroupResponse>(responseHandler);
        }

        public Task<LabelResponse> GetLabel(Dispatcher.ResponseHandler<LabelResponse> responseHandler)
        {
            return GetProperty<GetLabelRequest, LabelResponse>(responseHandler);
        }

        public Task<LocationResponse> GetLocation(Dispatcher.ResponseHandler<LocationResponse> responseHandler)
        {
            return GetProperty<GetLocationRequest, LocationResponse>(responseHandler);
        }

        public Task<MultiZoneColorsResponse> GetMultiZoneColors(Dispatcher.ResponseHandler<MultiZoneColorsResponse> responseHandler)
        {
            return GetProperty<GetMultiZoneColorsRequest, MultiZoneColorsResponse>(responseHandler);
        }

        public Task<PowerResponse> GetPower(Dispatcher.ResponseHandler<PowerResponse> responseHandler)
        {
            return GetProperty<GetPowerRequest, PowerResponse>(responseHandler);
        }

        public Task<VersionResponse> GetVersion(Dispatcher.ResponseHandler<VersionResponse> responseHandler)
        {
            return GetProperty<GetVersionRequest, VersionResponse>(responseHandler);
        }

        public Task SetColor(Color color, TimeSpan duration)
        {
            return Dispatcher.Instance.SendRequest(IpAddress, new SetColorRequest { Color = color, Duration = duration });
        }

        public Task SetExtendedColorZones(IReadOnlyCollection<Color> colors, TimeSpan duration)
        {
            return Dispatcher.Instance.SendRequest(IpAddress,
                                                   new SetExtendedMultiZoneColorsRequest
                                                   {
                                                       Settings = new ExtendedColorZoneSettings { Colors = colors, Duration = duration }
                                                   });
        }

        public async Task SetMultiZoneColors(IReadOnlyCollection<ColorZone> colors, TimeSpan duration)
        {
            // Blast out the compressed color list, specifying not to change the color until we're done
            foreach(var i in colors.Take(colors.Count - 1))
            {
                await Dispatcher.Instance.SendRequest(IpAddress,
                                                      new SetMultiZoneColorsRequest
                                                      {
                                                          Settings = new MultiZoneColorSettings
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
                                                      new SetMultiZoneColorsRequest
                                                      {
                                                          Settings = new MultiZoneColorSettings
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

        Task<TResponse> GetProperty<TRequest, TResponse>(Dispatcher.ResponseHandler<TResponse> responseHandler)
            where TRequest : Request, new() where TResponse : Response, new()
        {
            return Dispatcher.Instance.SendRequest(IpAddress, new TRequest(), responseHandler);
        }
    }
}
