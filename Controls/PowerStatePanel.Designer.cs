
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
            this.PowerGroupBox = new System.Windows.Forms.GroupBox();
            this.TableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.PowerStateComboBox = new System.Windows.Forms.ComboBox();
            this.PowerGroupBox.SuspendLayout();
            this.TableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // PowerGroupBox
            // 
            this.PowerGroupBox.AutoSize = true;
            this.PowerGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.PowerGroupBox.Controls.Add(this.TableLayoutPanel);
            this.PowerGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PowerGroupBox.Location = new System.Drawing.Point(8, 8);
            this.PowerGroupBox.Name = "PowerGroupBox";
            this.PowerGroupBox.Padding = new System.Windows.Forms.Padding(8);
            this.PowerGroupBox.Size = new System.Drawing.Size(277, 85);
            this.PowerGroupBox.TabIndex = 9;
            this.PowerGroupBox.TabStop = false;
            this.PowerGroupBox.Text = "Power";
            // 
            // TableLayoutPanel
            // 
            this.TableLayoutPanel.AutoSize = true;
            this.TableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.TableLayoutPanel.ColumnCount = 1;
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TableLayoutPanel.Controls.Add(this.PowerStateComboBox, 0, 0);
            this.TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TableLayoutPanel.Location = new System.Drawing.Point(8, 27);
            this.TableLayoutPanel.Name = "TableLayoutPanel";
            this.TableLayoutPanel.Padding = new System.Windows.Forms.Padding(8);
            this.TableLayoutPanel.RowCount = 1;
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TableLayoutPanel.Size = new System.Drawing.Size(261, 50);
            this.TableLayoutPanel.TabIndex = 2;
            // 
            // PowerStateComboBox
            // 
            this.PowerStateComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TableLayoutPanel.SetColumnSpan(this.PowerStateComboBox, 4);
            this.PowerStateComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PowerStateComboBox.FormattingEnabled = true;
            this.PowerStateComboBox.Location = new System.Drawing.Point(11, 11);
            this.PowerStateComboBox.Name = "PowerStateComboBox";
            this.PowerStateComboBox.Size = new System.Drawing.Size(239, 28);
            this.PowerStateComboBox.TabIndex = 0;
            this.PowerStateComboBox.SelectedIndexChanged += new System.EventHandler(this.PowerComboBox_SelectedIndexChanged);
            // 
            // PowerStatePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.PowerGroupBox);
            this.Name = "PowerStatePanel";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.Size = new System.Drawing.Size(293, 101);
            this.PowerGroupBox.ResumeLayout(false);
            this.PowerGroupBox.PerformLayout();
            this.TableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox PowerGroupBox;
        private System.Windows.Forms.TableLayoutPanel TableLayoutPanel;
        private System.Windows.Forms.ComboBox PowerStateComboBox;
    }
}
