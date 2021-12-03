using System.Windows.Forms;
using DerekWare.HomeAutomation.Common.Scenes;
using DerekWare.Iris.Properties;

namespace DerekWare.Iris
{
    public partial class SceneListViewPanel : UserControl
    {
        public SceneListViewPanel(Scene scene)
        {
            InitializeComponent();

            Scene = scene;
            DescriptionLabel.Text = Resources.ScenePanelDescription;
        }

        public Scene Scene { get => SceneListView.Scene; set => SceneListView.Scene = value; }
    }
}
