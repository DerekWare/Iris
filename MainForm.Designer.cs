
using DerekWare.HomeAutomation.Lifx.Lan.Devices;

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
            this.FileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ConnectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.BridgeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.CloseMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.UpdateMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RootLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.ComponentTreeView = new DerekWare.Iris.ComponentTreeView();
            this.scenesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CreateSceneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RemoveSceneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ApplySceneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NotifyIconMenuStrip.SuspendLayout();
            this.MenuStrip.SuspendLayout();
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
            this.FileMenuItem,
            this.scenesToolStripMenuItem,
            this.HelpMenuItem});
            this.MenuStrip.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip.Name = "MenuStrip";
            this.MenuStrip.Size = new System.Drawing.Size(2130, 33);
            this.MenuStrip.TabIndex = 0;
            // 
            // FileMenuItem
            // 
            this.FileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ConnectMenuItem,
            this.BridgeMenuItem,
            this.toolStripSeparator2,
            this.CloseMenuItem,
            this.ExitMenuItem});
            this.FileMenuItem.Name = "FileMenuItem";
            this.FileMenuItem.Size = new System.Drawing.Size(54, 29);
            this.FileMenuItem.Text = "&File";
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
            // HelpMenuItem
            // 
            this.HelpMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.UpdateMenuItem,
            this.AboutMenuItem});
            this.HelpMenuItem.Name = "HelpMenuItem";
            this.HelpMenuItem.Size = new System.Drawing.Size(65, 29);
            this.HelpMenuItem.Text = "&Help";
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
            this.RootLayoutPanel.Location = new System.Drawing.Point(0, 33);
            this.RootLayoutPanel.Name = "RootLayoutPanel";
            this.RootLayoutPanel.RowCount = 1;
            this.RootLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.RootLayoutPanel.Size = new System.Drawing.Size(2130, 1189);
            this.RootLayoutPanel.TabIndex = 1;
            // 
            // ComponentTreeView
            // 
            this.ComponentTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ComponentTreeView.HideSelection = false;
            this.ComponentTreeView.Location = new System.Drawing.Point(3, 3);
            this.ComponentTreeView.Name = "ComponentTreeView";
            this.ComponentTreeView.Size = new System.Drawing.Size(420, 1183);
            this.ComponentTreeView.TabIndex = 0;
            this.ComponentTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ComponentTreeView_AfterSelect);
            // 
            // scenesToolStripMenuItem
            // 
            this.scenesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CreateSceneToolStripMenuItem,this.RemoveSceneToolStripMenuItem, ApplySceneToolStripMenuItem});
            this.scenesToolStripMenuItem.Name = "scenesToolStripMenuItem";
            this.scenesToolStripMenuItem.Size = new System.Drawing.Size(82, 29);
            this.scenesToolStripMenuItem.Text = "&Scenes";
            // 
            // CreateSceneToolStripMenuItem
            // 
            this.CreateSceneToolStripMenuItem.Name = "CreateSceneToolStripMenuItem";
            this.CreateSceneToolStripMenuItem.Size = new System.Drawing.Size(270, 34);
            this.CreateSceneToolStripMenuItem.Text = "&Create";
            this.CreateSceneToolStripMenuItem.Click += new System.EventHandler(this.CreateSceneToolStripMenuItem_Click);
            // 
            // RemoveSceneToolStripMenuItem
            // 
            this.RemoveSceneToolStripMenuItem.Name = "RemoveSceneToolStripMenuItem";
            this.RemoveSceneToolStripMenuItem.Size = new System.Drawing.Size(270, 34);
            this.RemoveSceneToolStripMenuItem.Text = "&Remove";
            this.RemoveSceneToolStripMenuItem.Click += new System.EventHandler(this.RemoveSceneToolStripMenuItem_Click);
            this.RemoveSceneToolStripMenuItem.Enabled = false;
            // 
            // ApplySceneToolStripMenuItem
            // 
            this.ApplySceneToolStripMenuItem.Name = "ApplySceneToolStripMenuItem";
            this.ApplySceneToolStripMenuItem.Size = new System.Drawing.Size(270, 34);
            this.ApplySceneToolStripMenuItem.Text = "&Apply";
            this.ApplySceneToolStripMenuItem.Click += new System.EventHandler(this.ApplySceneToolStripMenuItem_Click);
            this.ApplySceneToolStripMenuItem.Enabled = false;
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
        private System.Windows.Forms.ToolStripMenuItem FileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem HelpMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ConnectMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AboutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem UpdateMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem ExitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CloseMenuItem;
        private System.Windows.Forms.TableLayoutPanel RootLayoutPanel;
        private ComponentTreeView ComponentTreeView;
        private System.Windows.Forms.ToolStripMenuItem BridgeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scenesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CreateSceneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RemoveSceneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ApplySceneToolStripMenuItem;
    }
}

