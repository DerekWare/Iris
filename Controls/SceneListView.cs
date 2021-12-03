using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common.Effects;
using DerekWare.HomeAutomation.Common.Scenes;
using DerekWare.HomeAutomation.Common.Themes;

namespace DerekWare.Iris
{
    public class SceneListView : ListView
    {
        readonly EffectMenuStrip EffectMenuStrip = new() { Text = "Change Effect" };
        readonly ThemeMenuStrip ThemeMenuStrip = new() { Text = "Change Theme" };

        Scene _Scene;

        public SceneListView()
        {
            if(DesignMode)
            {
                return;
            }

            FullRowSelect = true;
            HeaderStyle = ColumnHeaderStyle.Nonclickable;
            HideSelection = false;
            LabelWrap = false;
            View = View.Details;

            Columns.Add(null, "Device", 200);
            Columns.Add(null, "Power", 200);
            Columns.Add(null, "Colors", 200);
            Columns.Add(null, "Theme", 200);
            Columns.Add(null, "Effect", 200);

            ContextMenuStrip = new ContextMenuStrip();
            ContextMenuStrip.Items.Add(ThemeMenuStrip);
            ContextMenuStrip.Items.Add(EffectMenuStrip);

            ThemeMenuStrip.Click += ThemeMenuStrip_Click;
            EffectMenuStrip.Click += EffectMenuStrip_Click;
        }

        [Browsable(false)]
        public Scene Scene
        {
            get => _Scene;
            set
            {
                if(_Scene is not null)
                {
                    _Scene.Items.CollectionChanged -= OnCollectionChanged;
                }

                Items.Clear();

                _Scene = value;

                if(_Scene is null)
                {
                    return;
                }

                foreach(var i in Scene.Items)
                {
                    Items.Add(new ListViewItem(i));
                }

                Scene.Items.CollectionChanged += OnCollectionChanged;
            }
        }

        protected new bool DesignMode => Extensions.IsDesignMode();

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            var selected = SelectedIndices.Count > 0;
            ContextMenuStrip.Items.ForEach<ToolStripMenuItem>(i => i.Enabled = selected);
            base.OnSelectedIndexChanged(e);
        }

        #region Event Handlers

        void EffectMenuStrip_Click(object sender, AutoMenuStrip<IReadOnlyEffectProperties>.ClickEventArgs e)
        {
            // Create the effect
            var effect = EffectFactory.Instance.CreateInstance(e.Object.Name);

            if(DialogResult.OK != PropertyEditor.Show(this, effect))
            {
                return;
            }

            // Cache property edits
            EffectFactory.Instance.Add(effect);

            // Apply the effect
            foreach(ListViewItem item in SelectedItems)
            {
                item.SceneItem.Effect = effect;
            }
        }

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach(var i in e.OldItems.SafeEmpty())
            {
                Items.RemoveWhere<ListViewItem>(j => Equals(i, j.SceneItem));
            }

            foreach(var i in e.NewItems.SafeEmpty())
            {
                Items.Add(new ListViewItem((SceneItem)i));
            }
        }

        void ThemeMenuStrip_Click(object sender, AutoMenuStrip<IReadOnlyThemeProperties>.ClickEventArgs e)
        {
            // Create the theme
            var theme = ThemeFactory.Instance.CreateInstance(e.Object.Name);

            if(DialogResult.OK != PropertyEditor.Show(this, theme))
            {
                return;
            }

            // Cache property edits
            ThemeFactory.Instance.Add(theme);

            // Apply the theme
            foreach(ListViewItem item in SelectedItems)
            {
                item.SceneItem.Theme = theme;
            }
        }

        #endregion

        public class ListViewItem : System.Windows.Forms.ListViewItem
        {
            public ListViewItem(SceneItem sceneItem)
            {
                SceneItem = sceneItem;
                Text = SceneItem.Name;

                SubItems.Add(SceneItem.Power.ToString());
                SubItems.Add(SceneItem.MultiZoneColors?.Count.ToString());
                SubItems.Add(SceneItem.Theme?.Name);
                SubItems.Add(SceneItem.Effect?.Name);
            }

            public SceneItem SceneItem { get; }
        }
    }
}
