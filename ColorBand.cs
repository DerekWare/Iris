using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Cyotek.Windows.Forms;
using Color = DerekWare.HomeAutomation.Common.Colors.Color;

namespace DerekWare.Iris
{
    public class ColorBand : UserControl
    {
        static readonly ColorPickerDialog ColorPickerDialog = new();

        Color[] _Colors = new Color[0];

        public event EventHandler<ColorBandColorsChangedEventArgs> ColorsChanged;

        public Color[] Colors
        {
            get => _Colors;
            set
            {
                _Colors = value ?? new Color[1];

                for(var i = 0; i < _Colors.Length; ++i)
                {
                    if(_Colors[i] is null)
                    {
                        _Colors[i] = new Color();
                    }
                }

                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            var width = (float)Size.Width / Colors.Length;
            var index = (int)(e.X / width);

            if(!SelectColor(out var hsv))
            {
                return;
            }

            Colors[index] = hsv;
            Invalidate();
            ColorsChanged?.Invoke(this, new ColorBandColorsChangedEventArgs { Colors = Colors });
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var width = (float)Size.Width / Colors.Length;
            var height = (float)Size.Height;
            var x = 0.0f;

            foreach(var color in Colors.Select(color => color?.ToRgb() ?? System.Drawing.Color.Black))
            {
                using(var brush = new SolidBrush(color))
                {
                    e.Graphics.FillRectangle(brush, x, 0, Size.Width - x, height);
                }

                x += width;
            }
        }

        protected bool SelectColor(out Color hsv)
        {
            // Capture the current RGB color in the color picker to prevent precision loss
            // during HSV => RGB => HSV conversion.
            var prevColor = ColorPickerDialog.Color;

            if((DialogResult.OK != ColorPickerDialog.ShowDialog()) || (prevColor == ColorPickerDialog.Color))
            {
                hsv = null;
                return false;
            }

            hsv = new Color(ColorPickerDialog.Color);
            return true;
        }
    }
}
