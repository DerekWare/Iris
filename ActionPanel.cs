using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DerekWare.HomeAutomation.Common;
using DerekWare.HomeAutomation.Common.Effects;
using DerekWare.HomeAutomation.Common.Themes;
using DerekWare.Strings;
using PowerState = DerekWare.HomeAutomation.Common.PowerState;

namespace DerekWare.Iris
{
    public partial class ActionPanel : UserControl
    {
        bool InUpdate;

        public ActionPanel(IDevice device)
        {
            Device = device;

            InitializeComponent();
        }

        public IDevice Device { get; }

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

            var activeEffects = Device.Effects;

            foreach(var button in EffectLayoutPanel.Controls.OfType<Button>())
            {
                var effect = (IReadOnlyEffectProperties)button.Tag;
                var isActive = activeEffects.Contains(effect);

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
            var effect = EffectFactory.Instance.CreateInstance(((Button)sender).Text);

            if(DialogResult.OK != PropertyEditor.Show(this, effect))
            {
                return;
            }

            InUpdate = true;
            effect.Start(Device);
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
            EffectFactory.Instance.StopEffect(Device);
            Device.Power = powerState;
            InUpdate = false;
        }

        void SolidColorPanel_ColorChanged(object sender, ColorChangedEventArgs e)
        {
            InUpdate = true;
            EffectFactory.Instance.StopEffect(Device);
            Device.Color = e.Color;
            InUpdate = false;
        }

        void ThemeButton_Click(object sender, EventArgs e)
        {
            var theme = ThemeFactory.Instance.CreateInstance(((Button)sender).Text);

            if(DialogResult.OK != PropertyEditor.Show(this, theme))
            {
                return;
            }

            InUpdate = true;
            EffectFactory.Instance.StopEffect(Device);
            theme.Apply(Device);
            InUpdate = false;
        }

        void ZoneColorBand_ColorsChanged(object sender, ColorsChangedEventArgs e)
        {
            InUpdate = true;
            EffectFactory.Instance.StopEffect(Device);
            Device.MultiZoneColors = e.Colors;
            InUpdate = false;
        }

        #endregion
    }
}
