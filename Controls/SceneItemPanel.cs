using System.Windows.Forms;
using DerekWare.HomeAutomation.Common.Effects;
using DerekWare.HomeAutomation.Common.Scenes;
using DerekWare.HomeAutomation.Common.Themes;

namespace DerekWare.Iris
{
    public partial class SceneItemPanel : UserControl
    {
        SceneItem _SceneItem;
        bool InUpdate;

        public SceneItemPanel()
        {
            InitializeComponent();
        }

        public SceneItem SceneItem
        {
            get => _SceneItem;
            set
            {
                _SceneItem = value;
                UpdateState();
            }
        }

        void UpdateState()
        {
            InUpdate = true;

            TabPage.Text = SceneItem.Device?.Name;
            PowerStatePanel.Power = SceneItem.Power;
            SolidColorPanel.Color = SceneItem.Color;
            MultiZoneColorPanel.Colors = SceneItem.MultiZoneColors;
            ThemePanel.DeviceFamily = SceneItem.Family;
            ThemePanel.SelectedTheme = SceneItem.Theme;
            EffectPanel.DeviceFamily = SceneItem.Family;
            EffectPanel.SelectedEffect = SceneItem.Effect;

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
                SceneItem.Effect = effect;
            }
        }

        void MultiZoneColorPanel_ColorsChanged(object sender, ColorsChangedEventArgs e)
        {
            if(InUpdate)
            {
                return;
            }

            InUpdate = true;
            SceneItem.Effect = null;
            SceneItem.MultiZoneColors = e.Property;
            InUpdate = false;
        }

        void PowerStatePanel_PowerStateChanged(object sender, PowerStateChangedEventArgs e)
        {
            if(InUpdate)
            {
                return;
            }

            InUpdate = true;
            SceneItem.Power = e.Property;
            InUpdate = false;
        }

        void SolidColorPanel_ColorChanged(object sender, ColorChangedEventArgs e)
        {
            if(InUpdate)
            {
                return;
            }

            InUpdate = true;
            SceneItem.Effect = null;
            SceneItem.Color = e.Property;
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
                SceneItem.Theme = theme;
            }
        }

        #endregion
    }
}
