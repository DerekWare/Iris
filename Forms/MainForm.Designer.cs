
using DerekWare.HomeAutomation.Lifx.Lan;

namespace DerekWare.Iris
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.NotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.NotifyIconMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ShowWindowNotifyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ExitNotifyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip = new System.Windows.Forms.MenuStrip();
            this.FileToolStirpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ConnectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.BridgeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.LaunchOnSystemStartupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.SaveSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.CloseMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ScenesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SceneContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.CreateSceneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RenameSceneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SelectSceneDevicesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.ApplySceneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AutomaticallyApplySceneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.RemoveSceneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewReadmeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.UpdateMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RootLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.ComponentTreeView = new DerekWare.Iris.ComponentTreeView();
            this.NotifyIconMenuStrip.SuspendLayout();
            this.MenuStrip.SuspendLayout();
            this.SceneContextMenuStrip.SuspendLayout();
            this.RootLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // NotifyIcon
            // 
            this.NotifyIcon.ContextMenuStrip = this.NotifyIconMenuStrip;
            this.NotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("NotifyIcon.Icon")));
            this.NotifyIcon.Text = this.ProductName;
            this.NotifyIcon.Visible = true;
            this.NotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon_MouseDoubleClick);
            // 
            // NotifyIconMenuStrip
            // 
            this.NotifyIconMenuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.NotifyIconMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ShowWindowNotifyMenuItem,
            this.toolStripSeparator1,
            this.ExitNotifyMenuItem});
            this.NotifyIconMenuStrip.Name = "NotifyIconMenuStrip";
            this.NotifyIconMenuStrip.Size = new System.Drawing.Size(200, 74);
            // 
            // ShowWindowNotifyMenuItem
            // 
            this.ShowWindowNotifyMenuItem.Name = "ShowWindowNotifyMenuItem";
            this.ShowWindowNotifyMenuItem.Size = new System.Drawing.Size(199, 32);
            this.ShowWindowNotifyMenuItem.Text = "Show &Window";
            this.ShowWindowNotifyMenuItem.Click += new System.EventHandler(this.ShowWindowMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(196, 6);
            // 
            // ExitNotifyMenuItem
            // 
            this.ExitNotifyMenuItem.Name = "ExitNotifyMenuItem";
            this.ExitNotifyMenuItem.Size = new System.Drawing.Size(199, 32);
            this.ExitNotifyMenuItem.Text = "E&xit";
            this.ExitNotifyMenuItem.Click += new System.EventHandler(this.ExitMenuItem_Click);
            // 
            // MenuStrip
            // 
            this.MenuStrip.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.MenuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileToolStirpMenuItem,
            this.ScenesToolStripMenuItem,
            this.HelpToolStripMenuItem});
            this.MenuStrip.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip.Name = "MenuStrip";
            this.MenuStrip.Size = new System.Drawing.Size(2130, 36);
            this.MenuStrip.TabIndex = 0;
            // 
            // FileToolStirpMenuItem
            // 
            this.FileToolStirpMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ConnectMenuItem,
            this.BridgeMenuItem,
            this.toolStripSeparator6,
            this.LaunchOnSystemStartupToolStripMenuItem,
            this.toolStripSeparator3,
            this.SaveSettingsToolStripMenuItem,
            this.toolStripSeparator2,
            this.CloseMenuItem,
            this.ExitMenuItem});
            this.FileToolStirpMenuItem.Name = "FileToolStirpMenuItem";
            this.FileToolStirpMenuItem.Size = new System.Drawing.Size(54, 30);
            this.FileToolStirpMenuItem.Text = "&File";
            // 
            // ConnectMenuItem
            // 
            this.ConnectMenuItem.Name = "ConnectMenuItem";
            this.ConnectMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.ConnectMenuItem.Size = new System.Drawing.Size(373, 34);
            this.ConnectMenuItem.Text = "Connect to &LIFX Device...";
            this.ConnectMenuItem.Click += new System.EventHandler(this.ConnectMenuItem_Click);
            // 
            // BridgeMenuItem
            // 
            this.BridgeMenuItem.Name = "BridgeMenuItem";
            this.BridgeMenuItem.Size = new System.Drawing.Size(373, 34);
            this.BridgeMenuItem.Text = "Connect to &Hue Bridge...";
            this.BridgeMenuItem.Click += new System.EventHandler(this.BridgeMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(370, 6);
            // 
            // LaunchOnSystemStartupToolStripMenuItem
            // 
            this.LaunchOnSystemStartupToolStripMenuItem.CheckOnClick = true;
            this.LaunchOnSystemStartupToolStripMenuItem.Name = "LaunchOnSystemStartupToolStripMenuItem";
            this.LaunchOnSystemStartupToolStripMenuItem.Size = new System.Drawing.Size(373, 34);
            this.LaunchOnSystemStartupToolStripMenuItem.Text = "Launch on System Startup";
            this.LaunchOnSystemStartupToolStripMenuItem.CheckedChanged += new System.EventHandler(this.LaunchOnSystemStartupToolStripMenuItem_CheckedChanged);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(370, 6);
            // 
            // SaveSettingsToolStripMenuItem
            // 
            this.SaveSettingsToolStripMenuItem.Name = "SaveSettingsToolStripMenuItem";
            this.SaveSettingsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.SaveSettingsToolStripMenuItem.Size = new System.Drawing.Size(373, 34);
            this.SaveSettingsToolStripMenuItem.Text = "&Save Settings";
            this.SaveSettingsToolStripMenuItem.Click += new System.EventHandler(this.SaveSettingsToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(370, 6);
            // 
            // CloseMenuItem
            // 
            this.CloseMenuItem.Name = "CloseMenuItem";
            this.CloseMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.CloseMenuItem.Size = new System.Drawing.Size(373, 34);
            this.CloseMenuItem.Text = "&Close";
            this.CloseMenuItem.Click += new System.EventHandler(this.CloseMenuItem_Click);
            // 
            // ExitMenuItem
            // 
            this.ExitMenuItem.Name = "ExitMenuItem";
            this.ExitMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.F4)));
            this.ExitMenuItem.Size = new System.Drawing.Size(373, 34);
            this.ExitMenuItem.Text = "E&xit";
            this.ExitMenuItem.Click += new System.EventHandler(this.ExitMenuItem_Click);
            // 
            // ScenesToolStripMenuItem
            // 
            this.ScenesToolStripMenuItem.DropDown = this.SceneContextMenuStrip;
            this.ScenesToolStripMenuItem.Name = "ScenesToolStripMenuItem";
            this.ScenesToolStripMenuItem.Size = new System.Drawing.Size(82, 30);
            this.ScenesToolStripMenuItem.Text = "&Scenes";
            this.ScenesToolStripMenuItem.DropDownOpening += new System.EventHandler(this.ScenesToolStripMenuItem_DropDownOpening);
            // 
            // SceneContextMenuStrip
            // 
            this.SceneContextMenuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.SceneContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CreateSceneToolStripMenuItem,
            this.RenameSceneToolStripMenuItem,
            this.SelectSceneDevicesToolStripMenuItem,
            this.toolStripSeparator5,
            this.ApplySceneToolStripMenuItem,
            this.AutomaticallyApplySceneToolStripMenuItem,
            this.toolStripSeparator4,
            this.RemoveSceneToolStripMenuItem});
            this.SceneContextMenuStrip.Name = "SceneContextMenuStrip";
            this.SceneContextMenuStrip.OwnerItem = this.ScenesToolStripMenuItem;
            this.SceneContextMenuStrip.Size = new System.Drawing.Size(296, 208);
            // 
            // CreateSceneToolStripMenuItem
            // 
            this.CreateSceneToolStripMenuItem.Name = "CreateSceneToolStripMenuItem";
            this.CreateSceneToolStripMenuItem.Size = new System.Drawing.Size(295, 32);
            this.CreateSceneToolStripMenuItem.Text = "&Create Scene...";
            this.CreateSceneToolStripMenuItem.Click += new System.EventHandler(this.CreateSceneToolStripMenuItem_Click);
            // 
            // RenameSceneToolStripMenuItem
            // 
            this.RenameSceneToolStripMenuItem.Enabled = false;
            this.RenameSceneToolStripMenuItem.Name = "RenameSceneToolStripMenuItem";
            this.RenameSceneToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.RenameSceneToolStripMenuItem.Size = new System.Drawing.Size(295, 32);
            this.RenameSceneToolStripMenuItem.Text = "Re&name Scene";
            this.RenameSceneToolStripMenuItem.Click += new System.EventHandler(this.RenameSceneToolStripMenuItem_Click);
            // 
            // SelectSceneDevicesToolStripMenuItem
            // 
            this.SelectSceneDevicesToolStripMenuItem.Enabled = false;
            this.SelectSceneDevicesToolStripMenuItem.Name = "SelectSceneDevicesToolStripMenuItem";
            this.SelectSceneDevicesToolStripMenuItem.Size = new System.Drawing.Size(295, 32);
            this.SelectSceneDevicesToolStripMenuItem.Text = "Select &Devices...";
            this.SelectSceneDevicesToolStripMenuItem.Click += new System.EventHandler(this.SelectSceneDevicesToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(292, 6);
            // 
            // ApplySceneToolStripMenuItem
            // 
            this.ApplySceneToolStripMenuItem.Enabled = false;
            this.ApplySceneToolStripMenuItem.Name = "ApplySceneToolStripMenuItem";
            this.ApplySceneToolStripMenuItem.Size = new System.Drawing.Size(295, 32);
            this.ApplySceneToolStripMenuItem.Text = "&Apply Scene";
            this.ApplySceneToolStripMenuItem.Click += new System.EventHandler(this.ApplySceneToolStripMenuItem_Click);
            // 
            // AutomaticallyApplySceneToolStripMenuItem
            // 
            this.AutomaticallyApplySceneToolStripMenuItem.CheckOnClick = true;
            this.AutomaticallyApplySceneToolStripMenuItem.Enabled = false;
            this.AutomaticallyApplySceneToolStripMenuItem.Name = "AutomaticallyApplySceneToolStripMenuItem";
            this.AutomaticallyApplySceneToolStripMenuItem.Size = new System.Drawing.Size(295, 32);
            this.AutomaticallyApplySceneToolStripMenuItem.Text = "A&utomatically Apply Scene";
            this.AutomaticallyApplySceneToolStripMenuItem.CheckedChanged += new System.EventHandler(this.AutomaticallyApplySceneToolStripMenuItem_CheckedChanged);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(292, 6);
            // 
            // RemoveSceneToolStripMenuItem
            // 
            this.RemoveSceneToolStripMenuItem.Enabled = false;
            this.RemoveSceneToolStripMenuItem.Name = "RemoveSceneToolStripMenuItem";
            this.RemoveSceneToolStripMenuItem.Size = new System.Drawing.Size(295, 32);
            this.RemoveSceneToolStripMenuItem.Text = "&Remove Scene";
            this.RemoveSceneToolStripMenuItem.Click += new System.EventHandler(this.RemoveSceneToolStripMenuItem_Click);
            // 
            // HelpToolStripMenuItem
            // 
            this.HelpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ViewReadmeToolStripMenuItem,
            this.UpdateMenuItem,
            this.AboutMenuItem});
            this.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem";
            this.HelpToolStripMenuItem.Size = new System.Drawing.Size(65, 30);
            this.HelpToolStripMenuItem.Text = "&Help";
            // 
            // ViewReadmeToolStripMenuItem
            // 
            this.ViewReadmeToolStripMenuItem.Name = "ViewReadmeToolStripMenuItem";
            this.ViewReadmeToolStripMenuItem.Size = new System.Drawing.Size(272, 34);
            this.ViewReadmeToolStripMenuItem.Text = "View &ReadMe...";
            this.ViewReadmeToolStripMenuItem.Click += new System.EventHandler(this.ViewReadmeToolStripMenuItem_Click);
            // 
            // UpdateMenuItem
            // 
            this.UpdateMenuItem.Name = "UpdateMenuItem";
            this.UpdateMenuItem.Size = new System.Drawing.Size(272, 34);
            this.UpdateMenuItem.Text = "Check for &Updates...";
            this.UpdateMenuItem.Click += new System.EventHandler(this.UpdateMenuItem_Click);
            // 
            // AboutMenuItem
            // 
            this.AboutMenuItem.Name = "AboutMenuItem";
            this.AboutMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.AboutMenuItem.Size = new System.Drawing.Size(272, 34);
            this.AboutMenuItem.Text = "&About...";
            this.AboutMenuItem.Click += new System.EventHandler(this.AboutMenuItem_Click);
            // 
            // RootLayoutPanel
            // 
            this.RootLayoutPanel.ColumnCount = 2;
            this.RootLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.RootLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.RootLayoutPanel.Controls.Add(this.ComponentTreeView, 0, 0);
            this.RootLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RootLayoutPanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.AddColumns;
            this.RootLayoutPanel.Location = new System.Drawing.Point(0, 36);
            this.RootLayoutPanel.Name = "RootLayoutPanel";
            this.RootLayoutPanel.RowCount = 1;
            this.RootLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.RootLayoutPanel.Size = new System.Drawing.Size(2130, 1186);
            this.RootLayoutPanel.TabIndex = 1;
            // 
            // ComponentTreeView
            // 
            this.ComponentTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ComponentTreeView.HideSelection = false;
            this.ComponentTreeView.LabelEdit = true;
            this.ComponentTreeView.Location = new System.Drawing.Point(8, 8);
            this.ComponentTreeView.Margin = new System.Windows.Forms.Padding(8);
            this.ComponentTreeView.Name = "ComponentTreeView";
            this.ComponentTreeView.SelectedNode = null;
            this.ComponentTreeView.ShowFamilyNodes = true;
            this.ComponentTreeView.ShowStateNodes = true;
            this.ComponentTreeView.Size = new System.Drawing.Size(410, 1170);
            this.ComponentTreeView.TabIndex = 0;
            this.ComponentTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ComponentTreeView_AfterSelect);
            this.ComponentTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.ComponentTreeView_NodeMouseClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2130, 1222);
            this.Controls.Add(this.RootLayoutPanel);
            this.Controls.Add(this.MenuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultBounds;
            this.Text = this.ProductName;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.NotifyIconMenuStrip.ResumeLayout(false);
            this.MenuStrip.ResumeLayout(false);
            this.MenuStrip.PerformLayout();
            this.SceneContextMenuStrip.ResumeLayout(false);
            this.RootLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon NotifyIcon;
        private System.Windows.Forms.ContextMenuStrip NotifyIconMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem ShowWindowNotifyMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem ExitNotifyMenuItem;
        private System.Windows.Forms.MenuStrip MenuStrip;
        private System.Windows.Forms.ToolStripMenuItem FileToolStirpMenuItem;
        private System.Windows.Forms.ToolStripMenuItem HelpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ConnectMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AboutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem UpdateMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem ExitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CloseMenuItem;
        private System.Windows.Forms.TableLayoutPanel RootLayoutPanel;
        private ComponentTreeView ComponentTreeView;
        private System.Windows.Forms.ToolStripMenuItem BridgeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ScenesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SaveSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ContextMenuStrip SceneContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem CreateSceneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RenameSceneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RemoveSceneToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem ApplySceneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ViewReadmeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SelectSceneDevicesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem AutomaticallyApplySceneToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem LaunchOnSystemStartupToolStripMenuItem;
    }
}

