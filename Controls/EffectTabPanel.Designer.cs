
using DerekWare.Iris.Controls;

namespace DerekWare.Iris
{
    partial class EffectTabPanel
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
            this.TabControl = new EffectTabControl();
            this.GroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // GroupBox
            // 
            this.GroupBox.AutoSize = true;
            this.GroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.GroupBox.Controls.Add(this.TabControl);
            this.GroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GroupBox.Location = new System.Drawing.Point(8, 8);
            this.GroupBox.Name = "GroupBox";
            this.GroupBox.Padding = new System.Windows.Forms.Padding(8);
            this.GroupBox.Size = new System.Drawing.Size(32, 51);
            this.GroupBox.TabIndex = 9;
            this.GroupBox.TabStop = false;
            this.GroupBox.Text = "Effects";
            // 
            // TabControl
            // 
            this.TabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabControl.Location = new System.Drawing.Point(8, 27);
            this.TabControl.Name = "TabControl";
            this.TabControl.TabIndex = 2;
            // 
            // EffectPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.GroupBox);
            this.Name = "EffectTabPanel";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.Size = new System.Drawing.Size(48, 67);
            this.GroupBox.ResumeLayout(false);
            this.GroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private EffectTabControl TabControl;
        public CheckGroupBox GroupBox;
    }
}
