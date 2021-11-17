
using DerekWare.HomeAutomation.Common.Colors;
using DerekWare.HomeAutomation.Lifx.Lan.Colors;

namespace DerekWare.Iris
{
    partial class SolidColorPanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if(disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.HueLabel = new System.Windows.Forms.Label();
            this.KelvinUpDown = new System.Windows.Forms.NumericUpDown();
            this.KelvinLabel = new System.Windows.Forms.Label();
            this.BrightnessUpDown = new System.Windows.Forms.NumericUpDown();
            this.BrightnessLabel = new System.Windows.Forms.Label();
            this.SaturationUpDown = new System.Windows.Forms.NumericUpDown();
            this.SaturationLabel = new System.Windows.Forms.Label();
            this.HueUpDown = new System.Windows.Forms.NumericUpDown();
            this.StandardColorsComboBox = new System.Windows.Forms.ComboBox();
            this.ColorBand = new ColorBand();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.KelvinUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BrightnessUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SaturationUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.HueUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Controls.Add(this.ColorBand, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.HueLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.KelvinUpDown, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.KelvinLabel, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.BrightnessUpDown, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.BrightnessLabel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.SaturationUpDown, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.SaturationLabel, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.HueUpDown, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.StandardColorsComboBox, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1118, 128);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // HueLabel
            // 
            this.HueLabel.Location = new System.Drawing.Point(8, 52);
            this.HueLabel.Margin = new System.Windows.Forms.Padding(8);
            this.HueLabel.Name = "HueLabel";
            this.HueLabel.Size = new System.Drawing.Size(92, 26);
            this.HueLabel.TabIndex = 1;
            this.HueLabel.Text = "&Hue";
            this.HueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // KelvinUpDown
            // 
            this.KelvinUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.KelvinUpDown.DecimalPlaces = 4;
            this.KelvinUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.KelvinUpDown.Location = new System.Drawing.Point(584, 94);
            this.KelvinUpDown.Margin = new System.Windows.Forms.Padding(8);
            this.KelvinUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.KelvinUpDown.Name = "KelvinUpDown";
            this.KelvinUpDown.Size = new System.Drawing.Size(344, 26);
            this.KelvinUpDown.TabIndex = 8;
            this.KelvinUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.KelvinUpDown.ValueChanged += new System.EventHandler(this.KelvinUpDown_ValueChanged);
            // 
            // KelvinLabel
            // 
            this.KelvinLabel.Location = new System.Drawing.Point(476, 94);
            this.KelvinLabel.Margin = new System.Windows.Forms.Padding(8);
            this.KelvinLabel.Name = "KelvinLabel";
            this.KelvinLabel.Size = new System.Drawing.Size(92, 26);
            this.KelvinLabel.TabIndex = 7;
            this.KelvinLabel.Text = "&Kelvin";
            this.KelvinLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // BrightnessUpDown
            // 
            this.BrightnessUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BrightnessUpDown.DecimalPlaces = 4;
            this.BrightnessUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.BrightnessUpDown.Location = new System.Drawing.Point(116, 94);
            this.BrightnessUpDown.Margin = new System.Windows.Forms.Padding(8);
            this.BrightnessUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.BrightnessUpDown.Name = "BrightnessUpDown";
            this.BrightnessUpDown.Size = new System.Drawing.Size(344, 26);
            this.BrightnessUpDown.TabIndex = 6;
            this.BrightnessUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.BrightnessUpDown.ValueChanged += new System.EventHandler(this.BrightnessUpDown_ValueChanged);
            // 
            // BrightnessLabel
            // 
            this.BrightnessLabel.Location = new System.Drawing.Point(8, 94);
            this.BrightnessLabel.Margin = new System.Windows.Forms.Padding(8);
            this.BrightnessLabel.Name = "BrightnessLabel";
            this.BrightnessLabel.Size = new System.Drawing.Size(92, 26);
            this.BrightnessLabel.TabIndex = 5;
            this.BrightnessLabel.Text = "&Brightness";
            this.BrightnessLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SaturationUpDown
            // 
            this.SaturationUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SaturationUpDown.DecimalPlaces = 4;
            this.SaturationUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.SaturationUpDown.Location = new System.Drawing.Point(584, 52);
            this.SaturationUpDown.Margin = new System.Windows.Forms.Padding(8);
            this.SaturationUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.SaturationUpDown.Name = "SaturationUpDown";
            this.SaturationUpDown.Size = new System.Drawing.Size(344, 26);
            this.SaturationUpDown.TabIndex = 4;
            this.SaturationUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.SaturationUpDown.ValueChanged += new System.EventHandler(this.SaturationUpDown_ValueChanged);
            // 
            // SaturationLabel
            // 
            this.SaturationLabel.Location = new System.Drawing.Point(476, 52);
            this.SaturationLabel.Margin = new System.Windows.Forms.Padding(8);
            this.SaturationLabel.Name = "SaturationLabel";
            this.SaturationLabel.Size = new System.Drawing.Size(92, 26);
            this.SaturationLabel.TabIndex = 3;
            this.SaturationLabel.Text = "&Saturation";
            this.SaturationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // HueUpDown
            // 
            this.HueUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.HueUpDown.DecimalPlaces = 4;
            this.HueUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.HueUpDown.Location = new System.Drawing.Point(116, 52);
            this.HueUpDown.Margin = new System.Windows.Forms.Padding(8);
            this.HueUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.HueUpDown.Name = "HueUpDown";
            this.HueUpDown.Size = new System.Drawing.Size(344, 26);
            this.HueUpDown.TabIndex = 2;
            this.HueUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.HueUpDown.ValueChanged += new System.EventHandler(this.HueUpDown_ValueChanged);
            // 
            // StandardColorsComboBox
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.StandardColorsComboBox, 4);
            this.StandardColorsComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.StandardColorsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.StandardColorsComboBox.FormattingEnabled = true;
            this.StandardColorsComboBox.Location = new System.Drawing.Point(8, 8);
            this.StandardColorsComboBox.Margin = new System.Windows.Forms.Padding(8);
            this.StandardColorsComboBox.Name = "StandardColorsComboBox";
            this.StandardColorsComboBox.Size = new System.Drawing.Size(920, 28);
            this.StandardColorsComboBox.TabIndex = 0;
            this.StandardColorsComboBox.SelectedIndexChanged += new System.EventHandler(this.StandardColorsComboBox_SelectedIndexChanged);
            // 
            // ColorBand
            // 
            this.ColorBand.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ColorBand.Colors = new Color[0];
            this.ColorBand.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ColorBand.Location = new System.Drawing.Point(944, 8);
            this.ColorBand.Margin = new System.Windows.Forms.Padding(8);
            this.ColorBand.Name = "ColorBand";
            this.tableLayoutPanel1.SetRowSpan(this.ColorBand, 3);
            this.ColorBand.Size = new System.Drawing.Size(166, 112);
            this.ColorBand.TabIndex = 9;
            this.ColorBand.ColorsChanged += new System.EventHandler<ColorBandColorsChangedEventArgs>(this.ColorBand_ColorsChanged);
            // 
            // SolidColorPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SolidColorPanel";
            this.Size = new System.Drawing.Size(1118, 128);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.KelvinUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BrightnessUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SaturationUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.HueUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label HueLabel;
        private System.Windows.Forms.NumericUpDown KelvinUpDown;
        private System.Windows.Forms.Label KelvinLabel;
        private System.Windows.Forms.NumericUpDown BrightnessUpDown;
        private System.Windows.Forms.Label BrightnessLabel;
        private System.Windows.Forms.NumericUpDown SaturationUpDown;
        private System.Windows.Forms.Label SaturationLabel;
        private System.Windows.Forms.NumericUpDown HueUpDown;
        private ColorBand ColorBand;
        private System.Windows.Forms.ComboBox StandardColorsComboBox;
    }
}
