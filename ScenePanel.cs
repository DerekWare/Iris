using System.Windows.Forms;
using DerekWare.HomeAutomation.Common.Scenes;

namespace DerekWare.Iris
{
    public partial class ScenePanel : UserControl
    {
        public ScenePanel(Scene scene)
        {
            InitializeComponent();

            Scene = scene;
        }

        public Scene Scene { get => SceneListView.Scene; set => SceneListView.Scene = value; }
    }
}
