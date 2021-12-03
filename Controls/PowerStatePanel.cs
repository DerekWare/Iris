using System;
using System.ComponentModel;
using System.Windows.Forms;
using Enum = DerekWare.Reflection.Enum;
using PowerState = DerekWare.HomeAutomation.Common.PowerState;

namespace DerekWare.Iris
{
    public partial class PowerStatePanel : UserControl
    {
        PowerState _Power;
        bool InUpdate;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true)]
        public event EventHandler<PowerStateChangedEventArgs> PowerStateChanged;

        public PowerStatePanel()
        {
            InitializeComponent();

            if(Extensions.IsDesignMode())
            {
                return;
            }

            InUpdate = true;

            foreach(var value in Enum.GetValues<PowerState>())
            {
                PowerStateComboBox.Items.Add(value);
            }

            InUpdate = false;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public PowerState Power
        {
            get => _Power;
            set
            {
                _Power = value;
                InUpdate = true;
                PowerStateComboBox.SelectedItem = value;
                InUpdate = false;
            }
        }

        #region Event Handlers

        void PowerComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(InUpdate)
            {
                return;
            }

            PowerStateChanged?.Invoke(this, new PowerStateChangedEventArgs { Property = (PowerState)PowerStateComboBox.SelectedItem });
        }

        #endregion
    }
}
