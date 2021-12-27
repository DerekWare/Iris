using System.ComponentModel;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common;
using DerekWare.HomeAutomation.Lifx.Lan.Messages;

namespace DerekWare.HomeAutomation.Lifx.Lan.Devices
{
    public sealed class DeviceGroup : Common.DeviceGroup
    {
        internal DeviceGroup(GroupResponse response)
        {
            Name = response.Label;
            Uuid = response.Uuid;
        }

        [Browsable(false)]
        public override IClient Client => Lan.Client.Instance;

        public override string Name { get; }

        public override string Uuid { get; }

        internal SynchronizedHashSet<IDevice> InternalChildren => Children;

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
    }
}
