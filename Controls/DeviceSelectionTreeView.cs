using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common;

namespace DerekWare.Iris
{
    class DeviceSelectionTreeView : DeviceTreeView
    {
        HashSet<IDevice> _CheckedDevices = new();
        bool InUpdate;

        public DeviceSelectionTreeView()
        {
        }

        [Browsable(false)]
        public IReadOnlyCollection<IDevice> CheckedDevices
        {
            get => _CheckedDevices;
            set
            {
                _CheckedDevices = new HashSet<IDevice>(value.SafeEmpty());
                UpdateCheckState();
                ExpandCheckedNodes();
            }
        }

        protected override void AddDevice(IDevice device)
        {
            base.AddDevice(device);
            UpdateCheckState();
            ExpandCheckedNodes();
        }

        protected virtual void ExpandCheckedNodes()
        {
            ExpandNodes(TreeNode.GetAllChildNodes(Nodes).Where(i => i.Checked));
        }

        protected virtual void ExpandNodes(IEnumerable<TreeNode> nodes)
        {
            foreach(var node in nodes)
            {
                var parent = node.Parent;

                while(null != parent)
                {
                    parent.Expand();
                    parent = parent.Parent;
                }
            }
        }

        protected override void OnAfterCheck(TreeViewEventArgs e)
        {
            if(InUpdate)
            {
                return;
            }

            if(e.Node.Checked)
            {
                _CheckedDevices.Add(((DeviceNode)e.Node).Device);
            }
            else
            {
                _CheckedDevices.Remove(((DeviceNode)e.Node).Device);
            }

            UpdateCheckState();
            base.OnAfterCheck(e);
        }

        protected override void OnBeforeCheck(TreeViewCancelEventArgs e)
        {
            if(e.Node is not DeviceNode)
            {
                e.Cancel = true;
            }

            base.OnBeforeCheck(e);
        }

        protected void UpdateCheckState()
        {
            InUpdate = true;
            UpdateCheckState(Nodes);
            InUpdate = false;
        }

        void UpdateCheckState(TreeNodeCollection nodes)
        {
            foreach(TreeNode node in nodes)
            {
                if(node is DeviceNode deviceNode)
                {
                    if(_CheckedDevices.Contains(deviceNode.Device))
                    {
                        node.Checked = true;
                        continue;
                    }
                }

                node.Checked = false;
            }

            foreach(TreeNode node in nodes)
            {
                UpdateCheckState(node.Nodes);
            }
        }
    }
}
