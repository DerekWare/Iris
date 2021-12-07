using System;
using System.Linq;
using System.Windows.Forms;
using DerekWare.HomeAutomation.Common;
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.HomeAutomation.Common.Effects;
using DerekWare.HomeAutomation.Common.Scenes;
using DerekWare.HomeAutomation.Common.Themes;
using PowerState = DerekWare.HomeAutomation.Common.PowerState;

namespace DerekWare.Iris
{
    public class SceneItemPanel : DeviceActionPanel
    {
        public SceneItemPanel(SceneItem sceneItem)
        {
            SceneItem = sceneItem;
            ThemeButtonPanel.DeviceFamily = SceneItem.Family;
            EffectButtonPanel.DeviceFamily = SceneItem.Family;

            EnableCheckBoxes(this);
            UpdateState();
        }

        public IClient Client => SceneItem.Client;

        public override IDevice Device => SceneItem.Device;

        public SceneItem SceneItem { get; }

        protected override bool CreateEffect(IReadOnlyEffectProperties properties, out Effect effect)
        {
            // If the effect is the same as what the scene item already uses, show
            // its saved properties rather than the ones cached in the factory.
            if(SceneItem.Effect?.Matches(properties) ?? false)
            {
                if(DialogResult.OK == PropertyEditor.Show(this, SceneItem.Effect))
                {
                    effect = SceneItem.Effect;
                    return true;
                }

                effect = null;
                return false;
            }

            return base.CreateEffect(properties, out effect);
        }

        protected override bool CreateTheme(IReadOnlyThemeProperties properties, out Theme theme)
        {
            // If the theme is the same as what the scene item already uses, show
            // its saved properties rather than the ones cached in the factory.
            if(SceneItem.Theme?.Matches(properties) ?? false)
            {
                if(DialogResult.OK == PropertyEditor.Show(this, SceneItem.Theme))
                {
                    theme = SceneItem.Theme;
                    return true;
                }

                theme = null;
                return false;
            }

            return base.CreateTheme(properties, out theme);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            if(Client is not null)
            {
                Client.PropertiesChanged += OnDevicePropertiesChanged;
            }

            base.OnHandleCreated(e);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            if(Client is not null)
            {
                Client.PropertiesChanged -= OnDevicePropertiesChanged;
            }

            base.OnHandleDestroyed(e);
        }

        protected override void UpdateState()
        {
            InUpdate = true;

            PowerStatePanel.Power = SceneItem.Power;
            PowerStatePanel.GroupBox.Checked = true;

            if(PowerStatePanel.Power == PowerState.Off)
            {
                SolidColorPanel.GroupBox.Checked = false;
                SolidColorPanel.Color = Colors.Black;

                MultiZoneColorPanel.GroupBox.Checked = false;
                MultiZoneColorPanel.Colors = new[] { Colors.Black };

                ThemeButtonPanel.GroupBox.Checked = false;
                ThemeButtonPanel.SelectedTheme = null;

                EffectButtonPanel.GroupBox.Checked = false;
                EffectButtonPanel.SelectedEffect = null;
            }
            else
            {
                if(SceneItem.Theme is not null)
                {
                    SolidColorPanel.GroupBox.Checked = false;
                    SolidColorPanel.Color = Colors.Black;

                    MultiZoneColorPanel.GroupBox.Checked = false;
                    MultiZoneColorPanel.Colors = new[] { Colors.Black };

                    ThemeButtonPanel.GroupBox.Checked = true;
                    ThemeButtonPanel.SelectedTheme = SceneItem.Theme;
                }
                else if(SceneItem.MultiZoneColors is not null)
                {
                    SolidColorPanel.GroupBox.Checked = false;
                    SolidColorPanel.Color = Colors.Black;

                    MultiZoneColorPanel.GroupBox.Checked = true;
                    MultiZoneColorPanel.Colors = SceneItem.MultiZoneColors;
                    MultiZoneColorPanel.Enabled = true;

                    ThemeButtonPanel.GroupBox.Checked = false;
                    ThemeButtonPanel.SelectedTheme = null;
                }
                else if(SceneItem.Color is not null)
                {
                    SolidColorPanel.GroupBox.Checked = true;
                    SolidColorPanel.Color = SceneItem.Color;

                    MultiZoneColorPanel.GroupBox.Checked = false;
                    MultiZoneColorPanel.Colors = new[] { Colors.Black };

                    ThemeButtonPanel.GroupBox.Checked = false;
                    ThemeButtonPanel.SelectedTheme = null;
                }
                else
                {
                    SolidColorPanel.GroupBox.Checked = false;
                    SolidColorPanel.Color = Colors.Black;

                    MultiZoneColorPanel.GroupBox.Checked = false;
                    MultiZoneColorPanel.Colors = new[] { Colors.Black };

                    ThemeButtonPanel.GroupBox.Checked = false;
                    ThemeButtonPanel.SelectedTheme = null;
                }

                if(SceneItem.Effect is not null)
                {
                    EffectButtonPanel.GroupBox.Checked = true;
                    EffectButtonPanel.SelectedEffect = SceneItem.Effect;
                }
                else
                {
                    EffectButtonPanel.GroupBox.Checked = false;
                    EffectButtonPanel.SelectedEffect = null;
                }
            }

            var enable = Device is not null;
            PowerStatePanel.Enabled = enable;

            enable = enable && (SceneItem.Power == PowerState.On);
            BrightnessPanel.Enabled = false; // TODO?
            SolidColorPanel.Enabled = enable;
            MultiZoneColorPanel.Enabled = enable;
            ThemeButtonPanel.Enabled = enable;
            ThemeButtonPanel.Enabled = enable;
            EffectButtonPanel.Enabled = enable;
            EffectButtonPanel.Enabled = enable;

            InUpdate = false;
        }

