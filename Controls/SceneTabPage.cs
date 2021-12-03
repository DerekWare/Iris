using System.Windows.Forms;
using DerekWare.HomeAutomation.Common.Scenes;
using DerekWare.Iris.Properties;

namespace DerekWare.Iris
{
    public class SceneTabPage : TabPage
    {
        public SceneTabPage(SceneItem sceneItem)
        {
            // TODO update name when the device is found
            SceneItem = sceneItem;
            Text = SceneItem.Name;
            Panel = new SceneItemPanel(SceneItem) { Dock = DockStyle.Fill, Description = Resources.ScenePanelDescription };
            Controls.Add(Panel);
        }

        public SceneItemPanel Panel { get; }

        public SceneItem SceneItem { get; }
    }
}
