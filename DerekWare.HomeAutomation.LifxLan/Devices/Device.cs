using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
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

        int _IsValid;
        MultiZoneEffectSettings _MultiZoneEffect;
        string _Name;
        Products.Product _Product;
        Products.Vendor _Vendor;
        WaveformSettings _Waveform;
        int _ZoneCount = 1;

        internal Device(string ipAddress)
        {
            Controller = new DeviceController(ipAddress);

            StartRefreshTask();
        }

        public override IClient Client => Lan.Client.Instance;
        public override IReadOnlyCollection<IDeviceGroup> Groups => InternalGroups;
        public string IpAddress => Controller.IpAddress;
        public override bool IsColor => _Product?.features.color ?? false;
        public override bool IsValid => _IsValid != 0;
        public override string Name => _Name.IsNullOrEmpty() ? IpAddress : _Name;
        public string Product => _Product?.name;
        public override string Uuid => IpAddress;
        public string Vendor => _Vendor?.name;
        public override int ZoneCount => _ZoneCount;
        public Version FirmwareVersion { get; private set; }
        public bool? IsExtendedMultiZone { get; private set; }

        [Browsable(false)]
        public MultiZoneEffectSettings MultiZoneEffect { get => _MultiZoneEffect; set => SetMultiZoneEffect(value); }

        [Browsable(false)]
        public WaveformSettings Waveform { get => _Waveform; set => SetWaveform(value); }

        public override void RefreshState()
        {
            RefreshStateAsync();
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

        public void SetMultiZoneEffect(MultiZoneEffectSettings settings)
        {
            _MultiZoneEffect = settings.Clone();
            Controller.SetMultiZoneEffect(_MultiZoneEffect);
            OnStateChanged();
        }

        public void SetWaveform(WaveformSettings settings)
        {
            _Waveform = settings.Clone();
            Controller.SetWaveform(_Waveform);
            OnStateChanged();
        }

        protected override void ApplyColor(IReadOnlyCollection<Color> colors, TimeSpan transitionDuration)
        {
            // TODO my LIFX strip that runs an old version of the firmware accepts the
            // SetExtendedColorZones message even though it won't support
            // GetExtendedColorZones.
            if(IsMultiZone && (colors.Count > 1))
            {
                Controller.SetExtendedColorZones(colors, transitionDuration);
            }
            else
            {
                Controller.SetColor(colors.First(), transitionDuration);
            }
        }

        protected override void ApplyPower(PowerState power)
        {
            Controller.SetPower(Power);
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

            await Controller.GetHostFirmware(response =>
            {
                if(!Equals(response.Version, FirmwareVersion))
                {
                    FirmwareVersion = response.Version;
                    OnPropertiesChanged();
                }
            });

            await Controller.GetGroup(response =>
            {
                Lan.Client.Instance.CreateGroup(response, out var group);
                InternalGroups.Add(group);
                group.InternalChildren.Add(this);
            });

            await Controller.GetLocation(response =>
            {
                Lan.Client.Instance.CreateGroup(response, out var group);
                InternalGroups.Add(group);
                group.InternalChildren.Add(this);
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
            if(!IsValid)
            {
                await RefreshPropertiesAsync();
            }

            await Controller.GetPower(response =>
            {
                SetPower(response.Power, false);
            });

            // Rather than using the product registry, we'll attempt to discover device capabilities
            // by using the color messages. If we get a successful response, then we know the device
            // is color, multizone, etc.
            if(!IsMultiZone)
            {
                await Controller.GetColor(response =>
                {
                    SetColor(new[] { response.Color }, TimeSpan.Zero, false);
                });
            }

            if(!IsExtendedMultiZone.HasValue || !IsExtendedMultiZone.Value)
            {
                await Controller.GetColorZones(response =>
                {
                    if(!response.Colors.IsNullOrEmpty())
                    {
                        if(_ZoneCount != response.ZoneCount)
                        {
                            _ZoneCount = response.ZoneCount;
                            OnPropertiesChanged();
                        }

                        SetColor(response.Colors, TimeSpan.Zero, false);
                    }
                });
            }

            if(!IsExtendedMultiZone.HasValue || IsExtendedMultiZone.Value)
            {
                // Assume failure so if we never get a valid response, we don't keep sending
                // this message.
                IsExtendedMultiZone ??= false;

                await Controller.GetExtendedColorZones(response =>
                {
                    // My LIFX Z strip with old firmware responds to this message, but with
                    // an empty color array.
                    if(!response.Colors.IsNullOrEmpty())
                    {
                        if(!IsExtendedMultiZone.Value || (_ZoneCount != response.ZoneCount))
                        {
                            IsExtendedMultiZone = true;
                            _ZoneCount = response.ZoneCount;
                            OnPropertiesChanged();
                        }

                        SetColor(response.Colors, TimeSpan.Zero, false);
                    }
                });
            }

            // TODO query firmware effect/waveform
            if(0 == Interlocked.Exchange(ref _IsValid, 1))
            {
                OnPropertiesChanged();
            }
        }
    }
}
