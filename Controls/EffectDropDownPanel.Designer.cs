
namespace DerekWare.Iris
{
    partial class EffectDropDownPanel
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
            this.EffectGroupBox = new System.Windows.Forms.GroupBox();
            this.TableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.EffectComboBox = new System.Windows.Forms.ComboBox();
            this.EffectGroupBox.SuspendLayout();
            this.TableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // EffectGroupBox
            // 
            this.EffectGroupBox.AutoSize = true;
            this.EffectGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.EffectGroupBox.Controls.Add(this.TableLayoutPanel);
            this.EffectGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EffectGroupBox.Location = new System.Drawing.Point(8, 8);
            this.EffectGroupBox.Name = "EffectGroupBox";
            this.EffectGroupBox.Padding = new System.Windows.Forms.Padding(8);
            this.EffectGroupBox.Size = new System.Drawing.Size(159, 85);
            this.EffectGroupBox.TabIndex = 9;
            this.EffectGroupBox.TabStop = false;
            this.EffectGroupBox.Text = "Effects";
            // 
            // TableLayoutPanel
            // 
            this.TableLayoutPanel.AutoSize = true;
            this.TableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.TableLayoutPanel.ColumnCount = 1;
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TableLayoutPanel.Controls.Add(this.EffectComboBox, 0, 0);
            this.TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TableLayoutPanel.Location = new System.Drawing.Point(8, 27);
            this.TableLayoutPanel.Name = "TableLayoutPanel";
            this.TableLayoutPanel.Padding = new System.Windows.Forms.Padding(8);
            this.TableLayoutPanel.RowCount = 1;
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TableLayoutPanel.Size = new System.Drawing.Size(143, 50);
            this.TableLayoutPanel.TabIndex = 2;
            // 
            // EffectComboBox
            // 
            this.EffectComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EffectComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.EffectComboBox.FormattingEnabled = true;
            this.EffectComboBox.Location = new System.Drawing.Point(11, 11);
            this.EffectComboBox.Name = "EffectComboBox";
            this.EffectComboBox.Size = new System.Drawing.Size(121, 28);
            this.EffectComboBox.TabIndex = 0;
            this.EffectComboBox.SelectedIndexChanged += new System.EventHandler(this.EffectComboBox_SelectedIndexChanged);
            // 
            // EffectDropDownPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.EffectGroupBox);
            this.Name = "EffectDropDownPanel";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.Size = new System.Drawing.Size(175, 101);
            this.EffectGroupBox.ResumeLayout(false);
            this.EffectGroupBox.PerformLayout();
            this.TableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TableLayoutPanel;
        private System.Windows.Forms.GroupBox EffectGroupBox;
        private System.Windows.Forms.ComboBox EffectComboBox;
    }
}
