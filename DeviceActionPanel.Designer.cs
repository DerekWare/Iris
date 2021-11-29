
using DerekWare.HomeAutomation.Common.Colors;

namespace DerekWare.Iris
{
    partial class DeviceActionPanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if(disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.BaseLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.PropertiesGroupBox = new System.Windows.Forms.GroupBox();
            this.PropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.StateLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.ZoneColorGroupBox = new System.Windows.Forms.GroupBox();
            this.ZoneColorBand = new DerekWare.Iris.ColorBand();
            this.EffectGroupBox = new System.Windows.Forms.GroupBox();
            this.EffectLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.ThemeGroupBox = new System.Windows.Forms.GroupBox();
            this.ThemeLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.SolidColorGroupBox = new System.Windows.Forms.GroupBox();
            this.SolidColorPanel = new DerekWare.Iris.SolidColorPanel();
            this.PowerGroupBox = new System.Windows.Forms.GroupBox();
            this.PowerComboBox = new System.Windows.Forms.ComboBox();
            this.DescriptionLabel = new System.Windows.Forms.Label();
            this.BaseLayoutPanel.SuspendLayout();
            this.PropertiesGroupBox.SuspendLayout();
            this.StateLayoutPanel.SuspendLayout();
            this.ZoneColorGroupBox.SuspendLayout();
            this.EffectGroupBox.SuspendLayout();
            this.ThemeGroupBox.SuspendLayout();
            this.SolidColorGroupBox.SuspendLayout();
            this.PowerGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // BaseLayoutPanel
            // 
            this.BaseLayoutPanel.ColumnCount = 2;
            this.BaseLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.BaseLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.BaseLayoutPanel.Controls.Add(this.PropertiesGroupBox, 1, 1);
            this.BaseLayoutPanel.Controls.Add(this.StateLayoutPanel, 0, 1);
            this.BaseLayoutPanel.Controls.Add(this.DescriptionLabel, 0, 0);
            this.BaseLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BaseLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.BaseLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.BaseLayoutPanel.Name = "BaseLayoutPanel";
            this.BaseLayoutPanel.RowCount = 2;
            this.BaseLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.BaseLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.BaseLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.BaseLayoutPanel.Size = new System.Drawing.Size(1472, 737);
            this.BaseLayoutPanel.TabIndex = 5;
            // 
            // PropertiesGroupBox
            // 
            this.PropertiesGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PropertiesGroupBox.Controls.Add(this.PropertyGrid);
            this.PropertiesGroupBox.Location = new System.Drawing.Point(744, 28);
            this.PropertiesGroupBox.Margin = new System.Windows.Forms.Padding(8);
            this.PropertiesGroupBox.Name = "PropertiesGroupBox";
            this.PropertiesGroupBox.Padding = new System.Windows.Forms.Padding(8);
            this.PropertiesGroupBox.Size = new System.Drawing.Size(720, 701);
            this.PropertiesGroupBox.TabIndex = 5;
            this.PropertiesGroupBox.TabStop = false;
            this.PropertiesGroupBox.Text = "Properties";
            // 
            // PropertyGrid
            // 
            this.PropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PropertyGrid.Location = new System.Drawing.Point(8, 27);
            this.PropertyGrid.Margin = new System.Windows.Forms.Padding(8);
            this.PropertyGrid.Name = "PropertyGrid";
            this.PropertyGrid.Size = new System.Drawing.Size(704, 666);
            this.PropertyGrid.TabIndex = 0;
            // 
            // StateLayoutPanel
            // 
            this.StateLayoutPanel.ColumnCount = 1;
            this.StateLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.StateLayoutPanel.Controls.Add(this.ZoneColorGroupBox, 0, 2);
            this.StateLayoutPanel.Controls.Add(this.EffectGroupBox, 0, 4);
            this.StateLayoutPanel.Controls.Add(this.ThemeGroupBox, 0, 3);
            this.StateLayoutPanel.Controls.Add(this.SolidColorGroupBox, 0, 1);
            this.StateLayoutPanel.Controls.Add(this.PowerGroupBox, 0, 0);
            this.StateLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.StateLayoutPanel.Location = new System.Drawing.Point(0, 20);
            this.StateLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.StateLayoutPanel.Name = "StateLayoutPanel";
            this.StateLayoutPanel.RowCount = 5;
            this.StateLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.StateLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.StateLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.StateLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.StateLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.StateLayoutPanel.Size = new System.Drawing.Size(736, 717);
            this.StateLayoutPanel.TabIndex = 8;
            // 
            // ZoneColorGroupBox
            // 
            this.ZoneColorGroupBox.Controls.Add(this.ZoneColorBand);
            this.ZoneColorGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ZoneColorGroupBox.Location = new System.Drawing.Point(8, 267);
            this.ZoneColorGroupBox.Margin = new System.Windows.Forms.Padding(8);
            this.ZoneColorGroupBox.Name = "ZoneColorGroupBox";
            this.ZoneColorGroupBox.Padding = new System.Windows.Forms.Padding(8);
            this.ZoneColorGroupBox.Size = new System.Drawing.Size(720, 75);
            this.ZoneColorGroupBox.TabIndex = 12;
            this.ZoneColorGroupBox.TabStop = false;
            this.ZoneColorGroupBox.Text = "Zone Colors";
            // 
            // ZoneColorBand
            // 
            this.ZoneColorBand.Colors = new DerekWare.HomeAutomation.Common.Colors.Color[0];
            this.ZoneColorBand.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ZoneColorBand.Location = new System.Drawing.Point(8, 27);
            this.ZoneColorBand.Margin = new System.Windows.Forms.Padding(8);
            this.ZoneColorBand.Name = "ZoneColorBand";
            this.ZoneColorBand.Size = new System.Drawing.Size(704, 40);
            this.ZoneColorBand.TabIndex = 0;
            this.ZoneColorBand.ColorsChanged += new System.EventHandler<DerekWare.Iris.ColorsChangedEventArgs>(this.ZoneColorBand_ColorsChanged);
            // 
            // EffectGroupBox
            // 
            this.EffectGroupBox.Controls.Add(this.EffectLayoutPanel);
            this.EffectGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EffectGroupBox.Location = new System.Drawing.Point(8, 541);
            this.EffectGroupBox.Margin = new System.Windows.Forms.Padding(8);
            this.EffectGroupBox.Name = "EffectGroupBox";
            this.EffectGroupBox.Padding = new System.Windows.Forms.Padding(8);
            this.EffectGroupBox.Size = new System.Drawing.Size(720, 168);
            this.EffectGroupBox.TabIndex = 11;
            this.EffectGroupBox.TabStop = false;
            this.EffectGroupBox.Text = "Effects";
            // 
            // EffectLayoutPanel
            // 
            this.EffectLayoutPanel.ColumnCount = 4;
            this.EffectLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.EffectLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.EffectLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.EffectLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.EffectLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EffectLayoutPanel.Location = new System.Drawing.Point(8, 27);
            this.EffectLayoutPanel.Name = "EffectLayoutPanel";
            this.EffectLayoutPanel.RowCount = 4;
            this.EffectLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.EffectLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.EffectLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.EffectLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.EffectLayoutPanel.Size = new System.Drawing.Size(704, 133);
            this.EffectLayoutPanel.TabIndex = 0;
            // 
            // ThemeGroupBox
            // 
            this.ThemeGroupBox.Controls.Add(this.ThemeLayoutPanel);
            this.ThemeGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ThemeGroupBox.Location = new System.Drawing.Point(8, 358);
            this.ThemeGroupBox.Margin = new System.Windows.Forms.Padding(8);
            this.ThemeGroupBox.Name = "ThemeGroupBox";
            this.ThemeGroupBox.Padding = new System.Windows.Forms.Padding(8);
            this.ThemeGroupBox.Size = new System.Drawing.Size(720, 167);
            this.ThemeGroupBox.TabIndex = 10;
            this.ThemeGroupBox.TabStop = false;
            this.ThemeGroupBox.Text = "Themes";
            // 
            // ThemeLayoutPanel
            // 
            this.ThemeLayoutPanel.ColumnCount = 4;
            this.ThemeLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.ThemeLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.ThemeLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.ThemeLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.ThemeLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ThemeLayoutPanel.Location = new System.Drawing.Point(8, 27);
            this.ThemeLayoutPanel.Name = "ThemeLayoutPanel";
            this.ThemeLayoutPanel.RowCount = 4;
            this.ThemeLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.ThemeLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.ThemeLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.ThemeLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.ThemeLayoutPanel.Size = new System.Drawing.Size(704, 132);
            this.ThemeLayoutPanel.TabIndex = 0;
            // 
            // SolidColorGroupBox
            // 
            this.SolidColorGroupBox.Controls.Add(this.SolidColorPanel);
            this.SolidColorGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SolidColorGroupBox.Location = new System.Drawing.Point(8, 87);
            this.SolidColorGroupBox.Margin = new System.Windows.Forms.Padding(8);
            this.SolidColorGroupBox.Name = "SolidColorGroupBox";
            this.SolidColorGroupBox.Padding = new System.Windows.Forms.Padding(8);
            this.SolidColorGroupBox.Size = new System.Drawing.Size(720, 164);
            this.SolidColorGroupBox.TabIndex = 9;
            this.SolidColorGroupBox.TabStop = false;
            this.SolidColorGroupBox.Text = "Solid Color";
            // 
            // SolidColorPanel
            // 
            this.SolidColorPanel.Color = null;
            this.SolidColorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SolidColorPanel.Location = new System.Drawing.Point(8, 27);
            this.SolidColorPanel.Name = "SolidColorPanel";
            this.SolidColorPanel.Size = new System.Drawing.Size(704, 129);
            this.SolidColorPanel.TabIndex = 0;
            this.SolidColorPanel.ColorChanged += new System.EventHandler<DerekWare.Iris.ColorChangedEventArgs>(this.SolidColorPanel_ColorChanged);
            // 
            // PowerGroupBox
            // 
            this.PowerGroupBox.AutoSize = true;
            this.PowerGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.PowerGroupBox.Controls.Add(this.PowerComboBox);
            this.PowerGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PowerGroupBox.Location = new System.Drawing.Point(8, 8);
            this.PowerGroupBox.Margin = new System.Windows.Forms.Padding(8);
            this.PowerGroupBox.Name = "PowerGroupBox";
            this.PowerGroupBox.Padding = new System.Windows.Forms.Padding(8);
            this.PowerGroupBox.Size = new System.Drawing.Size(720, 63);
            this.PowerGroupBox.TabIndex = 8;
            this.PowerGroupBox.TabStop = false;
            this.PowerGroupBox.Text = "Power";
            // 
            // PowerComboBox
            // 
            this.PowerComboBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.PowerComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PowerComboBox.FormattingEnabled = true;
            this.PowerComboBox.Items.AddRange(new object[] {
            "Off",
            "On"});
            this.PowerComboBox.Location = new System.Drawing.Point(8, 27);
            this.PowerComboBox.Margin = new System.Windows.Forms.Padding(8);
            this.PowerComboBox.Name = "PowerComboBox";
            this.PowerComboBox.Size = new System.Drawing.Size(704, 28);
            this.PowerComboBox.TabIndex = 3;
            this.PowerComboBox.SelectedIndexChanged += new System.EventHandler(this.PowerComboBox_SelectedIndexChanged);
            // 
            // DescriptionLabel
            // 
            this.DescriptionLabel.AutoSize = true;
            this.BaseLayoutPanel.SetColumnSpan(this.DescriptionLabel, 2);
            this.DescriptionLabel.Location = new System.Drawing.Point(3, 0);
            this.DescriptionLabel.Name = "DescriptionLabel";
            this.DescriptionLabel.Size = new System.Drawing.Size(0, 20);
            this.DescriptionLabel.TabIndex = 0;
            this.DescriptionLabel.Visible = false;
            // 
            // DeviceActionPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.BaseLayoutPanel);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "DeviceActionPanel";
            this.Size = new System.Drawing.Size(1472, 737);
            this.BaseLayoutPanel.ResumeLayout(false);
            this.BaseLayoutPanel.PerformLayout();
            this.PropertiesGroupBox.ResumeLayout(false);
            this.StateLayoutPanel.ResumeLayout(false);
            this.StateLayoutPanel.PerformLayout();
            this.ZoneColorGroupBox.ResumeLayout(false);
            this.EffectGroupBox.ResumeLayout(false);
            this.ThemeGroupBox.ResumeLayout(false);
            this.SolidColorGroupBox.ResumeLayout(false);
            this.PowerGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel BaseLayoutPanel;
        private System.Windows.Forms.GroupBox PropertiesGroupBox;
        private System.Windows.Forms.PropertyGrid PropertyGrid;
        private System.Windows.Forms.TableLayoutPanel StateLayoutPanel;
        private System.Windows.Forms.GroupBox ThemeGroupBox;
        private System.Windows.Forms.GroupBox SolidColorGroupBox;
        private System.Windows.Forms.GroupBox PowerGroupBox;
        private System.Windows.Forms.ComboBox PowerComboBox;
        private System.Windows.Forms.TableLayoutPanel ThemeLayoutPanel;
        private System.Windows.Forms.GroupBox EffectGroupBox;
        private System.Windows.Forms.TableLayoutPanel EffectLayoutPanel;
        private System.Windows.Forms.GroupBox ZoneColorGroupBox;
        private ColorBand ZoneColorBand;
        private SolidColorPanel SolidColorPanel;
        private System.Windows.Forms.Label DescriptionLabel;
    }
}
