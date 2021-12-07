
namespace DerekWare.Iris
{
    partial class BrightnessPanel
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
            this.GroupBox = new CheckGroupBox();
            this.LayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.TrackBar = new System.Windows.Forms.TrackBar();
            this.GroupBox.SuspendLayout();
            this.LayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // GroupBox
            // 
            this.GroupBox.AutoSize = true;
            this.GroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.GroupBox.Controls.Add(this.LayoutPanel);
            this.GroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GroupBox.Location = new System.Drawing.Point(8, 8);
            this.GroupBox.Name = "GroupBox";
            this.GroupBox.Padding = new System.Windows.Forms.Padding(8);
            this.GroupBox.Size = new System.Drawing.Size(187, 83);
            this.GroupBox.TabIndex = 9;
            this.GroupBox.TabStop = false;
            this.GroupBox.Text = "Brightness";
            // 
            // LayoutPanel
            // 
            this.LayoutPanel.AutoSize = true;
            this.LayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.LayoutPanel.ColumnCount = 1;
            this.LayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.LayoutPanel.Controls.Add(this.TrackBar, 0, 0);
            this.LayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LayoutPanel.Location = new System.Drawing.Point(8, 27);
            this.LayoutPanel.Name = "LayoutPanel";
            this.LayoutPanel.Padding = new System.Windows.Forms.Padding(8);
            this.LayoutPanel.RowCount = 1;
            this.LayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.LayoutPanel.Size = new System.Drawing.Size(171, 48);
            this.LayoutPanel.TabIndex = 2;
            // 
            // TrackBar
            // 
            this.TrackBar.AutoSize = false;
            this.TrackBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TrackBar.LargeChange = 10;
            this.TrackBar.Location = new System.Drawing.Point(8, 8);
            this.TrackBar.Margin = new System.Windows.Forms.Padding(0);
            this.TrackBar.Maximum = 100;
            this.TrackBar.Name = "TrackBar";
            this.TrackBar.Size = new System.Drawing.Size(155, 32);
            this.TrackBar.TabIndex = 0;
            this.TrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.TrackBar.ValueChanged += new System.EventHandler(this.BrightnessTrackBar_ValueChanged);
            // 
            // BrightnessPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.GroupBox);
            this.Name = "BrightnessPanel";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.Size = new System.Drawing.Size(203, 99);
            this.GroupBox.ResumeLayout(false);
            this.GroupBox.PerformLayout();
            this.LayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal CheckGroupBox GroupBox;
        private System.Windows.Forms.TableLayoutPanel LayoutPanel;
        private System.Windows.Forms.TrackBar TrackBar;
    }
}
