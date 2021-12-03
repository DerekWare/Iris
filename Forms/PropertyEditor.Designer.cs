
namespace DerekWare.Iris
{
    partial class PropertyEditor
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.PropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.NopeButton = new System.Windows.Forms.Button();
            this.YupButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // PropertyGrid
            // 
            this.PropertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PropertyGrid.Location = new System.Drawing.Point(0, 0);
            this.PropertyGrid.Name = "PropertyGrid";
            this.PropertyGrid.Size = new System.Drawing.Size(777, 668);
            this.PropertyGrid.TabIndex = 1;
            // 
            // NopeButton
            // 
            this.NopeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.NopeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.NopeButton.Location = new System.Drawing.Point(646, 692);
            this.NopeButton.Name = "NopeButton";
            this.NopeButton.Size = new System.Drawing.Size(120, 40);
            this.NopeButton.TabIndex = 3;
            this.NopeButton.Text = "Cancel";
            this.NopeButton.UseVisualStyleBackColor = true;
            this.NopeButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // YupButton
            // 
            this.YupButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.YupButton.Location = new System.Drawing.Point(520, 692);
            this.YupButton.Name = "YupButton";
            this.YupButton.Size = new System.Drawing.Size(120, 40);
            this.YupButton.TabIndex = 2;
            this.YupButton.Text = "OK";
            this.YupButton.UseVisualStyleBackColor = true;
            this.YupButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // PropertyEditor
            // 
            this.AcceptButton = this.YupButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.NopeButton;
            this.ClientSize = new System.Drawing.Size(778, 744);
            this.Controls.Add(this.YupButton);
            this.Controls.Add(this.NopeButton);
            this.Controls.Add(this.PropertyGrid);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PropertyEditor";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Properties";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid PropertyGrid;
        private System.Windows.Forms.Button NopeButton;
        private System.Windows.Forms.Button YupButton;
    }
}