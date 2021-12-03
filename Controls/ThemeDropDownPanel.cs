using System;
using System.ComponentModel;
using System.Windows.Forms;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common;
using DerekWare.HomeAutomation.Common.Themes;

namespace DerekWare.Iris
{
    public partial class ThemeDropDownPanel : UserControl
    {
        string _DeviceFamily;
        IReadOnlyThemeProperties _SelectedTheme;
        bool InUpdate;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true)]
        public event EventHandler<SelectedThemeChangedEventArgs> SelectedThemeChanged;

        public ThemeDropDownPanel()
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
        public IReadOnlyThemeProperties SelectedTheme
        {
            get => (IReadOnlyThemeProperties)ThemeComboBox.SelectedItem;
            set
            {
                _SelectedTheme = value;
                UpdateState();
            }
        }

        void UpdateState()
        {
            foreach(var theme in ThemeFactory.Instance)
            {
                if(theme.IsCompatible(DeviceFamily))
                {
                    if(!ThemeComboBox.Items.Contains(theme))
                    {
                        ThemeComboBox.Items.Add(theme);
                    }
                }
            }

            ThemeComboBox.Items.RemoveWhere<IReadOnlyThemeProperties>(theme => !ThemeFactory.Instance.Contains(theme) || !theme.IsCompatible(DeviceFamily));

            InUpdate = true;
            ThemeComboBox.SelectedItem = SelectedTheme;
            InUpdate = false;
        }

        #region Event Handlers

        void ThemeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(InUpdate)
            {
                return;
            }

            SelectedThemeChanged?.Invoke(this, new SelectedThemeChangedEventArgs { Property = SelectedTheme });
        }

        #endregion
    }
}
