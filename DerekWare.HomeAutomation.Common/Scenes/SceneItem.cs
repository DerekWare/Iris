using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.HomeAutomation.Common.Devices;
using DerekWare.HomeAutomation.Common.Effects;
using DerekWare.HomeAutomation.Common.Themes;

namespace DerekWare.HomeAutomation.Common.Scenes
{
    [Serializable]
    public class SceneItem : DeferredDevice, IEquatable<SceneItem>
    {
        public SceneItem(IDevice device)
            : base(device)
        {
            SnapshotDeviceState();
        }

        public SceneItem(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
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

        public Color Color { get; set; }

        public Effect Effect { get; set; }

        public IReadOnlyCollection<Color> MultiZoneColors { get; set; }

        public PowerState Power { get; set; }

        public Theme Theme { get; set; }

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

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(Power), Power, Power.GetType());

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
                info.AddValue(nameof(Color), Color, Color.GetType());
            }

            if(Effect is not null)
            {
                info.AddValue(nameof(Effect), Effect, Effect.GetType());
            }
        }

        public bool SnapshotDeviceState()
        {
            if(Device is null)
            {
                return false;
            }

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
            return base.Equals(other);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
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
    }
}
