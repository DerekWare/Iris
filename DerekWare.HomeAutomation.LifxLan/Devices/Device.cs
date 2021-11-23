using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DerekWare.Collections;
using DerekWare.Diagnostics;
using DerekWare.HomeAutomation.Common;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.HomeAutomation.Lifx.Lan.Messages;

namespace DerekWare.HomeAutomation.Lifx.Lan.Devices
{
    // TODO property accessors aren't thread safe
    public class Device : Common.Device
    {
        internal SynchronizedHashSet<DeviceGroup> InternalGroups = new();

        readonly DeviceController Controller;

        MultiZoneEffectSettings _MultiZoneEffect;
        string _Name;
        Products.Product _Product;
        Products.Vendor _Vendor;
        WaveformSettings _Waveform;

        internal Device(string ipAddress, StateService response)
        {
            Controller = new DeviceController(ipAddress);
            _Name = ipAddress;

            OnCreate();
        }

        [Browsable(false), XmlIgnore]
        public override IClient Client => Lan.Client.Instance;

        [Browsable(false), XmlIgnore]
        public override IReadOnlyCollection<IDeviceGroup> Groups => InternalGroups;

        public string IpAddress => Controller.IpAddress;
        public override bool IsColor => _Product?.features.color ?? false;
        public bool IsExtendedMultiZone => _Product?.features.extended_multizone ?? false;
        public override bool IsMultiZone => _Product?.features.multizone ?? false;
        public override string Name => _Name;
        public override string Product => _Product?.name;
        public override string Uuid => IpAddress;
        public override string Vendor => _Vendor?.name;
        public override int ZoneCount => MultiZoneColors.Count;

        [Browsable(false), XmlIgnore]
        public MultiZoneEffectSettings MultiZoneEffect { get => _MultiZoneEffect; set => SetMultiZoneEffect(value); }

        [Browsable(false), XmlIgnore]
        public WaveformSettings Waveform { get => _Waveform; set => SetWaveform(value); }

        public async Task RefreshPropertiesAsync()
        {
            await Controller.GetVersion(response =>
            {
                _Vendor = Products.GetVendor((int)response.VendorId);
                _Product = _Vendor.GetProduct((int)response.ProductId);
            });

            await Controller.GetGroup(response =>
            {
                Lan.Client.Instance.CreateGroup(response, out var group);
                InternalGroups.Add(group);
                group.InternalDevices.Add(this);
            });

            await Controller.GetLocation(response =>
            {
                Lan.Client.Instance.CreateGroup(response, out var group);
                InternalGroups.Add(group);
                group.InternalDevices.Add(this);
            });

            await Controller.GetLabel(response =>
            {
                _Name = response.Label;
            });

            OnPropertiesChanged();
        }

        public override void RefreshState()
        {
            RefreshStateAsync();
        }

        public async Task RefreshStateAsync()
        {
            await Controller.GetPower(response =>
            {
                if(response.Power != _Power)
                {
                    _Power = response.Power;
                    OnStateChanged();
                }
            });

            await Controller.GetColor(response =>
            {
                if(response.Color != _Color)
                {
                    _Color = response.Color;
                    OnStateChanged();
                }
            });

            if(IsExtendedMultiZone)
            {
                await Controller.GetExtendedColorZones(response =>
                {
                    if(!_MultiZoneColors.SequenceEqual(response.Colors))
                    {
                        _MultiZoneColors = response.Colors.ToList();
                        OnStateChanged();
                    }
                });
            }
            else if(IsMultiZone)
            {
                await Controller.GetColorZones(response =>
                {
                    if(!_MultiZoneColors.SequenceEqual(response.Colors))
                    {
                        _MultiZoneColors = response.Colors.ToList();
                        OnStateChanged();
                    }
                });
            }

            StartRefreshTask();
        }

        public override void SetColor(Color color, TimeSpan transitionDuration)
        {
            base.SetColor(color, transitionDuration);
            Controller.SetColor(Color, transitionDuration);
        }

        public override void SetFirmwareEffect(object effect)
        {
            if(effect is null)
            {
                Controller.SetMultiZoneEffect(new MultiZoneEffectSettings { EffectType = MultiZoneEffectType.Off });
                return;
            }

            if(effect is MultiZoneEffectSettings effectSettings)
            {
                SetMultiZoneEffect(effectSettings);
            }
            else if(effect is WaveformSettings waveformSettings)
            {
                SetWaveform(waveformSettings);
            }
            else
            {
                Debug.Warning(this, "Invalid effect settings");
            }
        }

        public override void SetMultiZoneColors(IReadOnlyCollection<Color> colors, TimeSpan transitionDuration)
        {
            base.SetMultiZoneColors(colors, transitionDuration);

            if(IsMultiZone)
            {
                Controller.SetExtendedColorZones(MultiZoneColors, transitionDuration);
            }
            else
            {
                Controller.SetColor(_Color, transitionDuration);
            }
        }

        public void SetMultiZoneEffect(MultiZoneEffectSettings settings)
        {
            if(IsMultiZone || IsExtendedMultiZone)
            {
                _MultiZoneEffect = settings.Clone();
                Controller.SetMultiZoneEffect(_MultiZoneEffect);
                OnStateChanged();
            }
        }

        public override void SetPower(PowerState power)
        {
            base.SetPower(power);
            Controller.SetPower(Power);
        }

        public void SetWaveform(WaveformSettings settings)
        {
            if(IsMultiZone || IsExtendedMultiZone)
            {
                _Waveform = settings.Clone();
                Controller.SetWaveform(_Waveform);
                OnStateChanged();
            }
        }

        protected override void OnPropertiesChanged(DeviceEventArgs e)
        {
            base.OnPropertiesChanged(e);
            Lan.Client.Instance.OnPropertiesChanged(this);
        }

        protected override void OnStateChanged(DeviceEventArgs e)
        {
            base.OnStateChanged(e);
            Lan.Client.Instance.OnStateChanged(this);
        }

        async void OnCreate()
        {
            await RefreshPropertiesAsync();
            await RefreshStateAsync();
        }
    }
}
