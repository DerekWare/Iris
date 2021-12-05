using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DerekWare.HomeAutomation.Common;

namespace DerekWare.Iris
{
    public partial class DeviceSelectionForm : Form
    {
        public DeviceSelectionForm()
        {
            InitializeComponent();
        }

        #region Event Handlers

        void AcceptButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        #endregion

        public static bool Show(IWin32Window owner, ref IReadOnlyCollection<IDevice> devices)
        {
            var form = new DeviceSelectionForm();
            form.DeviceTreeView.CheckedDevices = devices;

            var result = form.ShowDialog(owner);

            if(DialogResult.OK != result)
            {
                devices = null;
                return false;
            }

            devices = form.DeviceTreeView.CheckedDevices;
            return true;
        }
    }
}
