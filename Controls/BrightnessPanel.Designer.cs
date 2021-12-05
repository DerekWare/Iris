
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
            this.BrightnessGroupBox = new System.Windows.Forms.GroupBox();
            this.TableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.BrightnessTrackBar = new System.Windows.Forms.TrackBar();
            this.BrightnessGroupBox.SuspendLayout();
            this.TableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BrightnessTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // BrightnessGroupBox
            // 
            this.BrightnessGroupBox.AutoSize = true;
            this.BrightnessGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BrightnessGroupBox.Controls.Add(this.TableLayoutPanel);
            this.BrightnessGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BrightnessGroupBox.Location = new System.Drawing.Point(8, 8);
            this.BrightnessGroupBox.Name = "BrightnessGroupBox";
            this.BrightnessGroupBox.Padding = new System.Windows.Forms.Padding(8);
            this.BrightnessGroupBox.Size = new System.Drawing.Size(142, 126);
            this.BrightnessGroupBox.TabIndex = 9;
            this.BrightnessGroupBox.TabStop = false;
            this.BrightnessGroupBox.Text = "Brightness";
            // 
            // TableLayoutPanel
            // 
            this.TableLayoutPanel.AutoSize = true;
            this.TableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.TableLayoutPanel.ColumnCount = 1;
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TableLayoutPanel.Controls.Add(this.BrightnessTrackBar, 0, 0);
            this.TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TableLayoutPanel.Location = new System.Drawing.Point(8, 27);
            this.TableLayoutPanel.Name = "TableLayoutPanel";
            this.TableLayoutPanel.Padding = new System.Windows.Forms.Padding(8);
            this.TableLayoutPanel.RowCount = 1;
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TableLayoutPanel.Size = new System.Drawing.Size(126, 91);
            this.TableLayoutPanel.TabIndex = 2;
            // 
            // BrightnessTrackBar
            // 
            this.BrightnessTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BrightnessTrackBar.LargeChange = 10;
            this.BrightnessTrackBar.Location = new System.Drawing.Point(11, 11);
            this.BrightnessTrackBar.Maximum = 100;
            this.BrightnessTrackBar.Name = "BrightnessTrackBar";
            this.BrightnessTrackBar.Size = new System.Drawing.Size(104, 69);
            this.BrightnessTrackBar.TabIndex = 0;
            this.BrightnessTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.BrightnessTrackBar.ValueChanged += new System.EventHandler(this.BrightnessTrackBar_ValueChanged);
            // 
            // BrightnessPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.BrightnessGroupBox);
            this.Name = "BrightnessPanel";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.Size = new System.Drawing.Size(158, 142);
            this.BrightnessGroupBox.ResumeLayout(false);
            this.BrightnessGroupBox.PerformLayout();
            this.TableLayoutPanel.ResumeLayout(false);
            this.TableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BrightnessTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox BrightnessGroupBox;
        private System.Windows.Forms.TableLayoutPanel TableLayoutPanel;
        private System.Windows.Forms.TrackBar BrightnessTrackBar;
    }
}
