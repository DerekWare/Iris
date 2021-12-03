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

        public void SnapshotActiveScene()
        {
            (TabControl.SelectedTab as SceneTabPage)?.SceneItem?.SnapshotDeviceState();
        }

        void UpdateState()
        {
            if(Scene.Items.IsNullOrEmpty())
            {
                TabControl.Visible = false;
                DescriptionLabel.Visible = true;
                TabControl.TabPages.OfType<SceneTabPage>().ToList().ForEach(i => i.Dispose());
            }
            else
            {
                TabControl.TabPages.OfType<SceneTabPage>().Where(i => !Scene.Items.Contains(i.SceneItem)).ToList().ForEach(i => i.Dispose());
                TabControl.TabPages.AddRange(Scene.Items.Select(i => new SceneTabPage(i)).ToArray<TabPage>());
                TabControl.Visible = true;
                DescriptionLabel.Visible = false;
            }
        }

        #region Event Handlers

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateState();
        }

        #endregion
    }
}
