using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Forms;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common.Scenes;

namespace DerekWare.Iris
{
    // Extends DeviceTreeView to include scenes
    class ComponentTreeView : DeviceTreeView
    {
        public readonly SceneCategoryNode Scenes = new();

        public ComponentTreeView()
        {
            if(DesignMode)
            {
                return;
            }

            LabelEdit = true;

            TreeNode.Add(Nodes, Scenes);
            SceneFactory.Instance.ForEach(i => SceneNode.Add(Scenes.Nodes, i));
            SceneFactory.Instance.CollectionChanged += OnSceneFactoryCollectionChanged;
        }

        public Scene SelectedScene => (SelectedNode as SceneNode)?.Scene;

        public void CreateScene(string name = "My Scene")
        {
            // Create the new scene, which will automatically update the treeview
            // TODO create a unique name
            var scene = SceneFactory.Instance.CreateInstance(name);

            // Find the treenode and select it
            SelectedNode = SceneNode.Find(Scenes.Nodes, scene);

            // Start the rename
            SelectedNode.BeginEdit();
        }

        public void RemoveSelectedScene()
        {
            if(SelectedNode is not SceneNode sceneNode)
            {
                return;
            }

            SelectedNode.EndEdit(true);
            SelectedNode = Scenes;
            SceneFactory.Instance.Remove(sceneNode.Scene);
        }

        protected override void OnAfterCheck(TreeViewEventArgs e)
        {
            if(SelectedNode is SceneNode sceneNode && e.Node is DeviceNode deviceNode)
            {
                if(e.Node.Checked)
                {
                    sceneNode.Scene.Add(deviceNode.Device);
                }
                else
                {
                    sceneNode.Scene.Remove(deviceNode.Device);
                }
            }

            base.OnAfterCheck(e);
        }

        protected override void OnAfterLabelEdit(NodeLabelEditEventArgs e)
        {
            if(!e.CancelEdit && !e.Label.IsNullOrEmpty() && e.Node is SceneNode sceneNode)
            {
                sceneNode.Scene.Name = e.Label;
            }

            base.OnAfterLabelEdit(e);
        }

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            CheckBoxes = e.Node is SceneNode;
            SetCheckState(Nodes);
            base.OnAfterSelect(e);
        }

        protected override void OnBeforeCheck(TreeViewCancelEventArgs e)
        {
            e.Cancel = e.Node is not DeviceNode;
            base.OnBeforeCheck(e);
        }

        protected override void OnBeforeLabelEdit(NodeLabelEditEventArgs e)
        {
            e.CancelEdit = e.Node is not SceneNode;
            base.OnBeforeLabelEdit(e);
        }

        protected void SetCheckState(TreeNode node)
        {
            node.Checked = node is DeviceNode deviceNode && (SelectedScene?.Contains(deviceNode.Device) ?? false);
            SetCheckState(node.Nodes);
        }

        protected void SetCheckState(TreeNodeCollection nodes)
        {
            nodes.ForEach<TreeNode>(SetCheckState);
        }

        #region Event Handlers

        void OnSceneFactoryCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(InvokeRequired)
            {
                BeginInvoke(new Action(() => OnSceneFactoryCollectionChanged(sender, e)));
                return;
            }

            e.OldItems.SafeEmpty().OfType<Scene>().ForEach(i => SceneNode.Remove(Scenes.Nodes, i));
            e.NewItems.SafeEmpty().OfType<Scene>().ForEach(i => SceneNode.Add(Scenes.Nodes, i));
        }

        #endregion

        // Represents the scene category
        public class SceneCategoryNode : TreeNode
        {
            public SceneCategoryNode(string text = "Scenes")
                : base(text)
            {
            }
        }

        // Represents a Scene
        public class SceneNode : TreeNode
        {
            public SceneNode(Scene scene)
                : base(scene.Name)
            {
                Scene = scene;
            }

            public Scene Scene { get; }

            public static SceneNode Add(TreeNodeCollection parent, Scene scene)
            {
                return Add(parent, new SceneNode(scene));
            }

            public static SceneNode Find(TreeNodeCollection parent, Scene scene)
            {
                return Find<SceneNode>(parent, i => i.Scene.Equals(scene));
            }

            public static void Remove(TreeNodeCollection parent, Scene scene)
            {
                Remove<SceneNode>(parent, i => i.Scene.Equals(scene));
            }
        }
    }
}
