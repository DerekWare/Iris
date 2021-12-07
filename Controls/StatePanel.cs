using System.Drawing;
using System.Windows.Forms;

namespace DerekWare.Iris
{
    public class StatePanel : UserControl
    {
        public readonly CheckGroupBox GroupBox;

        protected bool InUpdate;

        public StatePanel()
        {
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            GroupBox = new CheckGroupBox { Dock = DockStyle.Fill, Parent = this, Text = "" };
        }

        protected new bool DesignMode => Extensions.IsDesignMode();
    }
}