        void EnableCheckBoxes(Control parent)
        {
            foreach(var i in parent.Controls.OfType<CheckGroupBox>())
            {
                if(PowerStatePanel.GroupBox == i)
                {
                    i.ShowCheckBox = false;
                    continue;
                }

                if(BrightnessPanel.GroupBox == i)
                {
                    i.ShowCheckBox = false;
                    continue;
                }

                if(i.ShowCheckBox)
                {
                    continue;
                }

                i.ShowCheckBox = true;
                i.ApplyChildState = true;
                i.CheckedChanged += OnCheckedChanged;
            }

            foreach(Control i in parent.Controls)
            {
                EnableCheckBoxes(i);
            }
        }

        void UpdateScene()
        {
            SceneItem.Power = PowerStatePanel.Power;
            SceneItem.Color = SolidColorPanel.GroupBox.Checked ? SolidColorPanel.Color : null;
            SceneItem.MultiZoneColors = MultiZoneColorPanel.GroupBox.Checked ? MultiZoneColorPanel.Colors : null;
            SceneItem.Theme = ThemeButtonPanel.GroupBox.Checked ? ThemeFactory.Instance.CreateInstance(ThemeButtonPanel.SelectedTheme.Name) : null;
            SceneItem.Effect = EffectButtonPanel.GroupBox.Checked ? EffectFactory.Instance.CreateInstance(EffectButtonPanel.SelectedEffect.Name) : null;
        }

        #region Event Handlers

        protected override void OnMultiZoneColorsChanged(object sender, ColorsChangedEventArgs e)
        {
            if(InUpdate)
            {
                return;
            }

            SceneItem.MultiZoneColors = e.Property;
            MultiZoneColorPanel.Colors = e.Property;
            UpdateScene();
            base.OnMultiZoneColorsChanged(sender, e);
        }

        protected override void OnPowerStateChanged(object sender, PowerStateChangedEventArgs e)
        {
            if(InUpdate)
            {
                return;
            }

            SceneItem.Power = e.Property;
            PowerStatePanel.Power = e.Property;
            UpdateScene();
            base.OnPowerStateChanged(sender, e);
        }

        protected override void OnSelectedEffectChanged(object sender, SelectedEffectChangedEventArgs e)
        {
            EffectButtonPanel.SelectedEffect = e.Property;
            base.OnSelectedEffectChanged(sender, e);
        }

        protected override void OnSelectedThemeChanged(object sender, SelectedThemeChangedEventArgs e)
        {
            ThemeButtonPanel.SelectedTheme = e.Property;
            base.OnSelectedThemeChanged(sender, e);
        }

        protected override void OnSolidColorChanged(object sender, ColorChangedEventArgs e)
        {
            if(InUpdate)
            {
                return;
            }

            SceneItem.Color = e.Property;
            SolidColorPanel.Color = e.Property;
            UpdateScene();
            base.OnSolidColorChanged(sender, e);
        }

        void OnCheckedChanged(object sender, EventArgs e)
        {
            if(InUpdate)
            {
                return;
            }

            var groupBox = sender as CheckGroupBox;

            if(groupBox is null)
            {
                return;
            }

            if(!groupBox.Checked)
            {
                UpdateScene();
                return;
            }

            InUpdate = true;

            if(sender == SolidColorPanel.GroupBox)
            {
                MultiZoneColorPanel.GroupBox.Checked = false;
                ThemeButtonPanel.GroupBox.Checked = false;
                EffectButtonPanel.GroupBox.Checked = false;
            }
            else if(sender == MultiZoneColorPanel.GroupBox)
            {
                SolidColorPanel.GroupBox.Checked = false;
                ThemeButtonPanel.GroupBox.Checked = false;
                EffectButtonPanel.GroupBox.Checked = false;
            }
            else if(sender == ThemeButtonPanel.GroupBox)
            {
                SolidColorPanel.GroupBox.Checked = false;
                MultiZoneColorPanel.GroupBox.Checked = false;
            }

            UpdateScene();

            InUpdate = true;
        }

        void OnDevicePropertiesChanged(object sender, DeviceEventArgs e)
        {
            if(InvokeRequired)
            {
                BeginInvoke(new Action(() => OnDevicePropertiesChanged(sender, e)));
                return;
            }

            UpdateState();
        }

        #endregion
    }
}
