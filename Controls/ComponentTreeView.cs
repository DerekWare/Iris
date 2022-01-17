using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common;
using DerekWare.HomeAutomation.Common.Scenes;
using DerekWare.Strings;

namespace DerekWare.Iris
{
    // Extends DeviceTreeView to include scenes
    class ComponentTreeView : DeviceTreeView
    {
        public readonly SceneCategoryNode ScenesNode = new();

        public ComponentTreeView()
        {
            LabelEdit = true;
            ShowFamilyNodes = true;
            ShowStateNodes = true;

            if(DesignMode)
            {
                return;
            }

            TreeNode.Add(Nodes, ScenesNode);
            SceneFactory.Instance.ForEach(i => SceneNode.Add(ScenesNode.Nodes, i));
        }

        [Browsable(false)]
        public Scene SelectedScene => (SelectedNode as SceneNode)?.Scene;

        [Browsable(false)]
        public new TreeNode SelectedNode { get => (TreeNode)base.SelectedNode; set => base.SelectedNode = value; }

        public bool Activate(TreeNode node)
        {
            if(node is not SceneNode sceneNode)
            {
                return false;
            }

            sceneNode.Scene.Apply();

            var sceneName = sceneNode.Scene.Name;
            var deviceNames = sceneNode.Scene.Items.Select(i => i.Name).Join(", ");
            var message = $"Scene \"{sceneName}\" applied to {deviceNames}";
            MessageBox.Show(message, ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);

            return true;

        }

        public void CreateScene(string name = "My Scene")
        {
            // Create the new scene, which will automatically update the treeview
            // TODO create a unique name
            var scene = SceneFactory.Instance.CreateInstance(name);

            // Find the node and select it
            SelectedNode = SceneNode.Find(ScenesNode.Nodes, scene);

            // Show the device selection dialog
            if(!SelectSceneDevices())
            {
                SceneFactory.Instance.Remove(scene);
                return;
            }

            // Rename the scene
            SelectedNode.BeginEdit();
        }

        public void RemoveSelectedScene()
        {
            if(SelectedNode is not SceneNode sceneNode)
            {
                return;
            }

            SelectedNode.EndEdit(true);
            SelectedNode = ScenesNode;
            SceneFactory.Instance.Remove(sceneNode.Scene);
        }

        public bool RenameScene()
        {
            if(!LabelEdit || SelectedNode is not SceneNode sceneNode)
            {
                return false;
            }

            sceneNode.BeginEdit();
            return true;
        }

        public bool SelectSceneDevices()
        {
            var scene = SelectedScene;

            if(scene is null)
            {
                return false;
            }

            IReadOnlyCollection<IDevice> devices = scene.Items.Select(i => i.Device).ToList();

            if(!DeviceSelectionDialog.Show(Parent, ref devices))
            {
                return false;
            }

            scene.AddRange(devices);
            scene.Items.RemoveWhere(i => !devices.Contains(i.Device));
            return true;
        }

        protected override void OnAfterLabelEdit(NodeLabelEditEventArgs e)
        {
            if(!e.CancelEdit && !e.Label.IsNullOrEmpty() && e.Node is SceneNode sceneNode)
            {
                sceneNode.Scene.Name = e.Label;
            }

            base.OnAfterLabelEdit(e);
        }

        protected override void OnBeforeLabelEdit(NodeLabelEditEventArgs e)
        {
            e.CancelEdit = e.Node is not SceneNode;
            base.OnBeforeLabelEdit(e);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            if(DesignMode)
            {
                return;
            }

            SceneFactory.Instance.CollectionChanged += OnSceneFactoryCollectionChanged;
            base.OnHandleCreated(e);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            if(DesignMode)
            {
                return;
            }

            SceneFactory.Instance.CollectionChanged -= OnSceneFactoryCollectionChanged;
            base.OnHandleDestroyed(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.Enter:
                    Activate(SelectedNode);
                    return;
            }

            base.OnKeyDown(e);
        }

        protected override void OnNodeMouseDoubleClick(TreeNodeMouseClickEventArgs e)
        {
            Activate((TreeNode)e.Node);
            base.OnNodeMouseDoubleClick(e);
        }

        #region Event Handlers

        void OnSceneFactoryCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(InvokeRequired)
            {
                BeginInvoke(new Action(() => OnSceneFactoryCollectionChanged(sender, e)));
                return;
            }

            e.OldItems.SafeEmpty().OfType<Scene>().ForEach(i => SceneNode.Remove(ScenesNode.Nodes, i));
            e.NewItems.SafeEmpty().OfType<Scene>().ForEach(i => SceneNode.Add(ScenesNode.Nodes, i));
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
