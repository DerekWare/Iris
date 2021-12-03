using System.Windows.Forms;
using DerekWare.HomeAutomation.Common.Scenes;
using DerekWare.Iris.Properties;

namespace DerekWare.Iris
{
    public class SceneTabPage : TabPage
    {
        public SceneTabPage(SceneItem sceneItem)
        {
            SceneItem = sceneItem;
            Text = SceneItem.Device?.Name ?? SceneItem.Uuid;
            ActionPanel = new DeviceActionPanel(SceneItem) { Dock = DockStyle.Fill, Description = Resources.ScenePanelDescription };
            Controls.Add(ActionPanel);

            // TODO register for client and device discovery?
        }

        public DeviceActionPanel ActionPanel { get; }

        public SceneItem SceneItem { get; }
    }
}
