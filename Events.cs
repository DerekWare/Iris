using System;
using System.Collections.Generic;
using DerekWare.HomeAutomation.Common;
using DerekWare.HomeAutomation.Common.Effects;
using DerekWare.HomeAutomation.Common.Themes;

namespace DerekWare.Iris
{
    public class BrightnessChangedEventArgs : PropertyChangedEventArgs<double>
    {
    }

    public class ColorChangedEventArgs : PropertyChangedEventArgs<Color>
    {
    }

    public class ColorsChangedEventArgs : PropertyChangedEventArgs<IReadOnlyCollection<Color>>
    {
    }

    public class PowerStateChangedEventArgs : PropertyChangedEventArgs<PowerState>
    {
    }

    public class PropertyChangedEventArgs<T> : EventArgs
    {
        public T Property { get; set; }
    }

    public class SelectedEffectChangedEventArgs : PropertyChangedEventArgs<IReadOnlyEffectProperties>
    {
    }

    public class SelectedThemeChangedEventArgs : PropertyChangedEventArgs<IReadOnlyThemeProperties>
    {
    }
}
