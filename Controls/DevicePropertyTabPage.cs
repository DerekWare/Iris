using System;
using System.ComponentModel;
using System.Windows.Forms;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common;

namespace DerekWare.Iris.Controls
{
    public class DevicePropertyTabPage<T> : TabPage
        where T : IName, IFamily, IDescription
    {
        readonly Button ApplyButton = new() { Text = "Apply", Dock = DockStyle.Fill };
        readonly TableLayoutPanel LayoutPanel = new() { Dock = DockStyle.Fill };
        readonly PropertyGrid PropertyGrid = new() { Dock = DockStyle.Fill };

        public event EventHandler ApplyClicked { add => ApplyButton.Click += value; remove => ApplyButton.Click -= value; }

        public DevicePropertyTabPage(T obj)
        {
            SelectedObject = obj;
            Text = SelectedObject.Name;

            LayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            LayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));

            LayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            LayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));

            Controls.Add(LayoutPanel);

            PropertyGrid.SelectedObject = SelectedObject;
            LayoutPanel.Controls.Add(PropertyGrid);
            LayoutPanel.SetColumn(PropertyGrid, 0);
            LayoutPanel.SetRow(PropertyGrid, 0);
            LayoutPanel.SetColumnSpan(PropertyGrid, LayoutPanel.ColumnStyles.Count);

            LayoutPanel.Controls.Add(ApplyButton);
            LayoutPanel.SetColumn(ApplyButton, LayoutPanel.ColumnStyles.Count - 1);
            LayoutPanel.SetRow(ApplyButton, LayoutPanel.RowStyles.Count - 1);

            if(!SelectedObject.Description.IsNullOrEmpty())
            {
                new ToolTip().SetToolTip(this, SelectedObject.Description);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public T SelectedObject { get; }
    }
}
