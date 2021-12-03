using System;
using System.Windows.Forms;

namespace DerekWare.Iris
{
    public partial class ConnectDeviceDialog : Form
    {
        public ConnectDeviceDialog()
        {
            InitializeComponent();

            YupButton.Enabled = LooksLikeAnAddress;
        }

        public string IpAddress { get => IpAddressTextBox.Text; set => IpAddressTextBox.Text = value; }

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

        void IpAddressTextBox_TextChanged(object sender, EventArgs e)
        {
            YupButton.Enabled = LooksLikeAnAddress;
        }

        void NopeButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        void YupButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        #endregion
    }
}
