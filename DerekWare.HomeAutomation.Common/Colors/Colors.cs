using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DerekWare.HomeAutomation.Common
{
    public static partial class Colors
    {
        public static readonly Color Black = new(0, 0, 0, 0);
        public static readonly Color Blue = new(240 / 360.0, 1, 1, 1);
        public static readonly Color Cyan = new(180 / 360.0, 1, 1, 1);
        public static readonly Color Green = new(120 / 360.0, 1, 1, 1);
        public static readonly Color LightBlue = new(210 / 360.0, 1, 1, 1);
        public static readonly Color Magenta = new(300 / 360.0, 1, 1, 1);
        public static readonly Color Orange = new(30 / 360.0, 1, 1, 1);
        public static readonly Color Red = new(0 / 360.0, 1, 1, 1);
        public static readonly Color Violet = new(270 / 360.0, 1, 1, 1);
        public static readonly Color WarmWhite = new(0, 0, 1, 0);
        public static readonly Color White = new(0, 0, 1, 1);
        public static readonly Color Yellow = new(60 / 360.0, 1, 1, 1);

        public static IEnumerable<Color> All =>
            from field in typeof(Colors).GetFields(BindingFlags.Public | BindingFlags.Static)
            let value = (Color)field.GetValue(null)
            select value;

        public static Color GetColorByName(string name)
        {
            return (from field in typeof(Colors).GetFields(BindingFlags.Public | BindingFlags.Static)
                    where string.Equals(field.Name, name, StringComparison.OrdinalIgnoreCase)
                    select (Color)field.GetValue(null)).FirstOrDefault();
        }

        public static string GetColorName(this Color color)
        {
            return (from field in typeof(Colors).GetFields(BindingFlags.Public | BindingFlags.Static)
                    let value = (Color)field.GetValue(null)
                    where Equals(color, value)
                    select field.Name).FirstOrDefault();
        }
    }
}
