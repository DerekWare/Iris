using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using AutoUpdaterDotNET;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common.Effects;
using DerekWare.Iris.Properties;
using DerekWare.Strings;
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
            AutoUpdater.InstalledVersion = new Version(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion);
            
            // the reminder and skip both have a bug, so don't use them
            AutoUpdater.ShowRemindLaterButton = false;
            AutoUpdater.ShowSkipButton = false;

            Settings.Default.LifxDevices ??= new StringCollection();
            Settings.Default.LifxDevices.OfType<string>().ForEach(LifxClient.Instance.Connect);

            if(!Settings.Default.HueBridgeAddress.IsNullOrEmpty() && !Settings.Default.HueApiKey.IsNullOrEmpty())
            {
                HueClient.Instance.Connect(Settings.Default.HueBridgeAddress, Settings.Default.HueApiKey);
            }

            CheckForUpdates();
            base.OnLoad(e);
        }

        protected override void OnResize(EventArgs e)
        {
            if(FormWindowState.Minimized == WindowState)
            {
                MinimizeToTray();
            }

            base.OnResize(e);
        }

        void CheckForUpdates()
        {
            AutoUpdater.Start("http://www.derekware.com/software/iris/AutoUpdater.xml");
        }

        void MinimizeToTray()
        {
            RestoreWindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Maximized : FormWindowState.Normal;
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
        }

        void CloseMenuItem_Click(object sender, EventArgs e)
        {
            Close();
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

        void DeviceTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            foreach(var i in RootLayoutPanel.Controls.OfType<ActionPanel>())
            {
                RootLayoutPanel.Controls.Remove(i);
                i.Dispose();
            }

            if(e.Node is not DeviceTreeView.DeviceNode node)
            {
                return;
            }

            node.Device.RefreshState();
            RootLayoutPanel.Controls.Add(new ActionPanel(node.Device) { Dock = DockStyle.Fill }, 1, 0);
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
