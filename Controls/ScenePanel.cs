using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common.Scenes;
using DerekWare.Iris.Properties;

namespace DerekWare.Iris
{
    public partial class ScenePanel : UserControl
    {
        public ScenePanel(Scene scene)
        {
            InitializeComponent();

            if(DesignMode)
            {
                return;
            }

            Scene = scene;
            DescriptionLabel.Text = Resources.EmptySceneDescription;

            UpdateState();

            Scene.Items.CollectionChanged += OnCollectionChanged;
        }

        [Browsable(false)]
        public Scene Scene { get; }

        protected new bool DesignMode => Extensions.IsDesignMode();

        public SceneItem SceneItemFromTabPage(SceneTabPage page)
        {
            return page?.SceneItem;
        }

        public void SnapshotActiveScene()
        {
            (TabControl.SelectedTab as SceneTabPage)?.SceneItem?.SnapshotDeviceState();
        }

        public SceneTabPage TabPageFromSceneItem(SceneItem sceneItem)
        {
            return TabControl.TabPages.OfType<SceneTabPage>().WhereEquals(i => i.SceneItem, sceneItem).FirstOrDefault();
        }

        void UpdateState()
        {
            TabControl.TabPages.RemoveWhere<SceneTabPage>(i => !Scene.Items.Contains(SceneItemFromTabPage(i)));
            TabControl.TabPages.AddRange(Scene.Items.WhereNull(TabPageFromSceneItem).Select(i => new SceneTabPage(i)).ToArray<TabPage>());
            TabControl.Visible = !TabControl.TabPages.OfType<SceneTabPage>().IsNullOrEmpty();
            DescriptionLabel.Visible = !TabControl.Visible;
        }

        #region Event Handlers

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateState();
        }

        #endregion
    }
}
