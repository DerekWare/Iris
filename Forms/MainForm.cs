using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Forms;
using AutoUpdaterDotNET;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common.Effects;
using DerekWare.HomeAutomation.Common.Scenes;
using DerekWare.HomeAutomation.Common.Themes;
using DerekWare.Iris.Properties;
using LifxClient = DerekWare.HomeAutomation.Lifx.Lan.Client;
using HueClient = DerekWare.HomeAutomation.PhilipsHue.Client;

namespace DerekWare.Iris
{
    public partial class MainForm : Form
    {
        DeviceActionPanel DeviceActionPanel;
        bool IsExiting;
        FormWindowState RestoreWindowState = FormWindowState.Normal;
        ScenePanel ScenePanel;

        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            AutoUpdater.ApplicationExitEvent += OnApplicationExitEvent;
            AutoUpdater.InstalledVersion = Program.Version;

            // The reminder and skip both have a bug, so don't use them
            AutoUpdater.ShowRemindLaterButton = false;
            AutoUpdater.ShowSkipButton = false;

            LoadSettings();
            CheckForUpdates();
            base.OnLoad(e);
        }

        protected override void OnResize(EventArgs e)
        {
            if(FormWindowState.Minimized != WindowState)
            {
                RestoreWindowState = WindowState;
            }

            base.OnResize(e);
        }

        void CheckForUpdates()
        {
            AutoUpdater.Start("http://www.derekware.com/software/iris/AutoUpdater.xml");
        }

        void LoadSettings()
        {
            // Load cached LIFX devices
            Settings.Default.LifxDevices ??= new StringCollection();
            Settings.Default.LifxDevices.OfType<string>().ForEach(LifxClient.Instance.Connect);

            // Connect to the Hue bridge
            if(!Settings.Default.HueBridgeAddress.IsNullOrEmpty() && !Settings.Default.HueApiKey.IsNullOrEmpty())
            {
                HueClient.Instance.Connect(Settings.Default.HueBridgeAddress, Settings.Default.HueApiKey);
            }

            // Deserialize cached effect, them and scene settings
            EffectFactory.Instance.Deserialize(Settings.Default.Effects);
            ThemeFactory.Instance.Deserialize(Settings.Default.Themes);
            SceneFactory.Instance.Deserialize(Settings.Default.Scenes);
        }

        void MinimizeToTray()
        {
            WindowState = FormWindowState.Minimized;
            Visible = false;
            ShowInTaskbar = false;
        }

        void RestoreFromTray()
        {
            ShowInTaskbar = true;
            Visible = true;
            WindowState = RestoreWindowState;
            Activate();
        }

        void SaveSettings()
        {
            // Update the device list, pruning any addresses that were unreachable
            Settings.Default.LifxDevices = new StringCollection();
            Settings.Default.LifxDevices.AddRange(LifxClient.Instance.Devices.Where(i => i.IsValid)
                                                            .Select(i => i.Uuid)
                                                            .ToArray()); // HACK using internal knowledge that Uuid == IpAddress

            // Cache other settings
            Settings.Default.Effects = EffectFactory.Instance.Serialize();
            Settings.Default.Themes = ThemeFactory.Instance.Serialize();
            Settings.Default.Scenes = SceneFactory.Instance.Serialize();
            Settings.Default.Save();
        }

        #region Event Handlers

        void AboutMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog(this);
        }

        void ApplySceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ComponentTreeView.SelectedScene?.Apply();
        }

        void BridgeMenuItem_Click(object sender, EventArgs e)
        {
            var dlg = new ConnectBridgeDialog();

            if(DialogResult.OK != dlg.ShowDialog(this))
            {
                return;
            }

            HueClient.Instance.Connect(dlg.IpAddress, dlg.ApiKey);

            Settings.Default.HueBridgeAddress = dlg.IpAddress;
            Settings.Default.HueApiKey = dlg.ApiKey;
            Settings.Default.Save();
        }

        void CloseMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        void ComponentTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            DerekWare.Extensions.Dispose(ref DeviceActionPanel);
            DerekWare.Extensions.Dispose(ref ScenePanel);

            switch(e.Node)
            {
                case DeviceTreeView.DeviceNode deviceNode:
                    DeviceActionPanel = new DeviceActionPanel(deviceNode.Device) { Dock = DockStyle.Fill, Description = Resources.ActionPanelDescription };
                    RootLayoutPanel.Controls.Add(DeviceActionPanel, 1, 0);
                    break;

                case ComponentTreeView.SceneNode sceneNode:
                    ScenePanel = new ScenePanel(sceneNode.Scene) { Dock = DockStyle.Fill };
                    RootLayoutPanel.Controls.Add(ScenePanel, 1, 0);
                    break;
            }

            RenameSceneToolStripMenuItem.Enabled = ScenePanel is not null;
            RemoveSceneToolStripMenuItem.Enabled = ScenePanel is not null;
            ApplySceneToolStripMenuItem.Enabled = ScenePanel is not null;
            UpdateSceneToolStripMenuItem.Enabled = ScenePanel is not null;
        }

        void ComponentTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                ComponentTreeView.SelectedNode = (DeviceTreeView.TreeNode)e.Node;

                if(e.Node is ComponentTreeView.SceneNode || (e.Node == ComponentTreeView.ScenesNode))
                {
                    SceneContextMenuStrip.Show(ComponentTreeView, e.Location);
                }
            }
        }

        void ConnectMenuItem_Click(object sender, EventArgs e)
        {
            var dlg = new ConnectDeviceDialog();

            if(DialogResult.OK != dlg.ShowDialog(this))
            {
                return;
            }

            LifxClient.Instance.Connect(dlg.IpAddress);
        }

        void CreateSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ComponentTreeView.CreateScene();
        }

        void ExitMenuItem_Click(object sender, EventArgs e)
        {
            IsExiting = true;
            Close();
        }

        void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Stop any running effects
            // The try/catch is there because I somehow got 0.0.0.0 into my device list once and
            // sending a message to that address threw an exception, preventing the rest of the
            // method from running and clearing that invalid address out of the list.
            try
            {
                EffectFactory.Instance.StopAllEffects();
            }
            catch
            {
            }

            SaveSettings();
        }

        void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(IsExiting)
            {
                return;
            }

            e.Cancel = true;
            MinimizeToTray();
        }

        void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            RestoreFromTray();
        }

        void OnApplicationExitEvent()
        {
            IsExiting = true;
            Close();
        }

        void RemoveSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ComponentTreeView.RemoveSelectedScene();
        }

        void RenameSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ComponentTreeView.RenameScene();
        }

        void SaveSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        void ScenesToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
        }

        void ShowWindowMenuItem_Click(object sender, EventArgs e)
        {
            RestoreFromTray();
        }

        void UpdateMenuItem_Click(object sender, EventArgs e)
        {
            CheckForUpdates();
        }

        void UpdateSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScenePanel?.SnapshotActiveScene();
        }

        #endregion
    }
}
