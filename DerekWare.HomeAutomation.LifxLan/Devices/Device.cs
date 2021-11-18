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
using DerekWare.HomeAutomation.Common.Effects;
using DerekWare.HomeAutomation.Lifx.Lan.Messages;

namespace DerekWare.HomeAutomation.Lifx.Lan.Devices
{
    // TODO property accessors aren't thread safe
    public class Device : IDevice, IEquatable<Device>, IDisposable
    {
        internal SynchronizedHashSet<DeviceGroup> InternalGroups = new();

        readonly DeviceController Controller;

        Color _Color;
        List<Color> _MultiZoneColors = new() { new Color() };
        MultiZoneEffectSettings _MultiZoneEffect;
        PowerState _Power;
        Products.Product _Product;
        Products.Vendor _Vendor;
        WaveformSettings _Waveform;
        DeviceStateRefreshTask RefreshTask;

        public event EventHandler<DeviceEventArgs> PropertiesChanged;
        public event EventHandler<DeviceEventArgs> StateChanged;

        internal Device(string ipAddress, StateService response)
        {
            Controller = new DeviceController(ipAddress);
            Name = ipAddress;

            OnCreate();
        }

        public int DeviceCount => 1;

        [Browsable(false), XmlIgnore]
        public IReadOnlyCollection<IDevice> DeviceList => new IDevice[] { this };

        public string Family => Client.Instance.Family;

        [Browsable(false), XmlIgnore]
        public IReadOnlyCollection<IDeviceGroup> Groups => InternalGroups;

        public string IpAddress => Controller.IpAddress;

        public bool IsColor => _Product?.features.color ?? false;

        public bool IsExtendedMultiZone => _Product?.features.extended_multizone ?? false;

        public bool IsLifxZStrip => (_Vendor?.vid == 1) && (_Product.pid == 32);

        public bool IsMultiZone => _Product?.features.multizone ?? false;

        public string Product => _Product?.name;

        public string Uuid => IpAddress;

        public string Vendor => _Vendor?.name;

        public int ZoneCount => MultiZoneColors.Count;

        public IReadOnlyCollection<IEffect> Effects => EffectFactory.Instance.GetRunningEffects(this).ToList();

        [Browsable(false), XmlIgnore]
        public Color Color { get => _Color; set => SetColor(value); }

        [Browsable(false), XmlIgnore]
        public IReadOnlyCollection<Color> MultiZoneColors { get => _MultiZoneColors; set => SetMultiZoneColors(value); }

        [Browsable(false), XmlIgnore]
        public MultiZoneEffectSettings MultiZoneEffect { get => _MultiZoneEffect; set => SetMultiZoneEffect(value); }

        public string Name { get; private set; }

        [Browsable(false), XmlIgnore]
        public PowerState Power { get => _Power; set => SetPower(value); }

        [Browsable(false), XmlIgnore]
        public WaveformSettings Waveform { get => _Waveform; set => SetWaveform(value); }

        public async Task RefreshProperties()
        {
            await Controller.GetVersion(response =>
            {
                _Vendor = Products.GetVendor((int)response.VendorId);
                _Product = _Vendor.GetProduct((int)response.ProductId);
            });

            await Controller.GetGroup(response =>
            {
                Client.Instance.CreateGroup(response, out var group);
                InternalGroups.Add(group);
                group.InternalDevices.Add(this);
            });

            await Controller.GetLocation(response =>
            {
                Client.Instance.CreateGroup(response, out var group);
                InternalGroups.Add(group);
                group.InternalDevices.Add(this);
            });

            await Controller.GetLabel(response =>
            {
                Name = response.Label;
            });

            OnDevicePropertiesChanged();
        }

        public async Task RefreshState()
        {
            await Controller.GetPower(response =>
            {
                if(response.Power != _Power)
                {
                    _Power = response.Power;
                    OnDeviceStateChanged();

                    // If we're turned off, stop running effects
                    if(Power == PowerState.Off)
                    {
                        EffectFactory.Instance.Stop(this);
                    }
                }
            });

            await Controller.GetColor(response =>
            {
                if(response.Color != _Color)
                {
                    _Color = response.Color;
                    OnDeviceStateChanged();
                }
            });

            if(IsExtendedMultiZone)
            {
                await Controller.GetExtendedColorZones(response =>
                {
                    if(!_MultiZoneColors.SequenceEqual(response.Colors))
                    {
                        _MultiZoneColors = response.Colors.ToList();
                        OnDeviceStateChanged();
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
                        OnDeviceStateChanged();
                    }
                });
            }
        }

