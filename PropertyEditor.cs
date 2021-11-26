using System;
using System.Linq;
using System.Windows.Forms;
using DerekWare.HomeAutomation.Common;

namespace DerekWare.Iris
{
    public partial class PropertyEditor : Form
    {
        public PropertyEditor(object obj)
        {
            InitializeComponent();

            PropertyGrid.SelectedObject = obj;
        }

        #region Event Handlers

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        protected void OkButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        #endregion

        public static DialogResult Show(IWin32Window owner, object obj)
        {
            // Don't bother showing anything if there's nothing to set
            if(!obj.GetWritableProperties().Any())
            {
                return DialogResult.OK;
            }

            // Load cached properties
            PropertyCache.Write(obj);

            // Show the dialog
            var result = new PropertyEditor(obj).ShowDialog(owner);

            if(DialogResult.OK != result)
            {
                return result;
            }

            // Save the properties from the editor
            PropertyCache.Read(obj);
            return result;
        }
    }
}
