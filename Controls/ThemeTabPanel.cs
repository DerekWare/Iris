using System;
using System.ComponentModel;
using System.Windows.Forms;
using DerekWare.HomeAutomation.Common.Themes;

namespace DerekWare.Iris
{
    public partial class ThemeTabPanel : UserControl
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true)]
        public event EventHandler<PropertyChangedEventArgs<IReadOnlyThemeProperties>> SelectedObjectChanged
        {
            add => TabControl.SelectedObjectChanged += value;
            remove => TabControl.SelectedObjectChanged -= value;
        }

        public ThemeTabPanel()
        {
            InitializeComponent();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public string DeviceFamily { get => TabControl.DeviceFamily; set => TabControl.DeviceFamily = value; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public IReadOnlyThemeProperties SelectedObject { get => TabControl.SelectedObject; set => TabControl.SelectedObject = value; }
    }
}
