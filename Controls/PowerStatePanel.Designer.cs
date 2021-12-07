
namespace DerekWare.Iris
{
    partial class PowerStatePanel
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
            this.ComboBox = new System.Windows.Forms.ComboBox();
            this.GroupBox.SuspendLayout();
            this.LayoutPanel.SuspendLayout();
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
            this.GroupBox.Size = new System.Drawing.Size(271, 79);
            this.GroupBox.TabIndex = 9;
            this.GroupBox.TabStop = false;
            this.GroupBox.Text = "Power";
            // 
            // LayoutPanel
            // 
            this.LayoutPanel.AutoSize = true;
            this.LayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.LayoutPanel.ColumnCount = 1;
            this.LayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.LayoutPanel.Controls.Add(this.ComboBox, 0, 0);
            this.LayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LayoutPanel.Location = new System.Drawing.Point(8, 27);
            this.LayoutPanel.Name = "LayoutPanel";
            this.LayoutPanel.Padding = new System.Windows.Forms.Padding(8);
            this.LayoutPanel.RowCount = 1;
            this.LayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.LayoutPanel.Size = new System.Drawing.Size(255, 44);
            this.LayoutPanel.TabIndex = 2;
            // 
            // ComboBox
            // 
            this.LayoutPanel.SetColumnSpan(this.ComboBox, 4);
            this.ComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox.FormattingEnabled = true;
            this.ComboBox.Location = new System.Drawing.Point(8, 8);
            this.ComboBox.Margin = new System.Windows.Forms.Padding(0);
            this.ComboBox.Name = "ComboBox";
            this.ComboBox.Size = new System.Drawing.Size(239, 28);
            this.ComboBox.TabIndex = 0;
            this.ComboBox.SelectedIndexChanged += new System.EventHandler(this.PowerComboBox_SelectedIndexChanged);
            // 
            // PowerStatePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.GroupBox);
            this.Name = "PowerStatePanel";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.Size = new System.Drawing.Size(287, 95);
            this.GroupBox.ResumeLayout(false);
            this.GroupBox.PerformLayout();
            this.LayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal CheckGroupBox GroupBox;
        private System.Windows.Forms.TableLayoutPanel LayoutPanel;
        private System.Windows.Forms.ComboBox ComboBox;
    }
}
