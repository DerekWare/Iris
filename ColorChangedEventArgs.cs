using System;
using DerekWare.HomeAutomation.Common.Colors;

namespace DerekWare.Iris
{
    public class ColorChangedEventArgs : EventArgs
    {
        public Color Color { get; set; }
    }
}
