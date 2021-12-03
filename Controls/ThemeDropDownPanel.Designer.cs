
namespace DerekWare.Iris
{
    partial class ThemeDropDownPanel
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
            this.ThemeGroupBox = new System.Windows.Forms.GroupBox();
            this.TableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.ThemeComboBox = new System.Windows.Forms.ComboBox();
            this.ThemeGroupBox.SuspendLayout();
            this.TableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ThemeGroupBox
            // 
            this.ThemeGroupBox.AutoSize = true;
            this.ThemeGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ThemeGroupBox.Controls.Add(this.TableLayoutPanel);
            this.ThemeGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ThemeGroupBox.Location = new System.Drawing.Point(8, 8);
            this.ThemeGroupBox.Name = "ThemeGroupBox";
            this.ThemeGroupBox.Padding = new System.Windows.Forms.Padding(8);
            this.ThemeGroupBox.Size = new System.Drawing.Size(159, 85);
            this.ThemeGroupBox.TabIndex = 9;
            this.ThemeGroupBox.TabStop = false;
            this.ThemeGroupBox.Text = "Themes";
            // 
            // TableLayoutPanel
            // 
            this.TableLayoutPanel.AutoSize = true;
            this.TableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.TableLayoutPanel.ColumnCount = 1;
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TableLayoutPanel.Controls.Add(this.ThemeComboBox, 0, 0);
            this.TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TableLayoutPanel.Location = new System.Drawing.Point(8, 27);
            this.TableLayoutPanel.Name = "TableLayoutPanel";
            this.TableLayoutPanel.Padding = new System.Windows.Forms.Padding(8);
            this.TableLayoutPanel.RowCount = 1;
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TableLayoutPanel.Size = new System.Drawing.Size(143, 50);
            this.TableLayoutPanel.TabIndex = 2;
            // 
            // ThemeComboBox
            // 
            this.ThemeComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ThemeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ThemeComboBox.FormattingEnabled = true;
            this.ThemeComboBox.Location = new System.Drawing.Point(11, 11);
            this.ThemeComboBox.Name = "ThemeComboBox";
            this.ThemeComboBox.Size = new System.Drawing.Size(121, 28);
            this.ThemeComboBox.TabIndex = 0;
            this.ThemeComboBox.SelectedIndexChanged += new System.EventHandler(this.ThemeComboBox_SelectedIndexChanged);
            // 
            // ThemeDropDownPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.ThemeGroupBox);
            this.Name = "ThemeDropDownPanel";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.Size = new System.Drawing.Size(175, 101);
            this.ThemeGroupBox.ResumeLayout(false);
            this.ThemeGroupBox.PerformLayout();
            this.TableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TableLayoutPanel;
        private System.Windows.Forms.GroupBox ThemeGroupBox;
        private System.Windows.Forms.ComboBox ThemeComboBox;
    }
}
