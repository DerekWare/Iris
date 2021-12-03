using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Cyotek.Windows.Forms;
using DerekWare.Collections;
using Color = DerekWare.HomeAutomation.Common.Colors.Color;

namespace DerekWare.Iris
{
    public class ColorBand : UserControl
    {
        static readonly ColorPickerDialog ColorPickerDialog = new();

        Color[] _Colors = { new() };

        public event EventHandler<ColorsChangedEventArgs> ColorsChanged;

        public new bool DesignMode => Extensions.IsDesignMode();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public IReadOnlyCollection<Color> Colors
        {
            get => _Colors;
            set
            {
                var colors = value.IsNullOrEmpty() ? new Color[] { new() } : value.Select(i => i is not null ? i.Clone() : new Color()).ToArray();

                if(colors.SequenceEqual(_Colors))
                {
                    return;
                }

                _Colors = colors;
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            var width = (float)Size.Width / _Colors.Length;
            var index = (int)(e.X / width);

            if(!SelectColor(out var hsv))
            {
                return;
            }

            _Colors[index] = hsv;
            Invalidate();
            ColorsChanged?.Invoke(this, new ColorsChangedEventArgs { Property = _Colors });
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var width = (float)Size.Width / _Colors.Length;
            var height = (float)Size.Height;
            var x = 0.0f;

            foreach(var color in _Colors.Select(color => color?.ToRgb() ?? System.Drawing.Color.Black))
            {
                using(var brush = new SolidBrush(color))
                {
                    e.Graphics.FillRectangle(brush, x, 0, Size.Width - x, height);
                }

                x += width;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            Invalidate();
            base.OnResize(e);
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
