using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common.Colors;

namespace DerekWare.Iris
{
    public partial class SolidColorPanel : UserControl
    {
        Color _Color;
        bool InUpdate;

        public event EventHandler<ColorChangedEventArgs> ColorChanged;

        public SolidColorPanel()
        {
            InitializeComponent();

            InUpdate = true;

            StandardColorsComboBox.Items.AddRange(StandardColors.All.ToArray());

            InUpdate = false;
        }

        public Color Color
        {
            get => _Color;
            set
            {
                InUpdate = true;

                _Color = value ?? new Color();
                HueUpDown.Value = new decimal(_Color.Hue);
                SaturationUpDown.Value = new decimal(_Color.Saturation);
                BrightnessUpDown.Value = new decimal(_Color.Brightness);
                KelvinUpDown.Value = new decimal(_Color.Kelvin);
                ColorBand.Colors = new[] { _Color };
                StandardColorsComboBox.SelectedItem = StandardColorsComboBox.Items.Contains(_Color) ? _Color : null;

                InUpdate = false;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            FindControls<NumericUpDown>(this).ForEach(i => i.Controls[0].Enabled = false);
            base.OnLoad(e);
        }

        #region Event Handlers

        void BrightnessUpDown_ValueChanged(object sender, EventArgs e)
        {
            if(InUpdate)
            {
                return;
            }

            Color = new Color(Color.Hue, Color.Saturation, decimal.ToDouble(BrightnessUpDown.Value), Color.Kelvin);
            ColorChanged?.Invoke(this, new ColorChangedEventArgs { Color = _Color });
        }

        void ColorBand_ColorsChanged(object sender, ColorBandColorsChangedEventArgs e)
        {
            Color = e.Colors[0];
            ColorChanged?.Invoke(this, new ColorChangedEventArgs { Color = _Color });
        }

        void HueUpDown_ValueChanged(object sender, EventArgs e)
        {
            if(InUpdate)
            {
                return;
            }

            Color = new Color(decimal.ToDouble(HueUpDown.Value), Color.Saturation, Color.Brightness, Color.Kelvin);
            ColorChanged?.Invoke(this, new ColorChangedEventArgs { Color = _Color });
        }

        void KelvinUpDown_ValueChanged(object sender, EventArgs e)
        {
            if(InUpdate)
            {
                return;
            }

            Color = new Color(Color.Hue, Color.Saturation, Color.Brightness, decimal.ToDouble(KelvinUpDown.Value));
            ColorChanged?.Invoke(this, new ColorChangedEventArgs { Color = _Color });
        }

        void SaturationUpDown_ValueChanged(object sender, EventArgs e)
        {
            if(InUpdate)
            {
                return;
            }

            Color = new Color(Color.Hue, decimal.ToDouble(SaturationUpDown.Value), Color.Brightness, Color.Kelvin);
            ColorChanged?.Invoke(this, new ColorChangedEventArgs { Color = _Color });
        }

        void StandardColorsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(InUpdate || (StandardColorsComboBox.SelectedIndex < 0))
            {
                return;
            }

            Color = (Color)StandardColorsComboBox.SelectedItem;
            ColorChanged?.Invoke(this, new ColorChangedEventArgs { Color = _Color });
        }

        #endregion

        static IEnumerable<T> FindControls<T>(Control control)
            where T : Control
        {
            foreach(var i in control.Controls.OfType<T>())
            {
                yield return i;
            }

            foreach(Control i in control.Controls)
            {
                foreach(var j in FindControls<T>(i))
                {
                    yield return j;
                }
            }
        }
    }
}
