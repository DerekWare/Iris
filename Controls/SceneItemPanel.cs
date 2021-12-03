using System;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common;
using DerekWare.HomeAutomation.Common.Effects;
using DerekWare.HomeAutomation.Common.Scenes;
using DerekWare.HomeAutomation.Common.Themes;

namespace DerekWare.Iris
{
    public class SceneItemPanel : DeviceActionPanel
    {
        public SceneItemPanel(SceneItem sceneItem)
        {
            SceneItem = sceneItem;
        }

        public IClient Client => SceneItem.Client;

        public override IDevice Device => SceneItem.Device;

        public SceneItem SceneItem { get; }

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


        
        protected override bool OnSelectedEffectChanged(string name, out Effect effect)
        {
            if(!base.OnSelectedEffectChanged(name, out effect))
            {
                return false;
            }

            SceneItem.Effect = effect;
            UpdateState();
            return true;
        }

        protected override bool OnSelectedThemeChanged(string name, out Theme theme)
        {
            if(!base.OnSelectedThemeChanged(name, out theme))
            {
                return false;
            }

            SceneItem.Theme = theme;
            SceneItem.Color = null;
            SceneItem.MultiZoneColors = null;
            UpdateState();
            return true;
        }

        protected override void UpdateState()
        {
            InUpdate = true;

            // If we can't find a valid device, disable everything
            var enable = Device is not null;
            PowerStatePanel.Enabled = enable;
            SolidColorPanel.Enabled = enable;
            MultiZoneColorPanel.Enabled = enable;
            ThemeButtonPanel.Enabled = enable;
            ThemeButtonPanel.Enabled = enable;
            EffectButtonPanel.Enabled = enable;
            EffectButtonPanel.Enabled = enable;

            // TODO losing the color zone count
            if(enable)
            {
                PowerStatePanel.Power = SceneItem.Power;
                SolidColorPanel.Color = SceneItem.Color ?? Device.Color;
                MultiZoneColorPanel.Colors = SceneItem.MultiZoneColors.IsNullOrEmpty() ? Device.MultiZoneColors : SceneItem.MultiZoneColors;
                ThemeButtonPanel.DeviceFamily = SceneItem.Family;
                ThemeButtonPanel.SelectedTheme = SceneItem.Theme;
                EffectButtonPanel.DeviceFamily = SceneItem.Family;
                EffectButtonPanel.SelectedEffect = SceneItem.Effect;
            }

            InUpdate = false;
        }

        #region Event Handlers

        protected override void OnMultiZoneColorsChanged(object sender, ColorsChangedEventArgs e)
        {
            SceneItem.MultiZoneColors = e.Property;
            SceneItem.Color = null;
            SceneItem.Theme = null;
            base.OnMultiZoneColorsChanged(sender, e);
        }

        protected override void OnPowerStateChanged(object sender, PowerStateChangedEventArgs e)
        {
            SceneItem.Power = e.Property;
            base.OnPowerStateChanged(sender, e);
        }

        protected override void OnSolidColorChanged(object sender, ColorChangedEventArgs e)
        {
            SceneItem.Color = e.Property;
            SceneItem.MultiZoneColors = null;
            SceneItem.Theme = null;
            base.OnSolidColorChanged(sender, e);
        }

        void OnDevicePropertiesChanged(object sender, DeviceEventArgs e)
        {
            UpdateState();
        }

        #endregion
    }
}
