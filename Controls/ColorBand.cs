using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Cyotek.Windows.Forms;
using DerekWare.Collections;
using Color = DerekWare.HomeAutomation.Common.Color;

namespace DerekWare.Iris
{
    public class ColorBand : UserControl
    {
        static readonly ColorPickerDialog ColorPickerDialog = new();

        Color[] _Colors = { new() };

        public event EventHandler<ColorsChangedEventArgs> ColorsChanged;

        public new bool DesignMode => base.DesignMode || Extensions.IsDesignMode();

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

        protected override void OnEnabledChanged(EventArgs e)
        {
            Invalidate();
            base.OnEnabledChanged(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if(!Enabled)
            {
                return;
            }

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

            var controlWidth = Size.Width;
            var controlHeight = Size.Height;
            var segmentWidth = controlWidth / (float)_Colors.Length;
            var x = 0.0f;

            if(!Enabled)
            {
                e.Graphics.FillRectangle(SystemBrushes.Control, 0, 0, controlWidth, controlHeight);
                return;
            }

            foreach(var color in _Colors.Select(color => color?.ToRgb() ?? System.Drawing.Color.Black))
            {
                using(var brush = new SolidBrush(color))
                {
                    e.Graphics.FillRectangle(brush, x, 0, controlWidth - x, controlHeight);
                }

                x += segmentWidth;
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
