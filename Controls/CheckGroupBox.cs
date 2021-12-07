using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using DerekWare.Collections;

namespace DerekWare.Iris
{
    public class CheckGroupBox : GroupBox
    {
        readonly CheckBox CheckBox;

        bool _ApplyChildState;

        //
        // Summary:
        //     Occurs when the value of the System.Windows.Forms.CheckBox.Checked property changes.
        [Browsable(true)]
        public event EventHandler CheckedChanged;

        public CheckGroupBox()
        {
            CheckBox = new CheckBox
            {
                AutoSize = true,
                Checked = false,
                ForeColor = new VisualStyleRenderer(VisualStyleElement.Button.GroupBox.Normal).GetColor(ColorProperty.TextColor),
                Location = new Point(10, 0),
                Parent = this,
                Visible = false
            };

            CheckBox.CheckedChanged += OnCheckedChanged;
            ControlAdded += OnControlAdded;
        }

        [Browsable(true)]
        public bool ApplyChildState
        {
            get => _ApplyChildState;
            set
            {
                if(_ApplyChildState == value)
                {
                    return;
                }

                _ApplyChildState = value;

                if(_ApplyChildState)
                {
                    Controls.OfType<Control>().WhereNotEquals(i => i, CheckBox).ForEach(i => i.Enabled = Checked);
                }
            }
        }

        [Browsable(true)]
        public bool Checked { get => CheckBox.Checked; set => CheckBox.Checked = ShowCheckBox && value; }

        [Browsable(true)]
        public bool ShowCheckBox
        {
            get => CheckBox.Visible;
            set
            {
                if(ShowCheckBox == value)
                {
                    return;
                }

                CheckBox.Visible = value;
                CheckBox.Checked = value;
                base.Text = value ? null : Text;
            }
        }

        //
        // Summary:
        //     Gets or sets the text associated with this control.
        //
        // Returns:
        //     The text associated with this control.
        [Browsable(true), Localizable(true)]
        public override string Text
        {
            get => CheckBox.Text;
            set
            {
                CheckBox.Text = value;
                base.Text = ShowCheckBox ? null : Text;
            }
        }

        #region Event Handlers

        void OnCheckedChanged(object sender, EventArgs e)
        {
            if(ApplyChildState)
            {
                Controls.OfType<Control>().WhereNotEquals(i => i, CheckBox).ForEach(i => i.Enabled = Checked);
            }

            CheckedChanged?.Invoke(this, e);
        }

        void OnControlAdded(object sender, ControlEventArgs e)
        {
            if(!ApplyChildState)
            {
                return;
            }

            e.Control.Enabled = Checked;
        }

        #endregion
    }
}
