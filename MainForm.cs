using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Forms;
using AutoUpdaterDotNET;
using DerekWare.Collections;
using DerekWare.Diagnostics;
using DerekWare.HomeAutomation.Common.Effects;
using DerekWare.HomeAutomation.Common.Scenes;
using DerekWare.Iris.Properties;
using LifxClient = DerekWare.HomeAutomation.Lifx.Lan.Client;
using HueClient = DerekWare.HomeAutomation.PhilipsHue.Client;

namespace DerekWare.Iris
{
    public partial class MainForm : Form
    {
        bool IsExiting;
        FormWindowState RestoreWindowState = FormWindowState.Normal;

        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            AutoUpdater.ApplicationExitEvent += OnApplicationExitEvent;
            AutoUpdater.InstalledVersion = Program.AutoUpdaterVersion;

            // The reminder and skip both have a bug, so don't use them
            AutoUpdater.ShowRemindLaterButton = false;
            AutoUpdater.ShowSkipButton = false;

            Settings.Default.LifxDevices ??= new StringCollection();
            Settings.Default.LifxDevices.OfType<string>().ForEach(LifxClient.Instance.Connect);

            if(!Settings.Default.HueBridgeAddress.IsNullOrEmpty() && !Settings.Default.HueApiKey.IsNullOrEmpty())
            {
                HueClient.Instance.Connect(Settings.Default.HueBridgeAddress, Settings.Default.HueApiKey);
            }

            SceneFactory.Instance.Deserialize(Settings.Default.Scenes);

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
            RootLayoutPanel.Controls.OfType<DeviceActionPanel>().ToList().ForEach(i => i.Dispose());
            RootLayoutPanel.Controls.OfType<ScenePanel>().ToList().ForEach(i => i.Dispose());

            if(e.Node is DeviceTreeView.DeviceNode deviceNode)
            {
                // deviceNode.Device.RefreshState();
                RootLayoutPanel.Controls.Add(new DeviceActionPanel(deviceNode.Device) { Dock = DockStyle.Fill, Description = Resources.ActionPanelDescription },
                                             1,
                                             0);
            }
            else if(e.Node is ComponentTreeView.SceneNode sceneNode)
            {
                RootLayoutPanel.Controls.Add(new ScenePanel(sceneNode.Scene) { Dock = DockStyle.Fill }, 1, 0);
            }

            RemoveSceneToolStripMenuItem.Enabled = e.Node is ComponentTreeView.SceneNode;
            ApplySceneToolStripMenuItem.Enabled = e.Node is ComponentTreeView.SceneNode;
        }

        void ComponentTreeView_OnAfterCheck(object sender, TreeViewEventArgs e)
        {
            var scenePanel = RootLayoutPanel.Controls.OfType<ScenePanel>().FirstOrDefault();

            if(scenePanel is null)
            {
                Debug.Error(this, "Unexpected check event (no scene panel)");
                return;
            }

            var device = (e.Node as DeviceTreeView.DeviceNode)?.Device;

            if(device is null)
            {
                Debug.Error(this, "Unexpected check event (not a device)");
                return;
            }

            if(e.Node.Checked)
            {
                scenePanel.Scene.Add(device);
            }
            else
            {
                scenePanel.Scene.Remove(device);
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

            // Update the device list, pruning any addresses that were unreachable
            Settings.Default.LifxDevices = new StringCollection();
            Settings.Default.LifxDevices.AddRange(LifxClient.Instance.Devices.Where(i => i.IsValid)
                                                            .Select(i => i.Uuid)
                                                            .ToArray()); // HACK using internal knowledge that Uuid == IpAddress

            // Save the scene list to the settings
            Settings.Default.Scenes = SceneFactory.Instance.Serialize();
            Settings.Default.Save();
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

        void ShowWindowMenuItem_Click(object sender, EventArgs e)
        {
            RestoreFromTray();
        }

        void UpdateMenuItem_Click(object sender, EventArgs e)
        {
            CheckForUpdates();
        }

        #endregion
    }
}
