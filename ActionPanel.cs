using System;
using System.Linq;
using System.Windows.Forms;
using DerekWare.HomeAutomation.Common;
using DerekWare.HomeAutomation.Common.Scenes;
using DerekWare.Strings;
using PowerState = DerekWare.HomeAutomation.Common.PowerState;

namespace DerekWare.Iris
{
    public partial class ActionPanel : UserControl
    {
        bool InUpdate;

        public ActionPanel(IDevice device)
        {
            Tag = device;

            InitializeComponent();
        }

        public IDevice Device => (IDevice)Tag;

        protected override void OnLoad(EventArgs e)
        {
            Device.StateChanged += OnDeviceStateChanged;
            Device.PropertiesChanged += OnDevicePropertiesChanged;

            UpdateScenes();
            UpdateEffects();
            UpdateProperties();
            UpdateState();

            base.OnLoad(e);
        }

        protected void UpdateEffects()
        {
            EffectLayoutPanel.Controls.Clear();

            foreach(var properties in EffectFactory.Instance)
            {
                var button = new Button
                {
                    Dock = DockStyle.Fill,
                    Text = properties.Name,
                    Tag = properties,
                    Enabled = properties.Family.IsNullOrEmpty() || properties.Family.Equals(Device.Family)
                };

                button.Click += EffectButton_Click;
                EffectLayoutPanel.Controls.Add(button);
            }
        }

        protected void UpdateScenes()
        {
            SceneLayoutPanel.Controls.Clear();

            foreach(var properties in SceneFactory.Instance)
            {
                var button = new Button
                {
                    Dock = DockStyle.Fill,
                    Text = properties.Name,
                    Tag = properties,
                    Enabled = properties.Family.IsNullOrEmpty() || properties.Family.Equals(Device.Family)
                };

                button.Click += SceneButton_Click;
                SceneLayoutPanel.Controls.Add(button);
            }
        }

        void UpdateProperties()
        {
            PropertyGrid.SelectedObject = Device;
        }

        void UpdateState()
        {
            InUpdate = true;

            PowerComboBox.SelectedIndex = (int)Device.Power;
            SolidColorPanel.Color = Device.Color;
            ZoneColorBand.Colors = Device.MultiZoneColors.ToArray();

            // TODO
#if false
            var activeEffects = GetDevices().Select(i => i.ActiveEffect).WhereNotNull().Select(i => i.GetType()).ToHashSet();

            foreach(var button in EffectLayoutPanel.Controls.OfType<Button>())
            {
                var effectType = (Type)button.Tag;
                var isActive = activeEffects.Contains(effectType);

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
#endif

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
            EffectFactory.Instance.Stop(Device);
            Device.Power = powerState;
            InUpdate = false;
        }

        void SceneButton_Click(object sender, EventArgs e)
        {
            var scene = SceneFactory.Instance.CreateInstance(((Button)sender).Text);

            if(DialogResult.OK != PropertyEditor.Show(this, scene))
            {
                return;
            }

            InUpdate = true;
            EffectFactory.Instance.Stop(Device);
            scene.Apply(Device);
            InUpdate = false;
        }

        void SolidColorPanel_ColorChanged(object sender, ColorChangedEventArgs e)
        {
            InUpdate = true;
            EffectFactory.Instance.Stop(Device);
            Device.Color = e.Color;
            InUpdate = false;
        }

        void ZoneColorBand_ColorsChanged(object sender, ColorBandColorsChangedEventArgs e)
        {
            InUpdate = true;
            EffectFactory.Instance.Stop(Device);
            Device.MultiZoneColors = e.Colors;
            InUpdate = false;
        }

        #endregion
    }
}
