using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DerekWare.Collections;
using DerekWare.HomeAutomation.Common;
using DerekWare.HomeAutomation.Common.Effects;

namespace DerekWare.Iris
{
    public partial class EffectButtonPanel : UserControl
    {
        string _DeviceFamily;
        IReadOnlyEffectProperties _SelectedObject;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true)]
        public event EventHandler<PropertyChangedEventArgs<IReadOnlyEffectProperties>> SelectedObjectChanged;

        public EffectButtonPanel()
        {
            InitializeComponent();

            if(Extensions.IsDesignMode())
            {
                return;
            }

            EffectFactory.Instance.ForEach(Add);
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
        public IReadOnlyEffectProperties SelectedObject
        {
            get => _SelectedObject;
            set
            {
                _SelectedObject = value;
                UpdateState();
            }
        }

        Button Add(IReadOnlyEffectProperties effect)
        {
            var button = new Button { Text = effect.Name, Tag = effect, Dock = DockStyle.Fill };
            button.Click += OnClick;
            LayoutPanel.Controls.Add(button);

            if(!effect.Description.IsNullOrEmpty())
            {
                new ToolTip().SetToolTip(button, effect.Description);
            }

            return button;
        }

        void UpdateState()
        {
            foreach(var button in LayoutPanel.Controls.OfType<Button>())
            {
                var effect = (IReadOnlyEffectProperties)button.Tag;
                button.Enabled = effect.IsCompatible(DeviceFamily);

                if(effect.Matches(SelectedObject))
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
            SelectedObjectChanged?.Invoke(this, new SelectedEffectChangedEventArgs { Property = (IReadOnlyEffectProperties)((Button)sender).Tag });
        }

        #endregion
    }
}