        public void SetColor(Color color)
        {
            SetColor(color, TimeSpan.Zero);
        }

        public void SetMultiZoneColors(IReadOnlyCollection<Color> colors)
        {
            SetMultiZoneColors(colors, TimeSpan.Zero);
        }

        public void SetMultiZoneEffect(MultiZoneEffectSettings settings)
        {
            if(IsMultiZone || IsExtendedMultiZone)
            {
                _MultiZoneEffect = settings.Clone();
                Controller.SetMultiZoneEffect(_MultiZoneEffect);
                OnDeviceStateChanged();
            }
        }

        public void SetWaveform(WaveformSettings settings)
        {
            if(IsMultiZone || IsExtendedMultiZone)
            {
                _Waveform = settings.Clone();
                Controller.SetWaveform(_Waveform);
                OnDeviceStateChanged();
            }
        }

        public override string ToString()
        {
            return $"{Name} ({Family})";
        }

        internal void OnDevicePropertiesChanged()
        {
            OnDevicePropertiesChanged(new DeviceEventArgs { Device = this });
        }

        internal void OnDevicePropertiesChanged(DeviceEventArgs e)
        {
            PropertiesChanged?.Invoke(this, e);
            Client.Instance.OnPropertiesChanged(this);
        }

        internal void OnDeviceStateChanged()
        {
            OnDeviceStateChanged(new DeviceEventArgs { Device = this });
        }

        internal void OnDeviceStateChanged(DeviceEventArgs e)
        {
            StateChanged?.Invoke(this, e);
            Client.Instance.OnStateChanged(this);
        }

        async void OnCreate()
        {
            await RefreshProperties();
            await RefreshState();

            RefreshTask = new DeviceStateRefreshTask(this);
        }

        #region Equality

        public bool Equals(Device other)
        {
            if(ReferenceEquals(null, other))
            {
                return false;
            }

            if(ReferenceEquals(this, other))
            {
                return true;
            }

            return IpAddress.Equals(other.IpAddress);
        }

        public bool Equals(IDevice other)
        {
            return Equals(other as Device);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj))
            {
                return false;
            }

            if(ReferenceEquals(this, obj))
            {
                return true;
            }

            if(obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((Device)obj);
        }

        public override int GetHashCode()
        {
            return IpAddress != null ? IpAddress.GetHashCode() : 0;
        }

        public static bool operator ==(Device left, Device right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Device left, Device right)
        {
            return !Equals(left, right);
        }

        #endregion

        #region IDeviceState

        public void SetColor(Color color, TimeSpan transitionDuration)
        {
            _Color = color.Clone();
            _MultiZoneColors = _Color.Repeat(ZoneCount).ToList();
            Controller.SetColor(_Color, transitionDuration);
            OnDeviceStateChanged();
        }

        public void SetFirmwareEffect(object effect)
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

        public void SetMultiZoneColors(IReadOnlyCollection<Color> colors, TimeSpan transitionDuration)
        {
            var c = new List<Color>();

            while(c.Count < ZoneCount)
            {
                c.AddRange(colors.Take(Math.Min(colors.Count, ZoneCount - c.Count)).Select(i => i.Clone()));
            }

            _MultiZoneColors = c;
            _Color = _MultiZoneColors[0];

            if(IsExtendedMultiZone || IsLifxZStrip)
            {
                Controller.SetExtendedColorZones(_MultiZoneColors, transitionDuration);
            }
            else if(IsMultiZone)
            {
                Controller.SetColorZones(_MultiZoneColors.Compress().ToList(), transitionDuration);
            }
            else
            {
                Controller.SetColor(_Color, transitionDuration);
            }

            OnDeviceStateChanged();
        }

        public void SetPower(PowerState power)
        {
            _Power = power;
            Controller.SetPower(_Power);
            OnDeviceStateChanged();
        }

        void IDeviceState.RefreshState()
        {
            RefreshState();
        }

        #endregion

        #region IDisposable

        public virtual void Dispose()
        {
            Extensions.Dispose(ref RefreshTask);
        }

        #endregion
    }
}
