using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using DerekWare.Collections;
using DerekWare.Diagnostics;
using DerekWare.HomeAutomation.Common;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.HomeAutomation.Common.Effects;
using DerekWare.HomeAutomation.Common.Themes;
using PowerState = DerekWare.HomeAutomation.Common.PowerState;

namespace DerekWare.Iris
{
    public partial class DeviceActionPanel : UserControl
    {
        readonly IDevice _Device;
        int _InUpdateCounter;

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

        protected bool InUpdate
        {
            get => _InUpdateCounter > 0;
            set
            {
                if(value)
                {
                    var n = Interlocked.Increment(ref _InUpdateCounter);
                    Debug.Assert(n >= 1);
                }
                else
                {
                    var n = Interlocked.Decrement(ref _InUpdateCounter);
                    Debug.Assert(n >= 0);
                }
            }
        }

        protected void AttachDevice()
        {
            if(_Device is not null)
            {
                _Device.StateChanged += OnDeviceStateChanged;
                _Device.PropertiesChanged += OnDevicePropertiesChanged;
                _Device.RefreshState();
            }

            UpdateProperties();
            UpdateUiFromDevice();
        }

        protected virtual bool CreateEffect(IReadOnlyEffectProperties properties, out Effect effect)
        {
            effect = EffectFactory.Instance.CreateInstance(properties.Name);

            if(DialogResult.OK != PropertyEditor.Show(this, effect))
            {
                return false;
            }

            EffectFactory.Instance.Add(effect);
            return true;
        }

        protected virtual bool CreateTheme(IReadOnlyThemeProperties properties, out Theme theme)
        {
            theme = ThemeFactory.Instance.CreateInstance(properties.Name);

            if(DialogResult.OK != PropertyEditor.Show(this, theme))
            {
                return false;
            }

            ThemeFactory.Instance.Add(theme);
            return true;
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

        protected virtual void OnSelectedEffectChanged(Effect effect)
        {
            if(InUpdate || Device is null)
            {
                return;
            }

            InUpdate = true;
            Device.Theme = null;
            Device.Power = PowerState.On;
            Device.Effect = effect;
            InUpdate = false;

            UpdateUiFromDevice();
        }

        protected virtual void OnSelectedThemeChanged(Theme theme)
        {
            if(InUpdate || Device is null)
            {
                return;
            }

            InUpdate = true;
            Device.Effect = null;
            Device.Power = PowerState.On;
            Device.Theme = theme;
            InUpdate = false;

            UpdateUiFromDevice();
        }

        protected virtual void UpdateProperties()
        {
            PropertyGrid.SelectedObject = Device;
        }

        protected virtual void UpdateUiFromDevice()
        {
            InUpdate = true;

            PowerStatePanel.Power = Device?.Power ?? PowerState.Off;
            PowerStatePanel.Enabled = Device is not null;

            BrightnessPanel.Brightness = Device?.Brightness ?? 0;
            BrightnessPanel.Enabled = Device is not null;

            SolidColorPanel.Color = Device?.Color?.FirstOrDefault() ?? Colors.Black;
            SolidColorPanel.Enabled = Device is not null;

            MultiZoneColorPanel.Colors = Device?.Color ?? new[] { Colors.Black };
            MultiZoneColorPanel.Enabled = Device?.IsMultiZone ?? false;

            ThemePanel.DeviceFamily = Device?.Family;
            ThemePanel.SelectedObject = Device?.Theme;
            ThemePanel.Enabled = Device is not null;

            EffectPanel.DeviceFamily = Device?.Family;
            EffectPanel.SelectedObject = Device?.Effect;
            EffectPanel.Enabled = Device is not null;

            InUpdate = false;
        }

        #region Event Handlers

        protected virtual void OnBrightnessChanged(object sender, BrightnessChangedEventArgs e)
        {
            if(InUpdate || Device is null)
            {
                return;
            }

            InUpdate = true;
            Device.Effect = null;
            Device.Power = PowerState.On;
            Device.Brightness = e.Property;
            InUpdate = false;

            UpdateUiFromDevice();
        }

        protected virtual void OnMultiZoneColorsChanged(object sender, ColorsChangedEventArgs e)
        {
            if(InUpdate || Device is null)
            {
                return;
            }

            InUpdate = true;
            Device.Theme = null;
            Device.Effect = null;
            Device.Power = PowerState.On;
            Device.Color = e.Property;
            InUpdate = false;

            UpdateUiFromDevice();
        }

        protected virtual void OnPowerStateChanged(object sender, PowerStateChangedEventArgs e)
        {
            if(InUpdate || Device is null)
            {
                return;
            }

            InUpdate = true;
            Device.Power = e.Property;
            InUpdate = false;

            UpdateUiFromDevice();
        }

        protected virtual void OnSelectedEffectClicked(object sender, PropertyChangedEventArgs<IReadOnlyEffectProperties> e)
        {
            if(InUpdate)
            {
                return;
            }

            if(!CreateEffect(e.Property, out var effect))
            {
                return;
            }

            OnSelectedEffectChanged(effect);
        }

        protected virtual void OnSelectedThemeClicked(object sender, PropertyChangedEventArgs<IReadOnlyThemeProperties> e)
        {
            if(InUpdate)
            {
                return;
            }

            if(!CreateTheme(e.Property, out var theme))
            {
                return;
            }

            OnSelectedThemeChanged(theme);
            UpdateUiFromDevice();
        }

        protected virtual void OnSolidColorChanged(object sender, ColorChangedEventArgs e)
        {
            if(InUpdate || Device is null)
            {
                return;
            }

            InUpdate = true;
            Device.Theme = null;
            Device.Effect = null;
            Device.Power = PowerState.On;
            Device.Color = new [] { e.Property };
            InUpdate = false;

            UpdateUiFromDevice();
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

            UpdateUiFromDevice();
        }

        #endregion
    }
}
