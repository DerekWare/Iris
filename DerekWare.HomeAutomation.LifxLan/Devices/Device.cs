using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using DerekWare.Collections;
using DerekWare.Diagnostics;
using DerekWare.HomeAutomation.Common;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.HomeAutomation.Lifx.Lan.Messages;

namespace DerekWare.HomeAutomation.Lifx.Lan.Devices
{
    // TODO property accessors aren't thread safe
    public sealed class Device : Common.Device
    {
        internal SynchronizedHashSet<DeviceGroup> InternalGroups = new();

        readonly DeviceController Controller;

        MultiZoneEffectSettings _MultiZoneEffect;
        string _Name;
        Products.Product _Product;
        Products.Vendor _Vendor;
        WaveformSettings _Waveform;
        int _ZoneCount = 1;

        internal Device(string ipAddress, ServiceResponse response)
        {
            Controller = new DeviceController(ipAddress);

            StartRefreshTask();
        }

        public override IClient Client => Lan.Client.Instance;

        public override IReadOnlyCollection<IDeviceGroup> Groups => InternalGroups;

        public string IpAddress => Controller.IpAddress;

        public override bool IsColor => _Product?.features.color ?? false;

        public bool IsExtendedMultiZone => _Product?.features.extended_multizone ?? false;

        public override bool IsMultiZone => _Product?.features.multizone ?? false;

        public override bool IsValid => !_Name.IsNullOrEmpty();

        public override string Name => _Name.IsNullOrEmpty() ? IpAddress : _Name;

        public override string Product => _Product?.name;

        public override string Uuid => IpAddress;

        public override string Vendor => _Vendor?.name;

        public override int ZoneCount => _ZoneCount;

        [Browsable(false)]
        public MultiZoneEffectSettings MultiZoneEffect { get => _MultiZoneEffect; set => SetMultiZoneEffect(value); }

        [Browsable(false)]
        public WaveformSettings Waveform { get => _Waveform; set => SetWaveform(value); }

        public override void RefreshState()
        {
            RefreshStateAsync();
        }

        public override void SetColor(Color color, TimeSpan transitionDuration)
        {
            base.SetColor(color, transitionDuration);
            Controller.SetColor(Color, transitionDuration);
        }

        public override void SetFirmwareEffect(object effect)
        {
            effect ??= new MultiZoneEffectSettings { EffectType = MultiZoneEffectType.Off };

            switch(effect)
            {
                case MultiZoneEffectSettings effectSettings:
                    SetMultiZoneEffect(effectSettings);
                    break;

                case WaveformSettings waveformSettings:
                    SetWaveform(waveformSettings);
                    break;

                default:
                    Debug.Warning(this, "Invalid effect settings");
                    break;
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
                Controller.SetColor(Color, transitionDuration);
            }
        }

        public void SetMultiZoneEffect(MultiZoneEffectSettings settings)
        {
            _MultiZoneEffect = settings.Clone();
            Controller.SetMultiZoneEffect(_MultiZoneEffect);
            OnStateChanged();
        }

        public override void SetPower(PowerState power)
        {
            base.SetPower(power);
            Controller.SetPower(Power);
        }

        public void SetWaveform(WaveformSettings settings)
        {
            _Waveform = settings.Clone();
            Controller.SetWaveform(_Waveform);
            OnStateChanged();
        }

        protected override void OnPropertiesChanged()
        {
            base.OnPropertiesChanged();
            Lan.Client.Instance.OnPropertiesChanged(this);
        }

        protected override void OnStateChanged()
        {
            base.OnStateChanged();
            Lan.Client.Instance.OnStateChanged(this);
        }

        async Task RefreshPropertiesAsync()
        {
            await Controller.GetVersion(response =>
            {
                var vendor = Products.GetVendor((int)response.VendorId);

                if(!Equals(vendor, _Vendor))
                {
                    _Vendor = vendor;
                    OnPropertiesChanged();
                }

                var product = _Vendor.GetProduct((int)response.ProductId);

                if(!Equals(product, _Product))
                {
                    _Product = product;
                    OnPropertiesChanged();
                }
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
                var name = response.Label;

                if(!Equals(name, _Name))
                {
                    _Name = name;
                    OnPropertiesChanged();
                }
            });
        }

        async Task RefreshStateAsync()
        {
            await RefreshPropertiesAsync();

            await Controller.GetPower(response =>
            {
                base.SetPower(response.Power);
            });

            if(IsExtendedMultiZone)
            {
                await Controller.GetExtendedColorZones(response =>
                {
                    if(_ZoneCount != response.ZoneCount)
                    {
                        _ZoneCount = response.ZoneCount;
                        OnPropertiesChanged();
                    }

                    if(!_MultiZoneColors.SequenceEqual(response.Colors))
                    {
                        base.SetMultiZoneColors(response.Colors, TimeSpan.Zero);
                    }
                });
            }
            else if(IsMultiZone)
            {
                await Controller.GetColorZones(response =>
                {
                    if(_ZoneCount != response.ZoneCount)
                    {
                        _ZoneCount = response.ZoneCount;
                        OnPropertiesChanged();
                    }

                    if(!_MultiZoneColors.SequenceEqual(response.Colors))
                    {
                        base.SetMultiZoneColors(response.Colors, TimeSpan.Zero);
                    }
                });
            }
            else
            {
                await Controller.GetColor(response =>
                {
                    base.SetColor(response.Color, TimeSpan.Zero);
                });
            }

            // TODO query firmware effect/waveform
        }
    }
}
