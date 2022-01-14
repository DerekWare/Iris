using System;
using System.Collections.Generic;
using System.Linq;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common;
using DerekWare.HomeAutomation.Common.Colors;
using Q42.HueApi;
using Q42.HueApi.Models.Groups;

namespace DerekWare.HomeAutomation.PhilipsHue
{
    class DeviceGroup : Common.DeviceGroup, IHueDevice
    {
        protected Group HueGroup;

        readonly object SyncRoot = new();

        internal DeviceGroup(Group hueGroup)
        {
            SetState(hueGroup);
        }

        internal void SetState(Group hueGroup)
        {
            lock(SyncRoot)
            {
                HueGroup = hueGroup;

                foreach(var i in HueGroup.Lights)
                {
                    if(PhilipsHue.Client.Instance.InternalDevices.TryGetValue(i, out var device))
                    {
                        InternalChildren.Add(device);
                    }
                }
            }
        }

        public RoomClass? Class
        {
            get
            {
                lock(SyncRoot)
                {
                    return HueGroup.Class;
                }
            }
        }

        public override IClient Client => PhilipsHue.Client.Instance;

        public string ModelId
        {
            get
            {
                lock(SyncRoot)
                {
                    return HueGroup.ModelId;
                }
            }
        }

        public override string Name
        {
            get
            {
                lock(SyncRoot)
                {
                    return HueGroup.Name;
                }
            }
        }

        public GroupType? Type
        {
            get
            {
                lock(SyncRoot)
                {
                    return HueGroup.Type;
                }
            }
        }

        public override string Uuid {
            get
            {
                lock(SyncRoot)
                {
                    return HueGroup.Id;
                }
            }
        }

        internal new SynchronizedHashSet<IDevice> InternalChildren => base.InternalChildren;

        protected override void ApplyColor(IReadOnlyCollection<Color> color, TimeSpan transitionDuration)
        {
            // See if the colors are actually different. If they're not, we can optimize setting
            // the color on the group, not the lights.
            var first = color.First();
            var identical = (color.Count <= 1) || color.All(i => i == first);

            if(!identical)
            {
                base.ApplyColor(color, transitionDuration);
                return;
            }

            SendCommand(first.ToLightCommand());

            lock(SyncRoot)
            {
                Children.ForEach<Device>(i => i.SetColor(color, TimeSpan.Zero, false));
            }
        }

        protected override void ApplyPower(PowerState power)
        {
            SendCommand(new LightCommand { On = power == PowerState.On });

            lock(SyncRoot)
            {
                Children.ForEach<Device>(i => i.SetPower(power, false));
            }
        }

        protected override void OnPropertiesChanged()
        {
            base.OnPropertiesChanged();
            PhilipsHue.Client.Instance.OnPropertiesChanged(this);
        }

        protected override void OnStateChanged()
        {
            base.OnStateChanged();
            PhilipsHue.Client.Instance.OnStateChanged(this);
        }

        #region IDeviceState

        public override void SetFirmwareEffect(object effect)
        {
            SendCommand(new LightCommand { Effect = Extensions.GetEffectType(effect) });
        }

        #endregion

        #region IHueDevice

        public void SendCommand(LightCommand cmd)
        {
            lock(SyncRoot)
            {
                cmd.SendCommand(HueGroup);
            }
        }

        #endregion
    }
}
