using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Forms;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common;
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
            Settings.Default.LifxDevices ??= new StringCollection();
            Settings.Default.LifxDevices.OfType<string>().ForEach(LifxClient.Instance.Connect);

            if(!Settings.Default.HueBridgeAddress.IsNullOrEmpty() && !Settings.Default.HueApiKey.IsNullOrEmpty())
            {
                HueClient.Instance.Connect(Settings.Default.HueBridgeAddress, Settings.Default.HueApiKey);
            }

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
        }

        #region Event Handlers

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
                EffectFactory.Instance.StopAll();
            }
            catch
            {
            }

            // Update the device list, pruning any addresses that were unreachable
            // TODO make a standard way of detecting valid devices
            Settings.Default.LifxDevices = new StringCollection();
            Settings.Default.LifxDevices.AddRange(LifxClient.Instance.Devices.WhereNotNull(i => i.Product)
                                                            .Select(i => i.Uuid)
                                                            .ToArray()); // TODO don't use Uuid?
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

        void ShowWindowMenuItem_Click(object sender, EventArgs e)
        {
            RestoreFromTray();
        }

        #endregion

        class TabSortComparer : IComparer<TabPage>
        {
            public static readonly TabSortComparer Instance = new();

            #region Equality

            public int Compare(TabPage x, TabPage y)
            {
                // Order: groups, devices
                if(x.Tag is IDeviceGroup)
                {
                    if(y.Tag is IDeviceGroup)
                    {
                        return x.Text.CompareTo(y.Text);
                    }

                    return -1;
                }

                if(y.Tag is IDeviceGroup)
                {
                    return 1;
                }

                return x.Text.CompareTo(y.Text);
            }

            #endregion
        }
    }
}
