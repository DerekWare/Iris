using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true)]
        public event EventHandler<ColorChangedEventArgs> ColorChanged;

        public SolidColorPanel()
        {
            InitializeComponent();

            InUpdate = true;

            StandardColorsComboBox.Items.AddRange(Colors.All.ToArray());

            InUpdate = false;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
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

                if(!StandardColorsComboBox.DroppedDown)
                {
                    StandardColorsComboBox.SelectedItem = StandardColorsComboBox.Items.Contains(_Color) ? _Color : null;
                }

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

            ColorChanged?.Invoke(this,
                                 new ColorChangedEventArgs
                                 {
                                     Property = new Color(Color.Hue, Color.Saturation, decimal.ToDouble(BrightnessUpDown.Value), Color.Kelvin)
                                 });
        }

        void ColorBand_ColorsChanged(object sender, ColorsChangedEventArgs e)
        {
            if(InUpdate)
            {
                return;
            }

            ColorChanged?.Invoke(this, new ColorChangedEventArgs { Property = e.Property.First() });
        }

        void HueUpDown_ValueChanged(object sender, EventArgs e)
        {
            if(InUpdate)
            {
                return;
            }

            ColorChanged?.Invoke(this,
                                 new ColorChangedEventArgs
                                 {
                                     Property = new Color(decimal.ToDouble(HueUpDown.Value), Color.Saturation, Color.Brightness, Color.Kelvin)
                                 });
        }

        void KelvinUpDown_ValueChanged(object sender, EventArgs e)
        {
            if(InUpdate)
            {
                return;
            }

            ColorChanged?.Invoke(this,
                                 new ColorChangedEventArgs
                                 {
                                     Property = new Color(Color.Hue, Color.Saturation, Color.Brightness, decimal.ToDouble(KelvinUpDown.Value))
                                 });
        }

        void SaturationUpDown_ValueChanged(object sender, EventArgs e)
        {
            if(InUpdate)
            {
                return;
            }

            ColorChanged?.Invoke(this,
                                 new ColorChangedEventArgs
                                 {
                                     Property = new Color(Color.Hue, decimal.ToDouble(SaturationUpDown.Value), Color.Brightness, Color.Kelvin)
                                 });
        }

        void StandardColorsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(InUpdate || (StandardColorsComboBox.SelectedIndex < 0))
            {
                return;
            }

            ColorChanged?.Invoke(this, new ColorChangedEventArgs { Property = (Color)StandardColorsComboBox.SelectedItem });
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
