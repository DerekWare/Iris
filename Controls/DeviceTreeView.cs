using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common;
using PowerState = DerekWare.HomeAutomation.Common.PowerState;

namespace DerekWare.Iris
{
    class DeviceTreeView : TreeView
    {
        readonly DeviceFilterNode EffectActiveNode = new("Effect Active") { Predicate = device => device.Effect is not null };
        readonly DeviceFilterNode PowerOffNode = new("Power Off") { Predicate = device => device.Power == PowerState.Off };
        readonly DeviceFilterNode PowerOnNode = new("Power On") { Predicate = device => device.Power == PowerState.On };
        readonly DeviceFilterNode StateNode = new("State") { HideDevices = true };

        bool _ShowFamilyNodes;
        bool _ShowStateNodes;

        public DeviceTreeView()
        {
            if(DesignMode)
            {
                return;
            }

            TreeNode.Add(Nodes, new DevicesNode("All Devices"));
            TreeNode.Add(Nodes, new GroupsNode("All Groups"));

            StateNode.Nodes.Add(EffectActiveNode);
            StateNode.Nodes.Add(PowerOffNode);
            StateNode.Nodes.Add(PowerOnNode);
        }

        public bool ShowFamilyNodes
        {
            get => _ShowFamilyNodes;
            set
            {
                _ShowFamilyNodes = value;

                if(DesignMode)
                {
                    return;
                }

                if(value)
                {
                    UpdateDeviceList();
                }
                else
                {
                    Nodes.RemoveWhere<DeviceFamilyNode>(i => true);
                }
            }
        }

        public bool ShowStateNodes
        {
            get => _ShowStateNodes;
            set
            {
                _ShowStateNodes = value;

                if(DesignMode)
                {
                    return;
                }

                if(value)
                {
                    if(!Nodes.Contains(StateNode))
                    {
                        Nodes.Add(StateNode);
                    }

                    UpdateDeviceList();
                }
                else
                {
                    Nodes.Remove(StateNode);
                }
            }
        }

        protected virtual void AddDevice(IDevice device)
        {
            // Add device family nodes
            if(ShowFamilyNodes && !device.Family.IsNullOrEmpty())
            {
                var familyNode = TreeNode.Find<DeviceFamilyNode>(Nodes, device.Family) ?? TreeNode.Add(Nodes, new DeviceFamilyNode(device.Family));
            }

            // Update all nodes that like this device
            Nodes.OfType<DeviceFilterNode>().ForEach(i => i.Add(device));
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            if(DesignMode)
            {
                return;
            }

            foreach(var client in ClientFactory.Instance)
            {
                client.DeviceDiscovered += OnDeviceChanged;
                client.PropertiesChanged += OnDeviceChanged;
                client.StateChanged += OnDeviceChanged;
            }

            base.OnHandleCreated(e);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            if(DesignMode)
            {
                return;
            }

            foreach(var client in ClientFactory.Instance)
            {
                client.DeviceDiscovered -= OnDeviceChanged;
                client.PropertiesChanged -= OnDeviceChanged;
                client.StateChanged -= OnDeviceChanged;
            }

            base.OnHandleDestroyed(e);
        }

        protected virtual void UpdateDeviceList()
        {
            ClientFactory.Instance.SelectMany(i => i.Devices.Append(i.Groups)).ForEach(AddDevice);
        }

        #region Event Handlers

        protected virtual void OnDeviceChanged(object sender, DeviceEventArgs e)
        {
            if(InvokeRequired)
            {
                BeginInvoke(new Action(() => OnDeviceChanged(sender, e)));
                return;
            }

            AddDevice(e.Device);
        }

        #endregion

        // Separates devices into categories
        public class DeviceCategoryNode : DeviceFilterNode
        {
            public readonly DevicesNode Devices = new();
            public readonly GroupsNode Groups = new();

            public DeviceCategoryNode(string text)
                : base(text)
            {
                Add(Nodes, Devices);
                Add(Nodes, Groups);
            }
        }

        // Represents a device family
        public class DeviceFamilyNode : DeviceCategoryNode
        {
            public DeviceFamilyNode(string familyName)
                : base(familyName)
            {
                Predicate = device => string.Equals(device.Family, familyName);
                HideDevices = true;
            }
        }

        // Recursively filters out devices that don't match the predicate 
        public class DeviceFilterNode : TreeNode
        {
            public DeviceFilterNode(string text)
                : base(text)
            {
            }

            public bool HideDevices { get; set; }

            public Func<IDevice, bool> Predicate { get; set; }

            public DeviceNode Add(IDevice device)
            {
                // This method will add or update the child nodes based on the state of the device.
                var match = Match(device);

                // If the predicate no longer matches, remove the device from this node and all children.
                if(!match)
                {
                    Remove(device);
                    return null;
                }

                // If the predicate does match, but the device name has changed, remove it and when
                // we re-add it, it will be sorted correctly.
                var node = DeviceNode.Find(Nodes, device);

                if(node is not null && !node.Text.Equals(device.Name))
                {
                    Remove(device);
                    node = null;
                }

                // Add the node if it doesn't exist and we're supposed to show devices
                if(!HideDevices)
                {
                    node ??= DeviceNode.Add(Nodes, device);
                }

                // Recurse into children
                Nodes.OfType<DeviceFilterNode>().ForEach(i => i.Add(device));

                return node;
            }

            public void Remove(IDevice device)
            {
                // Remove any nodes that contain this device
                DeviceNode.Remove(Nodes, device);

                // Recurse into children
                Nodes.OfType<DeviceFilterNode>().ForEach(i => i.Remove(device));
            }

            protected bool Match(IDevice device)
            {
                return Predicate is null || Predicate(device);
            }
        }

        // Represents an IDevice or IDeviceGroup
        public class DeviceNode : TreeNode
        {
            public DeviceNode(IDevice device)
                : base(device.Name)
            {
                Device = device;
            }

            public IDevice Device { get; }

            public static DeviceNode Add(TreeNodeCollection parent, IDevice device)
            {
                return Add(parent, new DeviceNode(device));
            }

            public static DeviceNode Find(TreeNodeCollection parent, IDevice device)
            {
                return Find<DeviceNode>(parent, i => i.Device.Equals(device));
            }

            public static IEnumerable<IDevice> GetAllChildDevices(TreeNodeCollection parent)
            {
                return GetAllChildNodes(parent).OfType<DeviceNode>().Select(i => i.Device);
            }

            public static void Remove(TreeNodeCollection parent, IDevice device)
            {
                Remove<DeviceNode>(parent, i => i.Device.Equals(device));
            }
        }

        // Represents a list of devices (not groups)
        public class DevicesNode : DeviceFilterNode
        {
            public DevicesNode(string text = "Devices")
                : base(text)
            {
                Predicate = device => device is not IDeviceGroup;
            }
        }

        // Represents a list of device groups
        public class GroupsNode : DeviceFilterNode
        {
            public GroupsNode(string text = "Groups")
                : base(text)
            {
                Predicate = device => device is IDeviceGroup;
            }
        }
    }
}
