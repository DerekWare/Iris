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

            var state = TreeNode.Add(Nodes, new FilterNode("State") { HideDevices = true });

            TreeNode.Add(Nodes, new DeviceParentNode("All Devices"));
            TreeNode.Add(Nodes, new GroupParentNode("All Groups"));

            state.Add(new FilterNode("Effect Active") { Predicate = device => device.Effects.Any() });
            state.Add(new FilterNode("Power Off") { Predicate = device => device.Power == PowerState.Off });
            state.Add(new FilterNode("Power On") { Predicate = device => device.Power == PowerState.On });

            HueClient.Instance.DeviceDiscovered += OnDeviceChanged;
            HueClient.Instance.PropertiesChanged += OnDeviceChanged;
            HueClient.Instance.StateChanged += OnDeviceChanged;
            LifxClient.Instance.DeviceDiscovered += OnDeviceChanged;
            LifxClient.Instance.PropertiesChanged += OnDeviceChanged;
            LifxClient.Instance.StateChanged += OnDeviceChanged;
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
                var familyNode = TreeNode.Find<FamilyNode>(Nodes, e.Device.Family) ?? TreeNode.Add(Nodes, new FamilyNode(e.Device.Family));
            }

            // Update all nodes that like this device
            foreach(var child in Nodes.OfType<TreeNode>())
            {
                child.Add(e.Device);
            }
        }

        #endregion

        // Separates devices into categories
        public class CategoryNode : FilterNode
        {
            public readonly DeviceParentNode Devices = new();
            public readonly GroupParentNode Groups = new();

            public CategoryNode(string text)
                : base(text)
            {
                Add(Nodes, Devices);
                Add(Nodes, Groups);
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
        }

        // Represents a list of devices (not groups)
        public class DeviceParentNode : FilterNode
        {
            public DeviceParentNode(string text = "Devices")
                : base(text)
            {
                Predicate = device => device is not IDeviceGroup;
            }
        }

        // Represents a device family
        public class FamilyNode : CategoryNode
        {
            public FamilyNode(string familyName)
                : base(familyName)
            {
                Predicate = device => string.Equals(device.Family, familyName);
                HideDevices = true;
            }
        }

        // Recursively filters out devices that don't match the predicate 
        public class FilterNode : TreeNode
        {
            public FilterNode(string text)
                : base(text)
            {
            }

            public bool HideDevices { get; set; }

            public Func<IDevice, bool> Predicate { get; set; }

            public override DeviceNode Add(IDevice device)
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
                var node = Find(Nodes, device);

                if(node is not null && !node.Text.Equals(device.Name))
                {
                    Remove(device);
                    node = null;
                }

                // Add the node if it doesn't exist and we're supposed to show devices
                if(!HideDevices)
                {
                    node ??= base.Add(device);
                }

                // Recurse into children
                Nodes.OfType<FilterNode>().ForEach(i => i.Add(device));

                return node;
            }

            public override void Remove(IDevice device)
            {
                Nodes.OfType<FilterNode>().ForEach(i => i.Remove(device));
                base.Remove(device);
            }

            protected bool Match(IDevice device)
            {
                return Predicate is null || Predicate(device);
            }
        }

        // Represents a list of device groups
        public class GroupParentNode : FilterNode
        {
            public GroupParentNode(string text = "Groups")
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

            public virtual DeviceNode Add(IDevice device)
            {
                return Find(Nodes, device) ?? Add(Nodes, device);
            }

            public virtual void Remove(IDevice device)
            {
                var node = Find(Nodes, device);

                if(node is not null)
                {
                    Nodes.Remove(node);
                }
            }

            public static T Add<T>(TreeNodeCollection parent, T child)
                where T : TreeNode
            {
                parent.Insert(FindInsertionPoint(parent, child), child);
                return child;
            }

            public static DeviceNode Add(TreeNodeCollection parent, IDevice device)
            {
                return Add(parent, new DeviceNode(device));
            }

            public static T Find<T>(TreeNodeCollection parent, string text)
                where T : TreeNode
            {
                return parent.OfType<T>().FirstOrDefault(i => i.Text.Equals(text));
            }

            public static DeviceNode Find(TreeNodeCollection parent, IDevice device)
            {
                return device is null ? null : parent.OfType<DeviceNode>().FirstOrDefault(i => i.Device.Equals(device));
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
        }
    }
}
