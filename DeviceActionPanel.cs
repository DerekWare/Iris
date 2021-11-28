using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common;
using DerekWare.HomeAutomation.Common.Effects;
using DerekWare.HomeAutomation.Common.Themes;
using PowerState = DerekWare.HomeAutomation.Common.PowerState;

namespace DerekWare.Iris
{
    public partial class DeviceActionPanel : UserControl
    {
        bool InUpdate;

        public DeviceActionPanel(IDevice device)
        {
            Device = device;

            InitializeComponent();
        }

        public IDevice Device { get; }

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
            Device.StateChanged += OnDeviceStateChanged;
            Device.PropertiesChanged += OnDevicePropertiesChanged;

            UpdateThemes();
            UpdateEffects();
            UpdateProperties();
            UpdateState();
        }

        protected void DetachDevice()
        {
            Device.StateChanged -= OnDeviceStateChanged;
            Device.PropertiesChanged -= OnDevicePropertiesChanged;
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

        protected void UpdateEffects()
        {
            EffectLayoutPanel.Controls.Clear();

            foreach(var effect in EffectFactory.Instance)
            {
                var button = new Button
                {
                    Dock = DockStyle.Fill, Text = effect.Name, Tag = effect, Enabled = effect.Family.IsNullOrEmpty() || effect.Family.Equals(Device.Family)
                };

                if(!effect.Description.IsNullOrEmpty())
                {
                    new ToolTip().SetToolTip(button, effect.Description);
                }

                button.Click += EffectButton_Click;
                EffectLayoutPanel.Controls.Add(button);
            }
        }

        protected void UpdateThemes()
        {
            ThemeLayoutPanel.Controls.Clear();

            foreach(var theme in ThemeFactory.Instance)
            {
                var button = new Button
                {
                    Dock = DockStyle.Fill, Text = theme.Name, Tag = theme, Enabled = theme.Family.IsNullOrEmpty() || theme.Family.Equals(Device.Family)
                };

                if(!theme.Description.IsNullOrEmpty())
                {
                    new ToolTip().SetToolTip(button, theme.Description);
                }

                button.Click += ThemeButton_Click;
                ThemeLayoutPanel.Controls.Add(button);
            }
        }

        void UpdateProperties()
        {
            PropertyGrid.SelectedObject = Device;
        }

        void UpdateState()
        {
            InUpdate = true;

            if(!PowerComboBox.DroppedDown)
            {
                PowerComboBox.SelectedIndex = (int)Device.Power;
            }

            SolidColorPanel.Color = Device.Color;
            ZoneColorBand.Colors = Device.MultiZoneColors.ToArray();

            foreach(var button in EffectLayoutPanel.Controls.OfType<Button>())
            {
                var effect = (Effect)button.Tag;
                var isActive = Device.Effect == effect;

                if(isActive)
                {
                    button.BackColor = SystemColors.Highlight;
                    button.ForeColor = SystemColors.HighlightText;
                }
                else
                {
                    button.BackColor = default;
                    button.ForeColor = default;
                    button.UseVisualStyleBackColor = true;
                }
            }

            InUpdate = false;
        }

        #region Event Handlers

        void EffectButton_Click(object sender, EventArgs e)
        {
            // Create the effect
            var effect = EffectFactory.Instance.CreateInstance(((Button)sender).Text);

            if(DialogResult.OK != PropertyEditor.Show(this, effect))
            {
                return;
            }

            // Cache property edits
            EffectFactory.Instance.Add(effect);

            // Apply the effect
            InUpdate = true;
            Device.Effect = effect;
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

        void PowerComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(InUpdate)
            {
                return;
            }

            var powerState = (PowerState)PowerComboBox.SelectedIndex;

            InUpdate = true;
            Device.Power = powerState;
            InUpdate = false;
        }

        void SolidColorPanel_ColorChanged(object sender, ColorChangedEventArgs e)
        {
            InUpdate = true;
            Device.Effect = null;
            Device.Color = e.Color;
            InUpdate = false;
        }

        void ThemeButton_Click(object sender, EventArgs e)
        {
            // Create the them
            var theme = ThemeFactory.Instance.CreateInstance(((Button)sender).Text);

            if(DialogResult.OK != PropertyEditor.Show(this, theme))
            {
                return;
            }

            // Cache property edits
            ThemeFactory.Instance.Add(theme);

            // Apply the theme
            InUpdate = true;
            Device.Effect = null;
            theme.Apply(Device);
            InUpdate = false;
        }

        void ZoneColorBand_ColorsChanged(object sender, ColorsChangedEventArgs e)
        {
            InUpdate = true;
            Device.Effect = null;
            Device.MultiZoneColors = e.Colors;
            InUpdate = false;
        }

        #endregion
    }
}
