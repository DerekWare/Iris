using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using DerekWare.Diagnostics;

namespace DerekWare.HomeAutomation.Common.Devices
{
    [Description("DeferredDevice allows for lazy loading of a device based on Family and Uuid.")]
    public interface IDeferredDevice : IFamily, IUuid, IName, ISerializable, IEquatable<DeferredDevice>, IMatch, IDisposable
    {
        public IClient Client { get; }
        public IDevice Device { get; }
    }

    [Serializable]
    public class DeferredDevice : IDeferredDevice
    {
        IClient _Client;
        IDevice _Device;

        public event EventHandler<DeviceEventArgs> DeviceDiscovered;

        public DeferredDevice(IDevice device)
        {
            _Device = device;
            _Client = device.Client;

            Family = _Device.Family;
            Uuid = _Device.Uuid;
        }

        public DeferredDevice(SerializationInfo info, StreamingContext context)
        {
            Family = (string)info.GetValue(nameof(Family), typeof(string));
            Uuid = (string)info.GetValue(nameof(Uuid), typeof(string));

            if(Client is not null)
            {
                Client.DeviceDiscovered += OnDeviceDiscovered;
            }
        }

        protected DeferredDevice()
        {
        }

        public IClient Client
        {
            get
            {
                if(_Client is null)
                {
                    _Client = ClientFactory.Instance.CreateInstance(Family);

                    if(_Client is null)
                    {
                        Debug.Warning(this, $"Unable to find client {Family}");
                    }
                }

                return _Client;
            }
        }

        public IDevice Device
        {
            get
            {
                if(_Device is null)
                {
                    if(Client is null)
                    {
                        return null;
                    }

                    _Device = Client.Devices.FirstOrDefault(i => i.Uuid == Uuid) ?? Client.Groups.FirstOrDefault(i => i.Uuid == Uuid);

                    if(_Device is null)
                    {
                        Debug.Warning(this, $"Unable to find device {Uuid}");
                    }
                }

                return _Device;
            }
        }

        public string Family { get; }

        public string Name => Device?.Name ?? Uuid;

        public string Uuid { get; }

        protected virtual void Dispose(bool disposing)
        {
            if(disposing)
            {
                if(_Client is not null)
                {
                    _Client.DeviceDiscovered -= OnDeviceDiscovered;
                }

                _Client = null;
                _Device = null;
            }
        }

        #region Equality

        public bool Equals(DeferredDevice other)
        {
            if(ReferenceEquals(null, other))
            {
                return false;
            }

            if(ReferenceEquals(this, other))
            {
                return true;
            }

            return (Family == other.Family) && (Uuid == other.Uuid);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DeferredDevice);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Family != null ? Family.GetHashCode() : 0) * 397) ^ (Uuid != null ? Uuid.GetHashCode() : 0);
            }
        }

        public static bool operator ==(DeferredDevice left, DeferredDevice right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DeferredDevice left, DeferredDevice right)
        {
            return !Equals(left, right);
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region IMatch

        public virtual bool Matches(object obj)
        {
            return obj is IFamily family and IUuid uuid && Family.Equals(family.Family) && Uuid.Equals(uuid.Uuid);
        }

        #endregion

        #region ISerializable

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Family), Family, Family.GetType());
            info.AddValue(nameof(Uuid), Uuid, Uuid.GetType());
        }

        #endregion

        #region Event Handlers

        void OnDeviceDiscovered(object sender, DeviceEventArgs e)
        {
            if(!Matches(e.Device))
            {
                return;
            }

            _Device = e.Device;
            _Client = e.Device.Client;

            DeviceDiscovered?.Invoke(this, e);
        }

        #endregion
    }
}
