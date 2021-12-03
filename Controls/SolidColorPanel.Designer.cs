
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
            this.SolidColorGroupBox = new System.Windows.Forms.GroupBox();
            this.TableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.ColorBand = new DerekWare.Iris.ColorBand();
            this.HueLabel = new System.Windows.Forms.Label();
            this.KelvinUpDown = new System.Windows.Forms.NumericUpDown();
            this.KelvinLabel = new System.Windows.Forms.Label();
            this.BrightnessUpDown = new System.Windows.Forms.NumericUpDown();
            this.BrightnessLabel = new System.Windows.Forms.Label();
            this.SaturationUpDown = new System.Windows.Forms.NumericUpDown();
            this.SaturationLabel = new System.Windows.Forms.Label();
            this.HueUpDown = new System.Windows.Forms.NumericUpDown();
            this.StandardColorsComboBox = new System.Windows.Forms.ComboBox();
            this.SolidColorGroupBox.SuspendLayout();
            this.TableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.KelvinUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BrightnessUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SaturationUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.HueUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // SolidColorGroupBox
            // 
            this.SolidColorGroupBox.AutoSize = true;
            this.SolidColorGroupBox.Controls.Add(this.TableLayoutPanel);
            this.SolidColorGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SolidColorGroupBox.Location = new System.Drawing.Point(8, 8);
            this.SolidColorGroupBox.Name = "SolidColorGroupBox";
            this.SolidColorGroupBox.Padding = new System.Windows.Forms.Padding(8);
            this.SolidColorGroupBox.Size = new System.Drawing.Size(1102, 163);
            this.SolidColorGroupBox.TabIndex = 0;
            this.SolidColorGroupBox.TabStop = false;
            this.SolidColorGroupBox.Text = "Solid Color";
            // 
            // TableLayoutPanel
            // 
            this.TableLayoutPanel.AutoSize = true;
            this.TableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.TableLayoutPanel.ColumnCount = 5;
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.TableLayoutPanel.Controls.Add(this.ColorBand, 4, 0);
            this.TableLayoutPanel.Controls.Add(this.HueLabel, 0, 1);
            this.TableLayoutPanel.Controls.Add(this.KelvinUpDown, 3, 2);
            this.TableLayoutPanel.Controls.Add(this.KelvinLabel, 2, 2);
            this.TableLayoutPanel.Controls.Add(this.BrightnessUpDown, 1, 2);
            this.TableLayoutPanel.Controls.Add(this.BrightnessLabel, 0, 2);
            this.TableLayoutPanel.Controls.Add(this.SaturationUpDown, 3, 1);
            this.TableLayoutPanel.Controls.Add(this.SaturationLabel, 2, 1);
            this.TableLayoutPanel.Controls.Add(this.HueUpDown, 1, 1);
            this.TableLayoutPanel.Controls.Add(this.StandardColorsComboBox, 0, 0);
            this.TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TableLayoutPanel.Location = new System.Drawing.Point(8, 27);
            this.TableLayoutPanel.Name = "TableLayoutPanel";
            this.TableLayoutPanel.RowCount = 3;
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TableLayoutPanel.Size = new System.Drawing.Size(1086, 128);
            this.TableLayoutPanel.TabIndex = 1;
            // 
            // ColorBand
            // 
            this.ColorBand.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ColorBand.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ColorBand.Location = new System.Drawing.Point(920, 8);
            this.ColorBand.Margin = new System.Windows.Forms.Padding(8);
            this.ColorBand.Name = "ColorBand";
            this.TableLayoutPanel.SetRowSpan(this.ColorBand, 3);
            this.ColorBand.Size = new System.Drawing.Size(158, 112);
            this.ColorBand.TabIndex = 9;
            this.ColorBand.ColorsChanged += new System.EventHandler<DerekWare.Iris.ColorsChangedEventArgs>(this.ColorBand_ColorsChanged);
            // 
            // HueLabel
            // 
            this.HueLabel.Location = new System.Drawing.Point(8, 52);
            this.HueLabel.Margin = new System.Windows.Forms.Padding(8);
            this.HueLabel.Name = "HueLabel";
            this.HueLabel.Size = new System.Drawing.Size(92, 24);
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
            this.KelvinUpDown.Location = new System.Drawing.Point(572, 94);
            this.KelvinUpDown.Margin = new System.Windows.Forms.Padding(8);
            this.KelvinUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.KelvinUpDown.Name = "KelvinUpDown";
            this.KelvinUpDown.Size = new System.Drawing.Size(332, 26);
            this.KelvinUpDown.TabIndex = 8;
            this.KelvinUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.KelvinUpDown.ValueChanged += new System.EventHandler(this.KelvinUpDown_ValueChanged);
            // 
            // KelvinLabel
            // 
            this.KelvinLabel.Location = new System.Drawing.Point(464, 94);
            this.KelvinLabel.Margin = new System.Windows.Forms.Padding(8);
            this.KelvinLabel.Name = "KelvinLabel";
            this.KelvinLabel.Size = new System.Drawing.Size(92, 24);
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
            this.BrightnessUpDown.Size = new System.Drawing.Size(332, 26);
            this.BrightnessUpDown.TabIndex = 6;
            this.BrightnessUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.BrightnessUpDown.ValueChanged += new System.EventHandler(this.BrightnessUpDown_ValueChanged);
            // 
            // BrightnessLabel
            // 
            this.BrightnessLabel.Location = new System.Drawing.Point(8, 94);
            this.BrightnessLabel.Margin = new System.Windows.Forms.Padding(8);
            this.BrightnessLabel.Name = "BrightnessLabel";
            this.BrightnessLabel.Size = new System.Drawing.Size(92, 24);
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
            this.SaturationUpDown.Location = new System.Drawing.Point(572, 52);
            this.SaturationUpDown.Margin = new System.Windows.Forms.Padding(8);
            this.SaturationUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.SaturationUpDown.Name = "SaturationUpDown";
            this.SaturationUpDown.Size = new System.Drawing.Size(332, 26);
            this.SaturationUpDown.TabIndex = 4;
            this.SaturationUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.SaturationUpDown.ValueChanged += new System.EventHandler(this.SaturationUpDown_ValueChanged);
            // 
            // SaturationLabel
            // 
            this.SaturationLabel.Location = new System.Drawing.Point(464, 52);
            this.SaturationLabel.Margin = new System.Windows.Forms.Padding(8);
            this.SaturationLabel.Name = "SaturationLabel";
            this.SaturationLabel.Size = new System.Drawing.Size(92, 24);
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
            this.HueUpDown.Size = new System.Drawing.Size(332, 26);
            this.HueUpDown.TabIndex = 2;
            this.HueUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.HueUpDown.ValueChanged += new System.EventHandler(this.HueUpDown_ValueChanged);
            // 
            // StandardColorsComboBox
            // 
            this.TableLayoutPanel.SetColumnSpan(this.StandardColorsComboBox, 4);
            this.StandardColorsComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.StandardColorsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.StandardColorsComboBox.FormattingEnabled = true;
            this.StandardColorsComboBox.Location = new System.Drawing.Point(8, 8);
            this.StandardColorsComboBox.Margin = new System.Windows.Forms.Padding(8);
            this.StandardColorsComboBox.Name = "StandardColorsComboBox";
            this.StandardColorsComboBox.Size = new System.Drawing.Size(896, 28);
            this.StandardColorsComboBox.TabIndex = 0;
            this.StandardColorsComboBox.SelectedIndexChanged += new System.EventHandler(this.StandardColorsComboBox_SelectedIndexChanged);
            // 
            // SolidColorPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.SolidColorGroupBox);
            this.Name = "SolidColorPanel";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.Size = new System.Drawing.Size(1118, 179);
            this.SolidColorGroupBox.ResumeLayout(false);
            this.SolidColorGroupBox.PerformLayout();
            this.TableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.KelvinUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BrightnessUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SaturationUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.HueUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox SolidColorGroupBox;
        private System.Windows.Forms.TableLayoutPanel TableLayoutPanel;
        private ColorBand ColorBand;
        private System.Windows.Forms.Label HueLabel;
        private System.Windows.Forms.NumericUpDown KelvinUpDown;
        private System.Windows.Forms.Label KelvinLabel;
        private System.Windows.Forms.NumericUpDown BrightnessUpDown;
        private System.Windows.Forms.Label BrightnessLabel;
        private System.Windows.Forms.NumericUpDown SaturationUpDown;
        private System.Windows.Forms.Label SaturationLabel;
        private System.Windows.Forms.NumericUpDown HueUpDown;
        private System.Windows.Forms.ComboBox StandardColorsComboBox;
    }
}
