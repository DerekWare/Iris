using System;
using System.Windows.Forms;
using DerekWare.Collections;
using DerekWare.Diagnostics;
using DerekWare.HomeAutomation.PhilipsHue;
using HueClient = DerekWare.HomeAutomation.PhilipsHue.Client;

namespace DerekWare.Iris
{
    public partial class ConnectBridgeDialog : Form
    {
        public ConnectBridgeDialog()
        {
            HueClient.Instance.BridgeDiscovered += OnBridgeDiscovered;

            InitializeComponent();

            YupButton.Enabled = false;
            HueClient.Instance.BeginBridgeDiscovery();
        }

        public string ApiKey { get; private set; }
        public string EntertainmentKey { get; private set; }

        public string IpAddress
        {
            get
            {
                if(IpAddressComboBox.SelectedItem is Bridge bridge)
                {
                    return bridge.IpAddress;
                }

                return IpAddressComboBox.Text;
            }
            set => IpAddressComboBox.Text = value;
        }

        bool LooksLikeAnAddress
        {
            get
            {
                var s = IpAddress.Split('.');

                if(s.Length != 4)
                {
                    return false;
                }

                foreach(var i in s)
                {
                    if(!int.TryParse(i, out var n))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        #region Event Handlers

        void ConnectBridgeDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            HueClient.Instance.CancelBridgeDiscovery();
        }

        void IpAddressComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            YupButton.Enabled = !IpAddress.IsNullOrEmpty();
        }

        void IpAddressComboBox_TextUpdate(object sender, EventArgs e)
        {
            YupButton.Enabled = !IpAddress.IsNullOrEmpty();
        }

        void NopeButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        void OnBridgeDiscovered(object sender, BridgeEventArgs e)
        {
            if(InvokeRequired)
            {
                BeginInvoke(new Action(() => OnBridgeDiscovered(sender, e)));
                return;
            }

            IpAddressComboBox.Items.Add(e.Bridge);

            if(IpAddressComboBox.SelectedItem is null)
            {
                IpAddressComboBox.SelectedItem = e.Bridge;
            }
        }

        async void YupButton_Click(object sender, EventArgs e)
        {
            try
            {
                var result = await HueClient.Register(IpAddress);
                ApiKey = result.Username;
                EntertainmentKey = result.StreamingClientKey;
            }
            catch(Exception ex)
            {
                Debug.Error(this, ex);
                MessageBox.Show(this, ex.Message, ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        #endregion
    }
}
