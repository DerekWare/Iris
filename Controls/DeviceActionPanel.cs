using System;
using System.Windows.Forms;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.HomeAutomation.Common.Effects;
using DerekWare.HomeAutomation.Common.Themes;
using PowerState = DerekWare.HomeAutomation.Common.PowerState;

namespace DerekWare.Iris
{
    public partial class DeviceActionPanel : UserControl
    {
        protected bool InUpdate;

        readonly IDevice _Device;

        public DeviceActionPanel(IDevice device)
            : this()
        {
            _Device = device;
        }

        protected DeviceActionPanel()
        {
            InitializeComponent();
        }

        public virtual IDevice Device => _Device;

        public string Description
        {
            get => DescriptionLabel.Text;
            set
            {
                if(value.IsNullOrEmpty())
                {
                    DescriptionLabel.Text = null;
                    DescriptionLabel.Visible = false;
                    BaseLayoutPanel.RowStyles[0] = new RowStyle(SizeType.Absolute, 0);
                }
                else
                {
                    DescriptionLabel.Text = value;
                    DescriptionLabel.Visible = true;
                    BaseLayoutPanel.RowStyles[0] = new RowStyle(SizeType.AutoSize);
                }
            }
        }

        protected void AttachDevice()
        {
            if(_Device is not null)
            {
                _Device.StateChanged += OnDeviceStateChanged;
                _Device.PropertiesChanged += OnDevicePropertiesChanged;
            }

            UpdateProperties();
            UpdateState();
        }

        protected void DetachDevice()
        {
            if(_Device is not null)
            {
                _Device.StateChanged -= OnDeviceStateChanged;
                _Device.PropertiesChanged -= OnDevicePropertiesChanged;
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            AttachDevice();
            base.OnHandleCreated(e);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            DetachDevice();
            base.OnHandleDestroyed(e);
        }

        protected virtual bool OnSelectedEffectChanged(string name, out Effect effect)
        {
            effect = EffectFactory.Instance.CreateInstance(name);

            if(DialogResult.OK != PropertyEditor.Show(this, effect))
            {
                return false;
            }

            EffectFactory.Instance.Add(effect);
            return true;
        }

        protected virtual bool OnSelectedThemeChanged(string name, out Theme theme)
        {
            theme = ThemeFactory.Instance.CreateInstance(name);

            if(DialogResult.OK != PropertyEditor.Show(this, theme))
            {
                return false;
            }

            ThemeFactory.Instance.Add(theme);
            return true;
        }

        protected virtual void UpdateProperties()
        {
            PropertyGrid.SelectedObject = Device;
        }

        protected virtual void UpdateState()
        {
            if(InUpdate)
            {
                return;
            }
            
            InUpdate = true;

            PowerStatePanel.Power = Device?.Power ?? PowerState.Off;
            SolidColorPanel.Color = Device?.Color ?? Colors.Black;
            MultiZoneColorPanel.Colors = Device?.MultiZoneColors ?? new[] { Colors.Black };
            ThemeButtonPanel.DeviceFamily = Device?.Family;
            ThemeButtonPanel.SelectedTheme = Device?.Theme;
            EffectButtonPanel.DeviceFamily = Device?.Family;
            EffectButtonPanel.SelectedEffect = Device?.Effect;

            InUpdate = false;
        }

        #region Event Handlers

        protected virtual void OnMultiZoneColorsChanged(object sender, ColorsChangedEventArgs e)
        {
            if(InUpdate)
            {
                return;
            }

            InUpdate = true;

            if(Device is not null)
            {
                Device.MultiZoneColors = e.Property;
            }

            InUpdate = false;

            UpdateState();
        }

        protected virtual void OnPowerStateChanged(object sender, PowerStateChangedEventArgs e)
        {
            if(InUpdate)
            {
                return;
            }

            InUpdate = true;

            if(Device is not null)
            {
                Device.Power = e.Property;
            }

            InUpdate = false;

            UpdateState();
        }

        protected virtual void OnSelectedEffectChanged(object sender, SelectedEffectChangedEventArgs e)
        {
            if(InUpdate)
            {
                return;
            }

            if(!OnSelectedEffectChanged(e.Property.Name, out var effect))
            {
                return;
            }

            InUpdate = true;
            
            if(Device is not null)
            {
                Device.Effect = effect;
            }
            
            InUpdate = false;

            UpdateState();
        }

        protected virtual void OnSelectedThemeChanged(object sender, SelectedThemeChangedEventArgs e)
        {
            if(InUpdate)
            {
                return;
            }

            if(!OnSelectedThemeChanged(e.Property.Name, out var theme))
            {
                return;
            }

            InUpdate = true;

            if(Device is not null)
            {
                Device.Theme = theme;
            }

            InUpdate = false;

            UpdateState();
        }

        protected virtual void OnSolidColorChanged(object sender, ColorChangedEventArgs e)
        {
            if(InUpdate)
            {
                return;
            }

            InUpdate = true;

            if(Device is not null)
            {
                Device.Color = e.Property;
            }

            InUpdate = false;

            UpdateState();
        }

        void OnDevicePropertiesChanged(object sender, DeviceEventArgs e)
        {
            if(InvokeRequired)
            {
                BeginInvoke(new Action(() => OnDevicePropertiesChanged(sender, e)));
                return;
            }

            UpdateProperties();
        }

        void OnDeviceStateChanged(object sender, DeviceEventArgs e)
        {
            if(InvokeRequired)
            {
                BeginInvoke(new Action(() => OnDeviceStateChanged(sender, e)));
                return;
            }

            UpdateState();
        }

        #endregion
    }
}
