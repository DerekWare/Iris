using System;
using System.ComponentModel;
using System.Windows.Forms;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common;
using DerekWare.HomeAutomation.Common.Effects;

namespace DerekWare.Iris
{
    public partial class EffectDropDownPanel : UserControl
    {
        string _DeviceFamily;
        IReadOnlyEffectProperties _SelectedEffect;
        bool InUpdate;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true)]
        public event EventHandler<SelectedEffectChangedEventArgs> SelectedEffectChanged;

        public EffectDropDownPanel()
        {
            InitializeComponent();

            if(Extensions.IsDesignMode())
            {
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public string DeviceFamily
        {
            get => _DeviceFamily;
            set
            {
                _DeviceFamily = value;
                UpdateState();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public IReadOnlyEffectProperties SelectedEffect
        {
            get => (IReadOnlyEffectProperties)EffectComboBox.SelectedItem;
            set
            {
                _SelectedEffect = value;
                UpdateState();
            }
        }

        void UpdateState()
        {
            foreach(var effect in EffectFactory.Instance)
            {
                if(effect.IsCompatible(DeviceFamily))
                {
                    if(!EffectComboBox.Items.Contains(effect))
                    {
                        EffectComboBox.Items.Add(effect);
                    }
                }
            }

            EffectComboBox.Items.RemoveWhere<IReadOnlyEffectProperties>(
                effect => !EffectFactory.Instance.Contains(effect) || !effect.IsCompatible(DeviceFamily));

            InUpdate = true;
            EffectComboBox.SelectedItem = SelectedEffect;
            InUpdate = false;
        }

        #region Event Handlers

        void EffectComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(InUpdate)
            {
                return;
            }

            SelectedEffectChanged?.Invoke(this, new SelectedEffectChangedEventArgs { Property = SelectedEffect });
        }

        #endregion
    }
}
