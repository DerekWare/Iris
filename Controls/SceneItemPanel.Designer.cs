
using System.Windows.Forms;

namespace DerekWare.Iris
{
    partial class SceneItemPanel
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
            this.EffectPanel = new DerekWare.Iris.EffectDropDownPanel();
            this.ThemePanel = new DerekWare.Iris.ThemeDropDownPanel();
            this.MultiZoneColorPanel = new DerekWare.Iris.MultiZoneColorPanel();
            this.SolidColorPanel = new DerekWare.Iris.SolidColorPanel();
            this.PowerStatePanel = new DerekWare.Iris.PowerStatePanel();
            this.TableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.TabControl = new System.Windows.Forms.TabControl();
            this.TabPage = new System.Windows.Forms.TabPage();
            this.TableLayoutPanel.SuspendLayout();
            this.TabControl.SuspendLayout();
            this.TabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // EffectPanel
            // 
            this.EffectPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EffectPanel.Location = new System.Drawing.Point(3, 239);
            this.EffectPanel.Name = "EffectPanel";
            this.EffectPanel.Padding = new System.Windows.Forms.Padding(8);
            this.TableLayoutPanel.SetRowSpan(this.EffectPanel, 2);
            this.EffectPanel.Size = new System.Drawing.Size(957, 115);
            this.EffectPanel.TabIndex = 6;
            this.EffectPanel.SelectedEffectChanged += new System.EventHandler<DerekWare.Iris.SelectedEffectChangedEventArgs>(this.EffectPanel_SelectedEffectChanged);
            // 
            // ThemePanel
            // 
            this.ThemePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ThemePanel.Location = new System.Drawing.Point(3, 121);
            this.ThemePanel.Name = "ThemePanel";
            this.ThemePanel.Padding = new System.Windows.Forms.Padding(8);
            this.TableLayoutPanel.SetRowSpan(this.ThemePanel, 2);
            this.ThemePanel.Size = new System.Drawing.Size(957, 112);
            this.ThemePanel.TabIndex = 5;
            this.ThemePanel.SelectedThemeChanged += new System.EventHandler<DerekWare.Iris.SelectedThemeChangedEventArgs>(this.ThemePanel_SelectedThemeChanged);
            // 
            // MultiZoneColorPanel
            // 
            this.MultiZoneColorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MultiZoneColorPanel.Location = new System.Drawing.Point(966, 180);
            this.MultiZoneColorPanel.Name = "MultiZoneColorPanel";
            this.MultiZoneColorPanel.Padding = new System.Windows.Forms.Padding(8);
            this.TableLayoutPanel.SetRowSpan(this.MultiZoneColorPanel, 3);
            this.MultiZoneColorPanel.Size = new System.Drawing.Size(957, 174);
            this.MultiZoneColorPanel.TabIndex = 4;
            this.MultiZoneColorPanel.ColorsChanged += new System.EventHandler<DerekWare.Iris.ColorsChangedEventArgs>(this.MultiZoneColorPanel_ColorsChanged);
            // 
            // SolidColorPanel
            // 
            this.SolidColorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SolidColorPanel.Location = new System.Drawing.Point(966, 3);
            this.SolidColorPanel.Name = "SolidColorPanel";
            this.SolidColorPanel.Padding = new System.Windows.Forms.Padding(8);
            this.TableLayoutPanel.SetRowSpan(this.SolidColorPanel, 3);
            this.SolidColorPanel.Size = new System.Drawing.Size(957, 171);
            this.SolidColorPanel.TabIndex = 3;
            this.SolidColorPanel.ColorChanged += new System.EventHandler<DerekWare.Iris.ColorChangedEventArgs>(this.SolidColorPanel_ColorChanged);
            // 
            // PowerStatePanel
            // 
            this.PowerStatePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PowerStatePanel.Location = new System.Drawing.Point(3, 3);
            this.PowerStatePanel.Name = "PowerStatePanel";
            this.PowerStatePanel.Padding = new System.Windows.Forms.Padding(8);
            this.TableLayoutPanel.SetRowSpan(this.PowerStatePanel, 2);
            this.PowerStatePanel.Size = new System.Drawing.Size(957, 112);
            this.PowerStatePanel.TabIndex = 0;
            this.PowerStatePanel.PowerStateChanged += new System.EventHandler<DerekWare.Iris.PowerStateChangedEventArgs>(this.PowerStatePanel_PowerStateChanged);
            // 
            // TableLayoutPanel
            // 
            this.TableLayoutPanel.ColumnCount = 2;
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayoutPanel.Controls.Add(this.PowerStatePanel, 0, 0);
            this.TableLayoutPanel.Controls.Add(this.SolidColorPanel, 1, 0);
            this.TableLayoutPanel.Controls.Add(this.MultiZoneColorPanel, 1, 2);
            this.TableLayoutPanel.Controls.Add(this.ThemePanel, 0, 2);
            this.TableLayoutPanel.Controls.Add(this.EffectPanel, 0, 4);
            this.TableLayoutPanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.AddColumns;
            this.TableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.TableLayoutPanel.Name = "TableLayoutPanel";
            this.TableLayoutPanel.RowCount = 6;
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.TableLayoutPanel.Size = new System.Drawing.Size(1926, 357);
            this.TableLayoutPanel.TabIndex = 0;
            // 
            // TabControl
            // 
            this.TabControl.Controls.Add(this.TabPage);
            this.TabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabControl.Location = new System.Drawing.Point(0, 0);
            this.TabControl.Name = "TabControl";
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new System.Drawing.Size(1934, 390);
            this.TabControl.TabIndex = 0;
            // 
            // TabPage
            // 
            this.TabPage.Controls.Add(this.TableLayoutPanel);
            this.TabPage.Location = new System.Drawing.Point(4, 29);
            this.TabPage.Name = "TabPage";
            this.TabPage.Size = new System.Drawing.Size(1926, 357);
            this.TabPage.TabIndex = 0;
            this.TabPage.Text = "SceneItem";
            // 
            // SceneItemPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.TabControl);
            this.Name = "SceneItemPanel";
            this.Size = new System.Drawing.Size(1934, 390);
            this.TableLayoutPanel.ResumeLayout(false);
            this.TabControl.ResumeLayout(false);
            this.TabPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        TabPage TabPage;
        TabControl TabControl;
        private EffectDropDownPanel EffectPanel;
        private System.Windows.Forms.TableLayoutPanel TableLayoutPanel;
        private PowerStatePanel PowerStatePanel;
        private SolidColorPanel SolidColorPanel;
        private MultiZoneColorPanel MultiZoneColorPanel;
        private ThemeDropDownPanel ThemePanel;
    }
}
