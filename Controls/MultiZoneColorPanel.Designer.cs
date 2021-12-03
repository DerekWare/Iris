
namespace DerekWare.Iris
{
    partial class MultiZoneColorPanel
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
            this.ZoneColorGroupBox = new System.Windows.Forms.GroupBox();
            this.TableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.ColorBand = new DerekWare.Iris.ColorBand();
            this.ZoneColorGroupBox.SuspendLayout();
            this.TableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ZoneColorGroupBox
            // 
            this.ZoneColorGroupBox.AutoSize = true;
            this.ZoneColorGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ZoneColorGroupBox.Controls.Add(this.TableLayoutPanel);
            this.ZoneColorGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ZoneColorGroupBox.Location = new System.Drawing.Point(0, 0);
            this.ZoneColorGroupBox.Margin = new System.Windows.Forms.Padding(0);
            this.ZoneColorGroupBox.Name = "ZoneColorGroupBox";
            this.ZoneColorGroupBox.Padding = new System.Windows.Forms.Padding(8);
            this.ZoneColorGroupBox.Size = new System.Drawing.Size(144, 163);
            this.ZoneColorGroupBox.TabIndex = 13;
            this.ZoneColorGroupBox.TabStop = false;
            this.ZoneColorGroupBox.Text = "Zone Colors";
            // 
            // TableLayoutPanel
            // 
            this.TableLayoutPanel.AutoSize = true;
            this.TableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.TableLayoutPanel.ColumnCount = 1;
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.TableLayoutPanel.Controls.Add(this.ColorBand, 0, 0);
            this.TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TableLayoutPanel.Location = new System.Drawing.Point(8, 27);
            this.TableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.TableLayoutPanel.Name = "TableLayoutPanel";
            this.TableLayoutPanel.RowCount = 1;
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TableLayoutPanel.Size = new System.Drawing.Size(128, 128);
            this.TableLayoutPanel.TabIndex = 2;
            // 
            // ColorBand
            // 
            this.ColorBand.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ColorBand.Location = new System.Drawing.Point(8, 8);
            this.ColorBand.Margin = new System.Windows.Forms.Padding(8);
            this.ColorBand.Name = "ColorBand";
            this.ColorBand.Size = new System.Drawing.Size(112, 112);
            this.ColorBand.TabIndex = 0;
            // 
            // MultiZoneColorPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.ZoneColorGroupBox);
            this.Name = "MultiZoneColorPanel";
            this.Size = new System.Drawing.Size(144, 163);
            this.ZoneColorGroupBox.ResumeLayout(false);
            this.ZoneColorGroupBox.PerformLayout();
            this.TableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox ZoneColorGroupBox;
        private System.Windows.Forms.TableLayoutPanel TableLayoutPanel;
        private ColorBand ColorBand;
    }
}
