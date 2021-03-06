using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common;
using DerekWare.HomeAutomation.Common.Themes;

namespace DerekWare.Iris
{
    public partial class ThemeButtonPanel : UserControl
    {
        string _DeviceFamily;
        IReadOnlyThemeProperties _SelectedObject;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true)]
        public event EventHandler<PropertyChangedEventArgs<IReadOnlyThemeProperties>> SelectedObjectChanged;

        public ThemeButtonPanel()
        {
            InitializeComponent();

            if(Extensions.IsDesignMode())
            {
                return;
            }

            ThemeFactory.Instance.ForEach(Add);
            UpdateState();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public string DeviceFamily
        {
            get => _DeviceFamily;
            set
            {
                _DeviceFamily = value;
                UpdateState();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public IReadOnlyThemeProperties SelectedObject
        {
            get => _SelectedObject;
            set
            {
                _SelectedObject = value;
                UpdateState();
            }
        }

        Button Add(IReadOnlyThemeProperties theme)
        {
            var button = new Button { Text = theme.Name, Tag = theme, Dock = DockStyle.Fill };
            button.Click += OnClick;
            LayoutPanel.Controls.Add(button);

            if(!theme.Description.IsNullOrEmpty())
            {
                new ToolTip().SetToolTip(button, theme.Description);
            }

            return button;
        }

        void UpdateState()
        {
            foreach(var button in LayoutPanel.Controls.OfType<Button>())
            {
                var theme = (IReadOnlyThemeProperties)button.Tag;
                button.Enabled = theme.IsCompatible(DeviceFamily);

                if(theme.Matches(SelectedObject))
                {
                    button.BackColor = SystemColors.Highlight;
                    button.ForeColor = SystemColors.HighlightText;
                }
                else
                {
                    button.BackColor = default;
                    button.ForeColor = default;
                    button.UseVisualStyleBackColor = true;
                }
            }
        }

        #region Event Handlers

        void OnClick(object sender, EventArgs e)
        {
            // Don't update SelectedObject until the caller has a chance to display the property editor
            SelectedObjectChanged?.Invoke(this, new SelectedThemeChangedEventArgs { Property = (IReadOnlyThemeProperties)((Button)sender).Tag });
        }

        #endregion
    }
}
