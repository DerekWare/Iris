using System;
using System.Collections.Generic;
using System.IO;
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
    public class SceneItem : ISerializable, IEquatable<SceneItem>
    {
        public SceneItem(IDevice device)
        {
            SnapshotDevice(device);
        }

        public SceneItem(SerializationInfo info, StreamingContext context)
        {
            Family = (string)info.GetValue(nameof(Family), typeof(string));
            Uuid = (string)info.GetValue(nameof(Uuid), typeof(string));

            try
            {
                Power = (PowerState)info.GetValue(nameof(Power), typeof(PowerState));
            }
            catch(SerializationException ex)
            {
                Debug.Warning(this, ex);
            }

            try
            {
                Theme = (Theme)info.GetValue(nameof(Theme), typeof(Theme));
            }
            catch(SerializationException ex)
            {
                Debug.Warning(this, ex);
            }

            try
            {
                MultiZoneColors = (List<Color>)info.GetValue(nameof(MultiZoneColors), typeof(List<Color>));
            }
            catch(SerializationException ex)
            {
                Debug.Warning(this, ex);
            }

            try
            {
                Color = (Color)info.GetValue(nameof(Color), typeof(Color));
            }
            catch(SerializationException ex)
            {
                Debug.Warning(this, ex);
            }

            try
            {
                Effect = (Effect)info.GetValue(nameof(Effect), typeof(Effect));
            }
            catch(SerializationException ex)
            {
                Debug.Warning(this, ex);
            }

            FindDevice();
        }

        SceneItem()
        {
        }

        public string Name => Device?.Name;

        public IClient Client { get; private set; }

        public Color Color { get; private set; }

        public IDevice Device { get; private set; }

        public Effect Effect { get; private set; }

        public string Family { get; private set; }

        public IReadOnlyCollection<Color> MultiZoneColors { get; private set; }

        public PowerState Power { get; private set; }

        public Theme Theme { get; private set; }

        public string Uuid { get; private set; }

        public void Apply()
        {
            if(Device is null)
            {
                FindDevice();
            }

            // Apply values
            Device.Power = Power;
            Device.Theme = Theme;

            if(!MultiZoneColors.IsNullOrEmpty())
            {
                Device.MultiZoneColors = MultiZoneColors;
            }
            else
            {
                Device.Color = Color;
            }

            Device.Effect = Effect;
        }

        public bool IsDevice(IDevice device)
        {
            return Family.Equals(device.Family) && Uuid.Equals(device.Uuid);
        }

        public void SnapshotDevice(IDevice device)
        {
            Device = device;
            Family = device.Family;
            Uuid = device.Uuid;
            Power = device.Power;
            Theme = (Theme)device.Theme?.Clone();
            MultiZoneColors = device.MultiZoneColors?.ToList();
            Color = device.Color?.Clone();
            Effect = (Effect)device.Effect?.Clone();
        }

        void FindDevice()
        {
            // Find the client by family
            Client = ClientFactory.Instance.CreateInstance(Family);

            if(Client is null)
            {
                throw new InvalidDataException($"Unable to find client {Family}");
            }

            // Find the Cache by uuid
            Device = Client.Devices.FirstOrDefault(i => i.Uuid == Uuid);

            if(Device is null)
            {
                throw new InvalidDataException($"Unable to find Cache {Uuid}");
            }
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
