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
            ThemePanel.DeviceFamily = SceneItem.Family;
            EffectPanel.DeviceFamily = SceneItem.Family;

            EnableCheckBoxes(this);
            UpdateUiFromScene();
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

        protected override void OnSelectedEffectChanged(Effect effect)
        {
            SceneItem.Effect = effect;
            EffectPanel.SelectedObject = effect;
            base.OnSelectedEffectChanged(effect);
        }

        protected override void OnSelectedThemeChanged(Theme theme)
        {
            SceneItem.Theme = theme;
            ThemePanel.SelectedObject = theme;
            base.OnSelectedThemeChanged(theme);
        }

        protected override void UpdateUiFromDevice()
        {
        }

        protected void UpdateUiFromScene()
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

                ThemePanel.GroupBox.Checked = false;
                ThemePanel.SelectedObject = null;

                EffectPanel.GroupBox.Checked = false;
                EffectPanel.SelectedObject = null;
            }
            else
            {
                if(SceneItem.Theme is not null)
                {
                    SolidColorPanel.GroupBox.Checked = false;
                    SolidColorPanel.Color = Colors.Black;

                    MultiZoneColorPanel.GroupBox.Checked = false;
                    MultiZoneColorPanel.Colors = new[] { Colors.Black };

                    ThemePanel.GroupBox.Checked = true;
                    ThemePanel.SelectedObject = SceneItem.Theme;
                }
                else if(SceneItem.Color is not null)
                {
                    if(SceneItem.Color.Count > 1)
                    {
                        SolidColorPanel.GroupBox.Checked = false;
                        SolidColorPanel.Color = Colors.Black;

                        MultiZoneColorPanel.GroupBox.Checked = true;
                        MultiZoneColorPanel.Colors = SceneItem.Color;
                        MultiZoneColorPanel.Enabled = true;

                        ThemePanel.GroupBox.Checked = false;
                        ThemePanel.SelectedObject = null;
                    }
                    else
                    {
                        SolidColorPanel.GroupBox.Checked = true;
                        SolidColorPanel.Color = SceneItem.Color.FirstOrDefault();

                        MultiZoneColorPanel.GroupBox.Checked = false;
                        MultiZoneColorPanel.Colors = new[] { Colors.Black };

                        ThemePanel.GroupBox.Checked = false;
                        ThemePanel.SelectedObject = null;
                    }
                }
                else
                {
                    SolidColorPanel.GroupBox.Checked = false;
                    SolidColorPanel.Color = Colors.Black;

                    MultiZoneColorPanel.GroupBox.Checked = false;
                    MultiZoneColorPanel.Colors = new[] { Colors.Black };

                    ThemePanel.GroupBox.Checked = false;
                    ThemePanel.SelectedObject = null;
                }

                if(SceneItem.Effect is not null)
                {
                    EffectPanel.GroupBox.Checked = true;
                    EffectPanel.SelectedObject = SceneItem.Effect;
                }
                else
                {
                    EffectPanel.GroupBox.Checked = false;
                    EffectPanel.SelectedObject = null;
                }
            }

            var enable = Device is not null;
            PowerStatePanel.Enabled = enable;

            enable = enable && (SceneItem.Power == PowerState.On);
            BrightnessPanel.Enabled = false; // TODO?
            SolidColorPanel.Enabled = enable;
            MultiZoneColorPanel.Enabled = enable;
            ThemePanel.Enabled = enable;
            ThemePanel.Enabled = enable;
            EffectPanel.Enabled = enable;
            EffectPanel.Enabled = enable;

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

        #region Event Handlers

        protected override void OnMultiZoneColorsChanged(object sender, ColorsChangedEventArgs e)
        {
            if(InUpdate)
            {
                return;
            }

            SceneItem.Color = e.Property;
            MultiZoneColorPanel.Colors = e.Property;
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
            base.OnPowerStateChanged(sender, e);
        }

        protected override void OnSolidColorChanged(object sender, ColorChangedEventArgs e)
        {
            if(InUpdate)
            {
                return;
            }

            SceneItem.Color = new[] { e.Property };
            SolidColorPanel.Color = e.Property;
            base.OnSolidColorChanged(sender, e);
        }

        void OnCheckedChanged(object sender, EventArgs e)
        {
            if(sender is not CheckGroupBox groupBox)
            {
                return;
            }

            if(groupBox.Checked)
            {
                if(sender == SolidColorPanel.GroupBox)
                {
                    MultiZoneColorPanel.GroupBox.Checked = false;
                    ThemePanel.GroupBox.Checked = false;
                    EffectPanel.GroupBox.Checked = false;
                }
                else if(sender == MultiZoneColorPanel.GroupBox)
                {
                    SolidColorPanel.GroupBox.Checked = false;
                    ThemePanel.GroupBox.Checked = false;
                    EffectPanel.GroupBox.Checked = false;
                }
                else if(sender == ThemePanel.GroupBox)
                {
                    SolidColorPanel.GroupBox.Checked = false;
                    MultiZoneColorPanel.GroupBox.Checked = false;
                }
            }
            else
            {
                if(sender == SolidColorPanel.GroupBox)
                {
                    SceneItem.Color = null;
                }
                else if(sender == MultiZoneColorPanel.GroupBox)
                {
                    SceneItem.Color = null;
                }
                else if(sender == ThemePanel.GroupBox)
                {
                    SceneItem.Theme = null;
                }
                else if(sender == EffectPanel.GroupBox)
                {
                    SceneItem.Effect = null;
                }
            }
        }

        void OnDevicePropertiesChanged(object sender, DeviceEventArgs e)
        {
            if(InvokeRequired)
            {
                BeginInvoke(new Action(() => OnDevicePropertiesChanged(sender, e)));
                return;
            }

            UpdateUiFromScene();
        }

        #endregion
    }
}
