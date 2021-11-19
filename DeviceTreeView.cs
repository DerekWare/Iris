using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common;
using DerekWare.Strings;
using LifxClient = DerekWare.HomeAutomation.Lifx.Lan.Client;
using HueClient = DerekWare.HomeAutomation.PhilipsHue.Client;
using PowerState = DerekWare.HomeAutomation.Common.PowerState;

namespace DerekWare.Iris
{
    class DeviceTreeView : TreeView
    {
        readonly TreeNode Devices = new("Devices");
        readonly Dictionary<string, FamilyNode> Families = new();
        readonly TreeNode Groups = new("Groups");
        readonly TreeNode PowerOff = new("Power Off");
        readonly TreeNode PowerOn = new("Power On");
        readonly TreeNode EffectActive = new("Effect Active");

        public DeviceTreeView()
        {
            if(DesignMode)
            {
                return;
            }

            TreeNode.Add(Nodes, Groups);
            TreeNode.Add(Nodes, Devices);
            TreeNode.Add(Nodes, PowerOn);
            TreeNode.Add(Nodes, PowerOff);
            TreeNode.Add(Nodes, EffectActive);

            HueClient.Instance.DeviceDiscovered += OnDeviceChanged;
            HueClient.Instance.PropertiesChanged += OnDeviceChanged;
            HueClient.Instance.StateChanged += OnDeviceChanged;
            LifxClient.Instance.DeviceDiscovered += OnDeviceChanged;
            LifxClient.Instance.PropertiesChanged += OnDeviceChanged;
            LifxClient.Instance.StateChanged += OnDeviceChanged;
        }

        void Update(IDevice device)
        {
            if(InvokeRequired)
            {
                BeginInvoke(new Action(() => Update(device)));
                return;
            }

            DeviceNode.Add(device is IDeviceGroup ? Groups.Nodes : Devices.Nodes, device);

            if(!device.Family.IsNullOrEmpty())
            {
                if(!Families.TryGetValue(device.Family, out var familyNode))
                {
                    familyNode = new FamilyNode(device.Family);
                    Families.Add(device.Family, familyNode);
                    TreeNode.Add(Nodes, familyNode);
                }

                DeviceNode.Add(device is IDeviceGroup ? familyNode.Groups.Nodes : familyNode.Devices.Nodes, device);
            }

            if(device.Power == PowerState.On)
            {
                foreach(var i in PowerOff.Nodes.OfType<DeviceNode>().ToList().Where(i => Equals(i.Device, device)))
                {
                    PowerOff.Nodes.Remove(i);
                }

                DeviceNode.Add(PowerOn.Nodes, device);
            }
            else
            {
                foreach(var i in PowerOn.Nodes.OfType<DeviceNode>().ToList().Where(i => Equals(i.Device, device)))
                {
                    PowerOff.Nodes.Remove(i);
                }

                DeviceNode.Add(PowerOff.Nodes, device);
            }

            if(device.Effects.IsNullOrEmpty())
            {
                foreach(var i in EffectActive.Nodes.OfType<DeviceNode>().ToList().Where(i => Equals(i.Device, device)))
                {
                    EffectActive.Nodes.Remove(i);
                }
            }
            else
            {
                DeviceNode.Add(EffectActive.Nodes, device);
            }
        }

        #region Event Handlers

        void OnDeviceChanged(object sender, DeviceEventArgs e)
        {
            Update(e.Device);
        }

        #endregion

        public class DeviceNode : TreeNode
        {
            public DeviceNode(IDevice device)
                : base(device.Name)
            {
                Device = device;
            }

            public IDevice Device { get; }

            public static void Add(TreeNodeCollection parent, IDevice device)
            {
                if(device is IDeviceGroup group)
                {
                    Add<GroupNode>(parent, group);
                }
                else
                {
                    Add<DeviceNode>(parent, device);
                }
            }
        }

        public class FamilyNode : TreeNode
        {
            public readonly TreeNode Devices = new("Devices");
            public readonly TreeNode Groups = new("Groups");

            public FamilyNode(string name)
                : base(name)
            {
                Add(Nodes, Devices);
                Add(Nodes, Groups);
            }
        }

        public class GroupNode : DeviceNode
        {
            public GroupNode(IDeviceGroup group)
                : base(group)
            {
                Group = group;
            }

            public IDeviceGroup Group { get; }

            public static void Add(TreeNodeCollection parent, IDeviceGroup device)
            {
                Add<GroupNode>(parent, device);
            }
        }

        public class TreeNode : System.Windows.Forms.TreeNode
        {
            public TreeNode(string name)
            {
                Text = name;
            }

            public void Populate<T>(IEnumerable<IDevice> devices)
                where T : DeviceNode
            {
                devices.ForEach(i => Add<T>(Nodes, i));
            }

            public static TreeNode Add(TreeNodeCollection parent, TreeNode child)
            {
                var index = 0;

                foreach(TreeNode i in parent)
                {
                    if(child.Text.CompareTo(i.Text) < 0)
                    {
                        break;
                    }

                    ++index;
                }

                parent.Insert(index, child);
                return child;
            }

            public static T Add<T>(TreeNodeCollection parent, IDevice device)
                where T : DeviceNode
            {
                var child = Find<T>(parent, device);

                if(child is null)
                {
                    child = (T)Add(parent, (T)Activator.CreateInstance(typeof(T), device));
                }
                else
                {
                    child.Text = device.Name;
                }

                return child;
            }

            public static T Find<T>(TreeNodeCollection parent, string text)
                where T : TreeNode
            {
                return parent.OfType<T>().FirstOrDefault(i => i.Text.Equals(text));
            }

            public static T Find<T>(TreeNodeCollection parent, IDevice device)
                where T : DeviceNode
            {
                return parent.OfType<T>().FirstOrDefault(i => i.Device.Equals(device));
            }
        }
    }
}
