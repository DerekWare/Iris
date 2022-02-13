using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using DerekWare.HomeAutomation.Common;

namespace DerekWare.Iris
{
    public partial class MultiZoneColorPanel : UserControl
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true)]
        public event EventHandler<ColorsChangedEventArgs> ColorsChanged { add => ColorBand.ColorsChanged += value; remove => ColorBand.ColorsChanged += value; }

        public MultiZoneColorPanel()
        {
            InitializeComponent();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public IReadOnlyCollection<Color> Colors { get => ColorBand.Colors; set => ColorBand.Colors = value; }
    }
}
