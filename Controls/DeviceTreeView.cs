using System;
using System.Linq;
using System.Windows.Forms;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common;
using LifxClient = DerekWare.HomeAutomation.Lifx.Lan.Client;
using HueClient = DerekWare.HomeAutomation.PhilipsHue.Client;
using PowerState = DerekWare.HomeAutomation.Common.PowerState;

namespace DerekWare.Iris
{
    class DeviceTreeView : TreeView
    {
        public DeviceTreeView()
        {
            if(DesignMode)
            {
                return;
            }

            TreeNode.Add(Nodes, new DevicesNode("All Devices"));
            TreeNode.Add(Nodes, new GroupsNode("All Groups"));

            var state = TreeNode.Add(Nodes, new DeviceFilterNode("State") { HideDevices = true });
            state.Add(new DeviceFilterNode("Effect Active") { Predicate = device => device.Effect is not null });
            state.Add(new DeviceFilterNode("Power Off") { Predicate = device => device.Power == PowerState.Off });
            state.Add(new DeviceFilterNode("Power On") { Predicate = device => device.Power == PowerState.On });
        }

        protected new bool DesignMode => Extensions.IsDesignMode();

        protected override void OnHandleCreated(EventArgs e)
        {
            HueClient.Instance.DeviceDiscovered += OnDeviceChanged;
            HueClient.Instance.PropertiesChanged += OnDeviceChanged;
            HueClient.Instance.StateChanged += OnDeviceChanged;
            LifxClient.Instance.DeviceDiscovered += OnDeviceChanged;
            LifxClient.Instance.PropertiesChanged += OnDeviceChanged;
            LifxClient.Instance.StateChanged += OnDeviceChanged;

            base.OnHandleCreated(e);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            HueClient.Instance.DeviceDiscovered -= OnDeviceChanged;
            HueClient.Instance.PropertiesChanged -= OnDeviceChanged;
            HueClient.Instance.StateChanged -= OnDeviceChanged;
            LifxClient.Instance.DeviceDiscovered -= OnDeviceChanged;
            LifxClient.Instance.PropertiesChanged -= OnDeviceChanged;
            LifxClient.Instance.StateChanged -= OnDeviceChanged;

            base.OnHandleDestroyed(e);
        }

        #region Event Handlers

        void OnDeviceChanged(object sender, DeviceEventArgs e)
        {
            if(InvokeRequired)
            {
                BeginInvoke(new Action(() => OnDeviceChanged(sender, e)));
                return;
            }

            // Add device family nodes
            if(!e.Device.Family.IsNullOrEmpty())
            {
                var familyNode = TreeNode.Find<DeviceFamilyNode>(Nodes, e.Device.Family) ?? TreeNode.Add(Nodes, new DeviceFamilyNode(e.Device.Family));
            }

            // Update all nodes that like this device
            Nodes.OfType<DeviceFilterNode>().ForEach(i => i.Add(e.Device));
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

        // Intelligently adds or updates nodes using the correct node type and automatically sorted
        public class TreeNode : System.Windows.Forms.TreeNode
        {
            public TreeNode(string text)
            {
                Text = text;
            }

            public virtual T Add<T>(T child)
                where T : TreeNode
            {
                return Add(Nodes, child);
            }

            public static T Add<T>(TreeNodeCollection parent, T child)
                where T : TreeNode
            {
                parent.Insert(FindInsertionPoint(parent, child), child);
                return child;
            }

            public static T Find<T>(TreeNodeCollection parent, string text)
                where T : TreeNode
            {
                return Find<T>(parent, i => i.Text.Equals(text));
            }

            public static T Find<T>(TreeNodeCollection parent, Func<T, bool> wherePredicate)
                where T : TreeNode
            {
                return parent.OfType<T>().FirstOrDefault(wherePredicate);
            }

            public static int FindInsertionPoint(TreeNodeCollection parent, TreeNode child)
            {
                var index = 0;

                foreach(TreeNode i in parent)
                {
                    if(string.Compare(child.Text, i.Text, StringComparison.CurrentCulture) < 0)
                    {
                        break;
                    }

                    ++index;
                }

                return index;
            }

            public static void Remove<T>(TreeNodeCollection parent, Func<T, bool> wherePredicate)
                where T : TreeNode
            {
                parent.OfType<T>().Where(wherePredicate).ToList().ForEach(parent.Remove);
            }
        }
    }
}
