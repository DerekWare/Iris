using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common.Scenes;

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
            TabControl.TabPages.AddRange(Scene.Items.Select(i => new SceneTabPage(i)).ToArray<TabPage>());

            Scene.Items.CollectionChanged += OnCollectionChanged;
        }

        [Browsable(false)]
        public Scene Scene { get; }

        protected new bool DesignMode => Extensions.IsDesignMode();

        public void SnapshotActiveScene()
        {
            (TabControl.SelectedTab as SceneTabPage)?.SceneItem?.Snapshot();
        }

        #region Event Handlers

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach(SceneItem i in e.OldItems.SafeEmpty())
            {
                TabControl.TabPages.OfType<SceneTabPage>().Where(j => Equals(i, j.SceneItem)).ForEach(j => j.Dispose());
            }

            TabControl.TabPages.AddRange(e.NewItems.SafeEmpty().Cast<SceneItem>().Select(i => new SceneTabPage(i)).ToArray<TabPage>());
        }

        #endregion
    }
}
