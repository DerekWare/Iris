using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DerekWare.Collections;
using DerekWare.Diagnostics;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.HomeAutomation.Common.Effects;
using DerekWare.HomeAutomation.Common.Themes;

namespace DerekWare.HomeAutomation.Common.Scenes
{
    [Serializable]
    public class SceneItem : ISerializable, IEquatable<SceneItem>, IMatch
    {
        IClient _Client;
        IDevice _Device;

        public SceneItem(IDevice device)
        {
            _Device = device;
            _Client = device.Client;

            SnapshotDeviceState();
        }

        public SceneItem(SerializationInfo info, StreamingContext context)
        {
            Family = (string)info.GetValue(nameof(Family), typeof(string));
            Uuid = (string)info.GetValue(nameof(Uuid), typeof(string));

            var e = info.GetEnumerator();

            while(e.MoveNext())
            {
                switch(e.Current.Name)
                {
                    case nameof(Color):
                        Color = (Color)info.GetValue(e.Current.Name, typeof(Color));
                        break;

                    case nameof(Effect):
                        Effect = (Effect)info.GetValue(e.Current.Name, typeof(Effect));
                        break;

                    case nameof(MultiZoneColors):
                        MultiZoneColors = (List<Color>)info.GetValue(e.Current.Name, typeof(List<Color>));
                        break;

                    case nameof(Power):
                        Power = (PowerState)info.GetValue(e.Current.Name, typeof(PowerState));
                        break;

                    case nameof(Theme):
                        Theme = (Theme)info.GetValue(e.Current.Name, typeof(Theme));
                        break;
                }
            }
        }

        SceneItem()
        {
        }

        public IClient Client
        {
            get
            {
                _Client ??= ClientFactory.Instance.CreateInstance(Family);

                if(_Client is null)
                {
                    Debug.Warning(this, $"Unable to find client {Family}");
                }

                return _Client;
            }
        }

        public IDevice Device
        {
            get
            {
                if(Client is null)
                {
                    return null;
                }

                _Device ??= Client.Devices.FirstOrDefault(i => i.Uuid == Uuid) ?? Client.Groups.FirstOrDefault(i => i.Uuid == Uuid);

                if(_Device is null)
                {
                    Debug.Warning(this, $"Unable to find device {Uuid}");
                }

                return _Device;
            }
        }

        public string Name => Device?.Name ?? Uuid;

        public Color Color { get; set; }

        public Effect Effect { get; set; }

        public string Family { get; private set; }

        public IReadOnlyCollection<Color> MultiZoneColors { get; set; }

        public PowerState Power { get; set; }

        public Theme Theme { get; set; }

        public string Uuid { get; private set; }

        public bool ApplyScene()
        {
            if(Device is null)
            {
                return false;
            }

            Device.Power = Power;

            if(Theme is not null)
            {
                Device.Theme = Theme;
            }
            else if(!MultiZoneColors.IsNullOrEmpty())
            {
                Device.MultiZoneColors = MultiZoneColors;
            }
            else
            {
                Device.Color = Color;
            }

            Device.Effect = Effect;

            return true;
        }

        public bool SnapshotDeviceState()
        {
            if(Device is null)
            {
                return false;
            }

            Family = Device.Family;
            Uuid = Device.Uuid;
            Power = Device.Power;
            Color = Device.Color?.Clone();
            MultiZoneColors = Device.MultiZoneColors?.ToList();
            Theme = (Theme)Device.Theme?.Clone();
            Effect = (Effect)Device.Effect?.Clone();

            return true;
        }

        #region Equality

        public bool Equals(SceneItem other)
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

            return Equals((SceneItem)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Family != null ? Family.GetHashCode() : 0) * 397) ^ (Uuid != null ? Uuid.GetHashCode() : 0);
            }
        }

        public static bool operator ==(SceneItem left, SceneItem right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SceneItem left, SceneItem right)
        {
            return !Equals(left, right);
        }

        #endregion

        #region IMatch

        public bool Matches(object obj)
        {
            return obj is IDevice device && Family.Equals(device.Family) && Uuid.Equals(device.Uuid);
        }

        #endregion

        #region ISerializable

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Family), Family, typeof(string));
            info.AddValue(nameof(Uuid), Uuid, typeof(string));
            info.AddValue(nameof(Power), Power, typeof(PowerState));

            if(Power == PowerState.Off)
            {
                return;
            }

            if(Theme is not null)
            {
                info.AddValue(nameof(Theme), Theme, Theme.GetType());
            }
            else if(!MultiZoneColors.IsNullOrEmpty())
            {
                info.AddValue(nameof(MultiZoneColors), MultiZoneColors.ToList(), typeof(List<Color>));
            }
            else if(Color is not null)
            {
                info.AddValue(nameof(Color), Color, typeof(Color));
            }

            if(Effect is not null)
            {
                info.AddValue(nameof(Effect), Effect, Effect.GetType());
            }
        }

        #endregion
    }
}
