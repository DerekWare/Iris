using System;
using System.Windows.Forms;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common;
using DerekWare.HomeAutomation.Common.Effects;
using DerekWare.HomeAutomation.Common.Scenes;
using DerekWare.HomeAutomation.Common.Themes;

namespace DerekWare.Iris
{
    public partial class DeviceActionPanel : UserControl
    {
        readonly IDevice _Device;

        bool InUpdate;

        public DeviceActionPanel(IDevice device)
        {
            _Device = device;

            InitializeComponent();
        }

        public DeviceActionPanel(SceneItem sceneItem)
        {
            SceneItem = sceneItem;

            InitializeComponent();
        }

        public IDevice Device => SceneItem?.Device ?? _Device;

        public SceneItem SceneItem { get; }

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
            if(Device is not null)
            {
                Device.StateChanged += OnDeviceStateChanged;
                Device.PropertiesChanged += OnDevicePropertiesChanged;
            }

            UpdateProperties();
            UpdateState();
        }

        protected void DetachDevice()
        {
            if(Device is not null)
            {
                Device.StateChanged -= OnDeviceStateChanged;
                Device.PropertiesChanged -= OnDevicePropertiesChanged;
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

        void UpdateProperties()
        {
            PropertyGrid.SelectedObject = Device;
        }

        void UpdateState()
        {
            InUpdate = true;

            if(SceneItem is not null)
            {
                PowerStatePanel.Power = SceneItem.Power;
                SolidColorPanel.Color = SceneItem.Color;
                MultiZoneColorPanel.Colors = SceneItem.MultiZoneColors;
                ThemeButtonPanel.DeviceFamily = SceneItem.Family;
                ThemeButtonPanel.SelectedTheme = SceneItem.Theme;
                EffectButtonPanel.DeviceFamily = SceneItem.Family;
                EffectButtonPanel.SelectedEffect = SceneItem.Effect;
            }
            else
            {
                PowerStatePanel.Power = Device.Power;
                SolidColorPanel.Color = Device.Color;
                MultiZoneColorPanel.Colors = Device.MultiZoneColors;
                ThemeButtonPanel.DeviceFamily = Device.Family;
                ThemeButtonPanel.SelectedTheme = Device.Theme;
                EffectButtonPanel.DeviceFamily = Device.Family;
                EffectButtonPanel.SelectedEffect = Device.Effect;
            }

            InUpdate = false;
        }

        #region Event Handlers

        void EffectPanel_SelectedEffectChanged(object sender, SelectedEffectChangedEventArgs e)
        {
            if(InUpdate)
            {
                return;
            }

            var effect = EffectFactory.Instance.CreateInstance(e.Property.Name);

            if(DialogResult.OK == PropertyEditor.Show(this, effect))
            {
                EffectFactory.Instance.Add(effect);

                if(SceneItem is not null)
                {
                    SceneItem.Effect = effect;
                }

                if(Device is not null)
                {
                    Device.Effect = effect;
                }
            }
        }

        void MultiZoneColorPanel_ColorsChanged(object sender, ColorsChangedEventArgs e)
        {
            if(InUpdate)
            {
                return;
            }

            InUpdate = true;

            if(SceneItem is not null)
            {
                SceneItem.Effect = null;
                SceneItem.MultiZoneColors = e.Property;
            }

            if(Device is not null)
            {
                Device.Effect = null;
                Device.MultiZoneColors = e.Property;
            }

            InUpdate = false;
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

        void PowerStatePanel_PowerStateChanged(object sender, PowerStateChangedEventArgs e)
        {
            if(InUpdate)
            {
                return;
            }

            InUpdate = true;

            if(SceneItem is not null)
            {
                SceneItem.Power = e.Property;
            }

            if(Device is not null)
            {
                Device.Power = e.Property;
            }

            InUpdate = false;
        }

        void SolidColorPanel_ColorChanged(object sender, ColorChangedEventArgs e)
        {
            if(InUpdate)
            {
                return;
            }

            InUpdate = true;

            if(SceneItem is not null)
            {
                SceneItem.Effect = null;
                SceneItem.Color = e.Property;
            }

            if(Device is not null)
            {
                Device.Effect = null;
                Device.Color = e.Property;
            }

            InUpdate = false;
        }

        void ThemePanel_SelectedThemeChanged(object sender, SelectedThemeChangedEventArgs e)
        {
            if(InUpdate)
            {
                return;
            }

            var theme = ThemeFactory.Instance.CreateInstance(e.Property.Name);

            if(DialogResult.OK == PropertyEditor.Show(this, theme))
            {
                ThemeFactory.Instance.Add(theme);

                if(SceneItem is not null)
                {
                    SceneItem.Theme = theme;
                }

                if(Device is not null)
                {
                    Device.Theme = theme;
                }
            }
        }

        #endregion
    }
}
