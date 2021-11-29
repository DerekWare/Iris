using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common.Scenes;

namespace DerekWare.Iris
{
    public class SceneListView : ListView
    {
        Scene _Scene;

        public SceneListView()
        {
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

        #region Event Handlers

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
