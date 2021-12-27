using System;
using System.ComponentModel;
using System.Windows.Forms;
using DerekWare.HomeAutomation.Common.Effects;

namespace DerekWare.Iris
{
    public partial class EffectTabPanel : UserControl
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true)]
        public event EventHandler<PropertyChangedEventArgs<IReadOnlyEffectProperties>> SelectedObjectChanged
        {
            add => TabControl.SelectedObjectChanged += value;
            remove => TabControl.SelectedObjectChanged -= value;
        }

        public EffectTabPanel()
        {
            InitializeComponent();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public string DeviceFamily { get => TabControl.DeviceFamily; set => TabControl.DeviceFamily = value; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public IReadOnlyEffectProperties SelectedObject { get => TabControl.SelectedObject; set => TabControl.SelectedObject = value; }
    }
}
