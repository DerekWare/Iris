using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common;

namespace DerekWare.Iris.Controls
{
    public class DevicePropertyTabControl<T> : TabControl
        where T : IName, IFamily, IDescription
    {
        string _DeviceFamily;
        IReadOnlyCollection<T> _Factory;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true)]
        public event EventHandler<PropertyChangedEventArgs<T>> SelectedObjectChanged;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public string DeviceFamily
        {
            get => _DeviceFamily;
            set
            {
                _DeviceFamily = value;
                UpdatePages();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public IReadOnlyCollection<T> Factory
        {
            get => _Factory;
            set
            {
                _Factory = value;
                UpdatePages();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public T SelectedObject
        {
            get;
            set;

            // TODO
        }

        public void Add(T obj)
        {
            var page = new DevicePropertyTabPage<T>(obj);
            page.ApplyClicked += Page_ApplyClicked;
            TabPages.Add(page);
        }

        public void AddRange(IEnumerable<T> items)
        {
            items.ForEach(Add);
        }

        void UpdatePages()
        {
            TabPages.Clear();
            AddRange(_Factory?.Where(i => i.IsCompatible(_DeviceFamily)));
        }

        #region Event Handlers

        void Page_ApplyClicked(object sender, EventArgs e)
        {
            SelectedObject = ((DevicePropertyTabPage<T>)sender).SelectedObject;
            SelectedObjectChanged?.Invoke(this, new PropertyChangedEventArgs<T> { Property = SelectedObject });
        }

        #endregion
    }
}
