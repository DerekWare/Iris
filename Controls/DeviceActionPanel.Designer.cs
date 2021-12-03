
using DerekWare.HomeAutomation.Common;
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
            this.PropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.StateLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.SolidColorPanel = new DerekWare.Iris.SolidColorPanel();
            this.MultiZoneColorPanel = new DerekWare.Iris.MultiZoneColorPanel();
            this.PowerStatePanel = new DerekWare.Iris.PowerStatePanel();
            this.ThemeButtonPanel = new DerekWare.Iris.ThemeButtonPanel();
            this.EffectButtonPanel = new DerekWare.Iris.EffectButtonPanel();
            this.DescriptionLabel = new System.Windows.Forms.Label();
            this.BaseLayoutPanel.SuspendLayout();
            this.StateLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // BaseLayoutPanel
            // 
            this.BaseLayoutPanel.ColumnCount = 2;
            this.BaseLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.BaseLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.BaseLayoutPanel.Controls.Add(this.PropertyGrid, 1, 1);
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
            this.BaseLayoutPanel.Size = new System.Drawing.Size(1945, 1331);
            this.BaseLayoutPanel.TabIndex = 0;
            // 
            // PropertyGrid
            // 
            this.PropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PropertyGrid.Location = new System.Drawing.Point(988, 68);
            this.PropertyGrid.Margin = new System.Windows.Forms.Padding(16);
            this.PropertyGrid.Name = "PropertyGrid";
            this.PropertyGrid.Size = new System.Drawing.Size(941, 1247);
            this.PropertyGrid.TabIndex = 2;
            // 
            // StateLayoutPanel
            // 
            this.StateLayoutPanel.ColumnCount = 1;
            this.StateLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.StateLayoutPanel.Controls.Add(this.SolidColorPanel, 0, 1);
            this.StateLayoutPanel.Controls.Add(this.MultiZoneColorPanel, 0, 2);
            this.StateLayoutPanel.Controls.Add(this.PowerStatePanel, 0, 0);
            this.StateLayoutPanel.Controls.Add(this.ThemeButtonPanel, 0, 3);
            this.StateLayoutPanel.Controls.Add(this.EffectButtonPanel, 0, 4);
            this.StateLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.StateLayoutPanel.Location = new System.Drawing.Point(0, 52);
            this.StateLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.StateLayoutPanel.Name = "StateLayoutPanel";
            this.StateLayoutPanel.RowCount = 5;
            this.StateLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.StateLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.StateLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.StateLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.StateLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.StateLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.StateLayoutPanel.Size = new System.Drawing.Size(972, 1279);
            this.StateLayoutPanel.TabIndex = 1;
            // 
            // SolidColorPanel
            // 
            this.SolidColorPanel.AutoSize = true;
            this.SolidColorPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.SolidColorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SolidColorPanel.Location = new System.Drawing.Point(8, 125);
            this.SolidColorPanel.Margin = new System.Windows.Forms.Padding(8);
            this.SolidColorPanel.Name = "SolidColorPanel";
            this.SolidColorPanel.Padding = new System.Windows.Forms.Padding(8);
            this.SolidColorPanel.Size = new System.Drawing.Size(956, 179);
            this.SolidColorPanel.TabIndex = 1;
            this.SolidColorPanel.ColorChanged += new System.EventHandler<DerekWare.Iris.ColorChangedEventArgs>(this.SolidColorPanel_ColorChanged);
            // 
            // MultiZoneColorPanel
            // 
            this.MultiZoneColorPanel.AutoSize = true;
            this.MultiZoneColorPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.MultiZoneColorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MultiZoneColorPanel.Location = new System.Drawing.Point(8, 320);
            this.MultiZoneColorPanel.Margin = new System.Windows.Forms.Padding(8);
            this.MultiZoneColorPanel.Name = "MultiZoneColorPanel";
            this.MultiZoneColorPanel.Padding = new System.Windows.Forms.Padding(8);
            this.MultiZoneColorPanel.Size = new System.Drawing.Size(956, 179);
            this.MultiZoneColorPanel.TabIndex = 2;
            this.MultiZoneColorPanel.ColorsChanged += new System.EventHandler<DerekWare.Iris.ColorsChangedEventArgs>(this.MultiZoneColorPanel_ColorsChanged);
            // 
            // PowerStatePanel
            // 
            this.PowerStatePanel.AutoSize = true;
            this.PowerStatePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.PowerStatePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PowerStatePanel.Location = new System.Drawing.Point(8, 8);
            this.PowerStatePanel.Margin = new System.Windows.Forms.Padding(8);
            this.PowerStatePanel.Name = "PowerStatePanel";
            this.PowerStatePanel.Padding = new System.Windows.Forms.Padding(8);
            this.PowerStatePanel.Size = new System.Drawing.Size(956, 101);
            this.PowerStatePanel.TabIndex = 3;
            this.PowerStatePanel.PowerStateChanged += new System.EventHandler<DerekWare.Iris.PowerStateChangedEventArgs>(this.PowerStatePanel_PowerStateChanged);
            // 
            // ThemeButtonPanel
            // 
            this.ThemeButtonPanel.AutoSize = true;
            this.ThemeButtonPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ThemeButtonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ThemeButtonPanel.Location = new System.Drawing.Point(8, 515);
            this.ThemeButtonPanel.Margin = new System.Windows.Forms.Padding(8);
            this.ThemeButtonPanel.Name = "ThemeButtonPanel";
            this.ThemeButtonPanel.Padding = new System.Windows.Forms.Padding(8);
            this.ThemeButtonPanel.Size = new System.Drawing.Size(956, 370);
            this.ThemeButtonPanel.TabIndex = 4;
            this.ThemeButtonPanel.SelectedThemeChanged += new System.EventHandler<DerekWare.Iris.SelectedThemeChangedEventArgs>(this.ThemePanel_SelectedThemeChanged);
            // 
            // EffectButtonPanel
            // 
            this.EffectButtonPanel.AutoSize = true;
            this.EffectButtonPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.EffectButtonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EffectButtonPanel.Location = new System.Drawing.Point(8, 901);
            this.EffectButtonPanel.Margin = new System.Windows.Forms.Padding(8);
            this.EffectButtonPanel.Name = "EffectButtonPanel";
            this.EffectButtonPanel.Padding = new System.Windows.Forms.Padding(8);
            this.EffectButtonPanel.Size = new System.Drawing.Size(956, 370);
            this.EffectButtonPanel.TabIndex = 5;
            this.EffectButtonPanel.SelectedEffectChanged += new System.EventHandler<DerekWare.Iris.SelectedEffectChangedEventArgs>(this.EffectPanel_SelectedEffectChanged);
            // 
            // DescriptionLabel
            // 
            this.DescriptionLabel.AutoSize = true;
            this.BaseLayoutPanel.SetColumnSpan(this.DescriptionLabel, 2);
            this.DescriptionLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DescriptionLabel.Location = new System.Drawing.Point(16, 16);
            this.DescriptionLabel.Margin = new System.Windows.Forms.Padding(16);
            this.DescriptionLabel.Name = "DescriptionLabel";
            this.DescriptionLabel.Size = new System.Drawing.Size(1913, 20);
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
            this.Size = new System.Drawing.Size(1945, 1331);
            this.BaseLayoutPanel.ResumeLayout(false);
            this.BaseLayoutPanel.PerformLayout();
            this.StateLayoutPanel.ResumeLayout(false);
            this.StateLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel BaseLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel StateLayoutPanel;
        private System.Windows.Forms.Label DescriptionLabel;
        private SolidColorPanel SolidColorPanel;
        private System.Windows.Forms.PropertyGrid PropertyGrid;
        private MultiZoneColorPanel MultiZoneColorPanel;
        private PowerStatePanel PowerStatePanel;
        private ThemeButtonPanel ThemeButtonPanel;
        private EffectButtonPanel EffectButtonPanel;
    }
}
