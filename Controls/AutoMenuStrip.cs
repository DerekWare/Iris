using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DerekWare.HomeAutomation.Common;
using DerekWare.HomeAutomation.Common.Effects;
using DerekWare.HomeAutomation.Common.Themes;

namespace DerekWare.Iris
{
    public class AutoMenuStrip<T> : ToolStripMenuItem
        where T : IName
    {
        public new event EventHandler<ClickEventArgs> Click;

        protected new bool DesignMode => Extensions.IsDesignMode();

        public void Populate(IEnumerable<T> items)
        {
            DropDownItems.Clear();

            foreach(var i in items)
            {
                var menuItem = new ToolStripMenuItem(i.Name) { Tag = i };
                menuItem.Click += (sender, args) => Click?.Invoke(this, new ClickEventArgs { Object = i });
                DropDownItems.Add(menuItem);
            }
        }

        public class ClickEventArgs : EventArgs
        {
            public T Object { get; set; }
        }
    }

    public class EffectMenuStrip : AutoMenuStrip<IReadOnlyEffectProperties>
    {
        public EffectMenuStrip()
        {
            if(DesignMode)
            {
                return;
            }

            Populate(EffectFactory.Instance);
        }
    }

    public class ThemeMenuStrip : AutoMenuStrip<IReadOnlyThemeProperties>
    {
        public ThemeMenuStrip()
        {
            if(DesignMode)
            {
                return;
            }

            Populate(ThemeFactory.Instance);
        }
    }
}
