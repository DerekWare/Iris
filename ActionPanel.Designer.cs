
using DerekWare.HomeAutomation.Common.Colors;

namespace DerekWare.Iris
{
    partial class ActionPanel
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
            this.ZoneColorBand = new ColorBand();
            this.EffectGroupBox = new System.Windows.Forms.GroupBox();
            this.EffectLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.SceneGroupBox = new System.Windows.Forms.GroupBox();
            this.SceneLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.SolidColorGroupBox = new System.Windows.Forms.GroupBox();
            this.SolidColorPanel = new SolidColorPanel();
            this.PowerGroupBox = new System.Windows.Forms.GroupBox();
            this.PowerComboBox = new System.Windows.Forms.ComboBox();
            this.BaseLayoutPanel.SuspendLayout();
            this.PropertiesGroupBox.SuspendLayout();
            this.StateLayoutPanel.SuspendLayout();
            this.ZoneColorGroupBox.SuspendLayout();
            this.EffectGroupBox.SuspendLayout();
            this.SceneGroupBox.SuspendLayout();
            this.SolidColorGroupBox.SuspendLayout();
            this.PowerGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // BaseLayoutPanel
            // 
            this.BaseLayoutPanel.ColumnCount = 2;
            this.BaseLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.BaseLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.BaseLayoutPanel.Controls.Add(this.PropertiesGroupBox, 1, 0);
            this.BaseLayoutPanel.Controls.Add(this.StateLayoutPanel, 0, 0);
            this.BaseLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BaseLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.BaseLayoutPanel.Name = "BaseLayoutPanel";
            this.BaseLayoutPanel.RowCount = 1;
            this.BaseLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.BaseLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 737F));
            this.BaseLayoutPanel.Size = new System.Drawing.Size(1472, 737);
            this.BaseLayoutPanel.TabIndex = 5;
            // 
            // PropertiesGroupBox
            // 
            this.PropertiesGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PropertiesGroupBox.Controls.Add(this.PropertyGrid);
            this.PropertiesGroupBox.Location = new System.Drawing.Point(744, 8);
            this.PropertiesGroupBox.Margin = new System.Windows.Forms.Padding(8);
            this.PropertiesGroupBox.Name = "PropertiesGroupBox";
            this.PropertiesGroupBox.Padding = new System.Windows.Forms.Padding(8);
            this.PropertiesGroupBox.Size = new System.Drawing.Size(720, 721);
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
            this.PropertyGrid.Size = new System.Drawing.Size(704, 686);
            this.PropertyGrid.TabIndex = 0;
            // 
            // StateLayoutPanel
            // 
            this.StateLayoutPanel.ColumnCount = 1;
            this.StateLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.StateLayoutPanel.Controls.Add(this.ZoneColorGroupBox, 0, 2);
            this.StateLayoutPanel.Controls.Add(this.EffectGroupBox, 0, 4);
            this.StateLayoutPanel.Controls.Add(this.SceneGroupBox, 0, 3);
            this.StateLayoutPanel.Controls.Add(this.SolidColorGroupBox, 0, 1);
            this.StateLayoutPanel.Controls.Add(this.PowerGroupBox, 0, 0);
            this.StateLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.StateLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.StateLayoutPanel.Name = "StateLayoutPanel";
            this.StateLayoutPanel.RowCount = 5;
            this.StateLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.StateLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.StateLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.StateLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.StateLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.StateLayoutPanel.Size = new System.Drawing.Size(730, 731);
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
            this.ZoneColorGroupBox.Size = new System.Drawing.Size(714, 78);
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
            this.ZoneColorBand.Size = new System.Drawing.Size(698, 43);
            this.ZoneColorBand.TabIndex = 0;
            this.ZoneColorBand.ColorsChanged += new System.EventHandler<ColorsChangedEventArgs>(this.ZoneColorBand_ColorsChanged);
            // 
            // EffectGroupBox
            // 
            this.EffectGroupBox.Controls.Add(this.EffectLayoutPanel);
            this.EffectGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EffectGroupBox.Location = new System.Drawing.Point(8, 549);
            this.EffectGroupBox.Margin = new System.Windows.Forms.Padding(8);
            this.EffectGroupBox.Name = "EffectGroupBox";
            this.EffectGroupBox.Padding = new System.Windows.Forms.Padding(8);
            this.EffectGroupBox.Size = new System.Drawing.Size(714, 174);
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
            this.EffectLayoutPanel.Size = new System.Drawing.Size(698, 139);
            this.EffectLayoutPanel.TabIndex = 0;
            // 
            // SceneGroupBox
            // 
            this.SceneGroupBox.Controls.Add(this.SceneLayoutPanel);
            this.SceneGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SceneGroupBox.Location = new System.Drawing.Point(8, 361);
            this.SceneGroupBox.Margin = new System.Windows.Forms.Padding(8);
            this.SceneGroupBox.Name = "SceneGroupBox";
            this.SceneGroupBox.Padding = new System.Windows.Forms.Padding(8);
            this.SceneGroupBox.Size = new System.Drawing.Size(714, 172);
            this.SceneGroupBox.TabIndex = 10;
            this.SceneGroupBox.TabStop = false;
            this.SceneGroupBox.Text = "Scenes";
            // 
            // SceneLayoutPanel
            // 
            this.SceneLayoutPanel.ColumnCount = 4;
            this.SceneLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.SceneLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.SceneLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.SceneLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.SceneLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SceneLayoutPanel.Location = new System.Drawing.Point(8, 27);
            this.SceneLayoutPanel.Name = "SceneLayoutPanel";
            this.SceneLayoutPanel.RowCount = 4;
            this.SceneLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.SceneLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.SceneLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.SceneLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.SceneLayoutPanel.Size = new System.Drawing.Size(698, 137);
            this.SceneLayoutPanel.TabIndex = 0;
            // 
            // SolidColorGroupBox
            // 
            this.SolidColorGroupBox.Controls.Add(this.SolidColorPanel);
            this.SolidColorGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SolidColorGroupBox.Location = new System.Drawing.Point(8, 87);
            this.SolidColorGroupBox.Margin = new System.Windows.Forms.Padding(8);
            this.SolidColorGroupBox.Name = "SolidColorGroupBox";
            this.SolidColorGroupBox.Padding = new System.Windows.Forms.Padding(8);
            this.SolidColorGroupBox.Size = new System.Drawing.Size(714, 164);
            this.SolidColorGroupBox.TabIndex = 9;
            this.SolidColorGroupBox.TabStop = false;
            this.SolidColorGroupBox.Text = "Solid Color";
            // 
            // SolidColorPanel
            // 
            this.SolidColorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SolidColorPanel.Location = new System.Drawing.Point(8, 27);
            this.SolidColorPanel.Name = "SolidColorPanel";
            this.SolidColorPanel.Size = new System.Drawing.Size(698, 129);
            this.SolidColorPanel.TabIndex = 0;
            this.SolidColorPanel.ColorChanged += new System.EventHandler<ColorChangedEventArgs>(this.SolidColorPanel_ColorChanged);
            // 
            // PowerGroupBox
            // 
            this.PowerGroupBox.Controls.Add(this.PowerComboBox);
            this.PowerGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PowerGroupBox.Location = new System.Drawing.Point(8, 8);
            this.PowerGroupBox.Margin = new System.Windows.Forms.Padding(8);
            this.PowerGroupBox.Name = "PowerGroupBox";
            this.PowerGroupBox.Padding = new System.Windows.Forms.Padding(8);
            this.PowerGroupBox.Size = new System.Drawing.Size(714, 63);
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
            this.PowerComboBox.Size = new System.Drawing.Size(698, 28);
            this.PowerComboBox.TabIndex = 3;
            this.PowerComboBox.SelectedIndexChanged += new System.EventHandler(this.PowerComboBox_SelectedIndexChanged);
            // 
            // ActionPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.BaseLayoutPanel);
            this.Name = "ActionPanel";
            this.Size = new System.Drawing.Size(1472, 737);
            this.BaseLayoutPanel.ResumeLayout(false);
            this.PropertiesGroupBox.ResumeLayout(false);
            this.StateLayoutPanel.ResumeLayout(false);
            this.ZoneColorGroupBox.ResumeLayout(false);
            this.EffectGroupBox.ResumeLayout(false);
            this.SceneGroupBox.ResumeLayout(false);
            this.SolidColorGroupBox.ResumeLayout(false);
            this.PowerGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel BaseLayoutPanel;
        private System.Windows.Forms.GroupBox PropertiesGroupBox;
        private System.Windows.Forms.PropertyGrid PropertyGrid;
        private System.Windows.Forms.TableLayoutPanel StateLayoutPanel;
        private System.Windows.Forms.GroupBox SceneGroupBox;
        private System.Windows.Forms.GroupBox SolidColorGroupBox;
        private System.Windows.Forms.GroupBox PowerGroupBox;
        private System.Windows.Forms.ComboBox PowerComboBox;
        private System.Windows.Forms.TableLayoutPanel SceneLayoutPanel;
        private System.Windows.Forms.GroupBox EffectGroupBox;
        private System.Windows.Forms.TableLayoutPanel EffectLayoutPanel;
        private System.Windows.Forms.GroupBox ZoneColorGroupBox;
        private ColorBand ZoneColorBand;
        private SolidColorPanel SolidColorPanel;
    }
}
